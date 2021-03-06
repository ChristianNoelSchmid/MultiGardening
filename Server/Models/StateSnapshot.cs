using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Server.Networking.NetworkEvents;
using Server.State;

namespace Server.Models 
{
    public record StateSnapshot(
        ImmutableList<PlantPlacement> PlantSnapshotData,
        ImmutableList<CritterPlacement> CritterSnapshotData
    ) : ISerializable {
        public string Serialize() 
        {
            StringBuilder builder = new();

            builder.Append(PlantSnapshotData.Count);
            if(PlantSnapshotData.Count > 0)
            {
                builder.Append("#");
                builder.Append(
                    string.Join("#", 
                        PlantSnapshotData.Select(_ => 
                            new PlantPlacement(
                                _.Position,
                                _.PlantType, 
                                _.TimeToComplete
                            ).Serialize()
                )));
            }

            builder.Append($"#{ CritterSnapshotData.Count }");
            if(CritterSnapshotData.Count > 0)
            {
                builder.Append("#");
                builder.Append(
                    string.Join("#", 
                        CritterSnapshotData.Select(_ =>
                            new CritterPlacement(
                                _.Id,
                                _.Type,
                                _.Position
                            ).Serialize()
                )));
            }

            return builder.ToString();
        }
    }
}