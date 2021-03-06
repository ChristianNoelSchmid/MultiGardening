using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Networking.NetworkEvents;

namespace Server.Models 
{
    public class StateSnapshot : ISerializable
    {
        public List<PlantPlacement> PlantSnapshotData { get; set; }
        public List<CritterPlacement> CritterSnapshotData { get; set; }
        public string Serialize() 
        {
            var builder = new StringBuilder();

            builder.Append(PlantSnapshotData.Count);

            if(PlantSnapshotData.Count > 0)
            {
                builder.Append("#");
                builder.Append(
                    string.Join("#", 
                    PlantSnapshotData.Select(_ => 
                        new PlantPlacement
                        {
                            Position = _.Position,
                            PlantType = _.PlantType, 
                            TimeToComplete = _.TimeToComplete
                        }.Serialize()
                )));
            }

            builder.Append(CritterSnapshotData.Count);

            if(CritterSnapshotData.Count > 0)
            {
                builder.Append("#");
                builder.Append(
                    string.Join("#", 
                        CritterSnapshotData.Select(_ =>
                            new CritterPlacement
                            {
                                Id = _.Id,
                                Type = _.Type,
                                Position = _.Position
                            }.Serialize()
                )));
            }

            return builder.ToString();
        }
    }
}