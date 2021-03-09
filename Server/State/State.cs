using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Server.Models;
using Server.Networking;
using Server.Networking.NetworkEvents;

namespace Server.State
{
    /// <summary>
    /// Holds all state information concerning the Server and
    /// its functionality. Planting and Critter information
    /// is stored and compared when clients send information.
    /// </summary>
    public class ServerState
    {
        // The PlantInfo for each plant that can be positioned by the client
        private readonly PlantInfo [] _plantInfo = new PlantInfo [] 
        {
            new (PlantType: 0, SecondsToGrow: 10.0f), // Barell Cactus
            new (PlantType: 1, SecondsToGrow: 10.0f), // Aloe Vera
            new (PlantType: 2, SecondsToGrow: 15.0f)  // Pumpkin
        };

        // The critter info for each critter that can be positioned
        // by the server
        private readonly CritterInfo [] _critterInfo = new CritterInfo []
        {
            // Bumblebee
            new (CritterType: 0, SecondsToUpdate: 10, 
                 PlantTypeAttractions: ImmutableList.Create<int>(0, 2)),

            // Ladybug
            new (CritterType: 1, SecondsToUpdate: 30,
                 PlantTypeAttractions: ImmutableList.Create<int>(1, 2))
        };

        // The world bounds
        private readonly Range rangeX = 0..14;
        private readonly Range rangeY = 0..5;

        // Reference to the NetworkDatagramHandler
        private NetworkDatagramHandler _datagramHandler;

        /// <summary>
        /// Retrieves a StateSnapshot of the Server's current State
        /// (plants and critters)
        /// </summary>
        /// <returns>The Server's StateSnapshot</returns>
        public StateSnapshot GetSnapshot() => new StateSnapshot(
            PlantSnapshotData: _plantData.Select(
                _ => new PlantPlacement(_.Key, _.Value.Type, _.Value.FullyGrownTime)
            ).ToImmutableList(),
            CritterSnapshotData: _critterData.Select(
                _ => new CritterPlacement(_.Value.Id, _.Value.Type, _.Key)
            ).ToImmutableList()
        );

        // Lock for controlling multithreading of ServerState data
        private object _stateLock = new object();

        // Dictionary for all PlantData in current ServerState
        private ImmutableDictionary<GridPosition, PlantData> _plantData;

        // Dictionary for all CritterData in current ServerState
        private ImmutableDictionary<GridPosition, CritterData> _critterData;
        private ImmutableList<int> _critterTypeCounts;
        private int _critterId;

        private Random random;

        public ServerState(NetworkDatagramHandler datagramHandler) 
        {
            _plantData = ImmutableDictionary<GridPosition, PlantData>.Empty;
            _critterData = ImmutableDictionary<GridPosition, CritterData>.Empty;
            _datagramHandler = datagramHandler;
            _critterId = 0;

            _critterTypeCounts = ImmutableList<int>.Empty;
            for(int i = 0; i < _critterInfo.Length; ++i)
                _critterTypeCounts = _critterTypeCounts.Add(0);

            random = new Random();

            new Thread(CritterSpawner){ IsBackground = true }.Start();
        }

        /// <summary>
        /// Attempts to add a plant to the ServerState. Requires that the GridPosition
        /// given falls in the bounds of the server's world, and that the GridPosition isn't
        /// currently habitated by another plant.
        /// </summary>
        /// <param name="placement">The plant info</param>
        /// <returns>A copy of the placement given, with an updated TimeToComplete</returns>
        public Option<PlantPlacement> TryAddPlant(PlantPlacement placement)
        {
            // Return null if GridPosition is outside of bounds
            if(placement.Position.X < rangeX.Start.Value || placement.Position.X > rangeX.End.Value &&
               placement.Position.Y < rangeY.Start.Value && placement.Position.Y > rangeY.End.Value)
                return Option<PlantPlacement>.None;

            lock(_stateLock)
            {
                // Check that there isn't a plant which is already planted at
                // the GridPosition
                if(_plantData.ContainsKey(placement.Position)) 
                    return Option<PlantPlacement>.None;

                _plantData = _plantData.Add(
                    placement.Position, 
                    new (placement.PlantType,
                         DateTime.UtcNow.AddSeconds(_plantInfo[placement.PlantType].SecondsToGrow))
                );
            }

            // Return the same placement, with an updated
            // DateTime what represents to clients when the
            // plant is fully grown.
            return Option<PlantPlacement>.Some(
                placement with
                {
                    TimeToComplete = DateTime.UtcNow
                        .AddSeconds(
                            _plantInfo[placement.PlantType].SecondsToGrow
                        )
                }
            );
        }     

