using System;
using System.Collections.Generic;
using System.Globalization;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    /// <summary>
    /// NetworkEvent representing the Server's welcome to a client,
    /// providing all relevant information to the ServerState of the
    /// game, and assigning the Client an Id.
    /// </summary>
    public class Welcome : NetworkEvent 
    {
        public DataModel<StateSnapshot> Snapshot { get; set; }
        public Welcome() => Snapshot = null;
        public Welcome(string value) 
        {
            string [] args = value.Split('#');

            int plantCount = int.Parse(args[2]);
            int critterCount = int.Parse(args[3 + (plantCount * 4)]);

            var plantPlacements = new List<PlantPlacement>();
            var critterPlacements = new List<CritterPlacement>();

            for(int i = 0; i < plantCount; ++i)
                plantPlacements.Add(
                    new PlantPlacement
                    {
                        Position =  new GridPosition
                        {
                            X = int.Parse(args[3+i*4]),
                            Y = int.Parse(args[3+(i*4)+1])
                        },
                        PlantType = int.Parse(args[3+(i*4)+2]),
                        TimeToComplete = DateTime.ParseExact (
                            args[3+(i*4)+3], "yyyy-MM-dd HH:mm:ss", 
                            CultureInfo.InvariantCulture, DateTimeStyles.None
                        )
                    }
                );

            for(int i = 0; i < critterCount; ++i)
                critterPlacements.Add(
                    new CritterPlacement
                    {
                        Id = int.Parse(args[4+(plantCount*4)+i*4]),
                        Type = int.Parse(args[4+(plantCount*4)+(i*4)+1]),
                        Position = new GridPosition
                        {
                            X = int.Parse(args[4+(plantCount*4)+(i*4)+2]),
                            Y = int.Parse(args[4+(plantCount*4)+(i*4)+3])
                        }
                    }
                );
            
            Snapshot = new DataModel<StateSnapshot>
            {
                CallerId = int.Parse(args[0]),
                Secret = args[1],
                Value = new StateSnapshot
                {
                    PlantSnapshotData = plantPlacements,
                    CritterSnapshotData = critterPlacements
                }
            };
        }

        public string CreateString() => $"Welcome::{Snapshot.Serialize()}";
    }
}