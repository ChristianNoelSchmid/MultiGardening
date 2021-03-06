using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record Welcome : NetworkEvent 
    {
        public DataModel<StateSnapshot> Snapshot { get; init; }
        public Welcome() => Snapshot = null;
        public Welcome(string value) 
        {
            string [] args = value.Split('#');

            int plantCount = int.Parse(args[2]);
            int critterCount = int.Parse(args[2 + (plantCount * 4)]);

            var plantPlacements = ImmutableList<PlantPlacement>.Empty;
            var critterPlacements = ImmutableList<CritterPlacement>.Empty;

            for(int i = 0; i < plantCount; ++i)
                plantPlacements = plantPlacements.Add(
                    new PlantPlacement(
                        Position: new GridPosition(
                            X: int.Parse(args[2+i*4]),
                            Y: int.Parse(args[2+(i*4)+1])
                        ),
                        PlantType: int.Parse(args[2+(i*4)+2]),
                        TimeToComplete: DateTime.ParseExact (
                            args[2+(i*4)+3], "yyyy-MM-dd HH:mm:ss", 
                            CultureInfo.InvariantCulture, DateTimeStyles.None
                        )
                    )
                );

            for(int i = 0; i < critterCount; ++i)
                critterPlacements = critterPlacements.Add(
                    new CritterPlacement(
                        Id: int.Parse(args[2+plantCount+i*4]),
                        Type: int.Parse(args[2+plantCount+(i*4)+1]),
                        Position: new GridPosition(
                            X: int.Parse(args[2+plantCount+(i*4)+2]),
                            Y: int.Parse(args[2+plantCount+(i*4)+3])
                        )
                    )
                );
            
            Snapshot = new DataModel<StateSnapshot>(
                CallerId: int.Parse(args[0]),
                Secret: args[1],
                Value: new StateSnapshot(
                    plantPlacements,
                    critterPlacements
                )
            );
        }

        public string CreateString() => $"Welcome::{Snapshot.Serialize()}";
    }
}