        // Determines whether new Critters need to be spawned by checking the
        // ratio of particular Plants to their attracted Critter.
        private void CritterSpawner()
        {
            while(true)
            {
                lock(_stateLock)
                {
                    // Create a list that represents the count for each Plant
                    // associated by their Critter attraction type
                    var desiredCritterCount = _critterInfo.Select(
                        cInfo => _plantData.Values.Where(
                                data => cInfo.PlantTypeAttractions.Contains(data.Type) &&
                                        data.FullyGrownTime < DateTime.UtcNow
                            ).Count()
                    ).ToArray();

                    // Cycle through the recorded critter type counts, and determine
                    // if there's any lack of Critters. If so, spawn the Critter, and send
                    // the information back to all clients.
                    for(int i = 0; i < _critterTypeCounts.Count; ++i) 
                    {
                        while(_critterTypeCounts[i] < desiredCritterCount[i] / 5)
                        {
                            var newPlacement = SpawnCritter(i);
                            _datagramHandler.SendToAll(
                                new CreatedCritter
                                {
                                    Placement = newPlacement
                                }.CreateString(), true
                            );
                        }
                    }

                    // Check whether any Critters already on the map
                    // need to be moved to a new position
                    foreach(var pos in _critterData.Keys)
                    {
                        if(_critterData[pos].UpdateTime < DateTime.UtcNow)
                        {

                            // If they do, locate a new GridPosition based on
                            // the Critters Plant attraction types. Update the Dictionary
                            // to include the new position
                            var oldData = _critterData[pos];
                            _critterData = _critterData.Remove(pos);
                            var newPosition = FindOpenCritterPos(oldData.Type);

                            _critterData = _critterData.Add(
                                newPosition,
                                oldData with { 
                                    UpdateTime = DateTime.UtcNow
                                        .AddSeconds(_critterInfo.First(info => info.CritterType == oldData.Type).SecondsToUpdate) 
                                }
                            );

                            // Send the new critter movement information to
                            // the Clients
                            _datagramHandler.SendToAll(
                                new MovedCritter
                                {
                                    Placement = new CritterPlacement (
                                        Id: oldData.Id,
                                        Type: oldData.Type,
                                        Position: newPosition 
                                    )
                                }.CreateString(), true
                            );
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }

        // Spawns a new critter, returning a CritterPlacement reference ready
        // to return to the Clients
        private CritterPlacement SpawnCritter(int type)
        {
            _critterTypeCounts = _critterTypeCounts.SetItem(type, _critterTypeCounts[type] + 1);

            var newCritter = new CritterData(
                Id: _critterId++, Type: type,
                UpdateTime: DateTime.UtcNow
                    .AddSeconds(_critterInfo.First(_ => _.CritterType == type).SecondsToUpdate)
            );

            var newPosition = FindOpenCritterPos(type);

            _critterData = _critterData.Add(newPosition, newCritter);

            return new (
                Id: newCritter.Id,
                Type: newCritter.Type,
                Position: newPosition
            );
        }

        // Determines a position where there exists a Plant
        // which the Critter type given is attracted to, and which
        // currently is hosting no Critters.
        private GridPosition FindOpenCritterPos(int type)
        {
            var openPositions = _plantData.Keys.Where(
                key =>     _critterInfo[type].PlantTypeAttractions.Contains(_plantData[key].Type) 
                        && !_critterData.ContainsKey(key)
                        && _plantData[key].FullyGrownTime < DateTime.UtcNow
            ).ToArray();

            return openPositions[random.Next(0, openPositions.Length - 1)];
        }
    }
}