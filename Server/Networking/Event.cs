using System;
using System.Text.Json;
using Server.Models;

namespace Server.Networking
{
    public abstract record Event { }
    public record PlayerJoined : Event 
    { 
        public ActorMovement ActorMovement { get; init; }
        public PlayerJoined(string value) => 
            ActorMovement = JsonSerializer.Deserialize<ActorMovement>(value);
    }
    public record PlayerLeft : Event 
    {
        public DataModel CallerInfo { get; init; }
        public PlayerLeft(string value) =>
            CallerInfo = JsonSerializer.Deserialize<DataModel>(value);
    }

    public record Tilled : Event 
    {
        public DataModel<GridPosition> Position { get; init; }
        public Tilled(string value) =>
            Position = JsonSerializer.Deserialize<DataModel<GridPosition>>(value);
    }

    public record Planted : Event 
    {
        public DataModel<PlantPlacement> Placement { get; init; }
        public Planted(string value) =>
            Placement = JsonSerializer.Deserialize<DataModel<PlantPlacement>>(value);
    }

    public record Destroyed : Event
    {
        public DataModel<GridPosition> Position { get; init; }
        public Destroyed(string value) =>
            Position = JsonSerializer.Deserialize<DataModel<GridPosition>>(value);
    }

    public record Pinged : Event
    {
        public DataModel CallerInfo;
        public Pinged(string value) => 
            CallerInfo = JsonSerializer.Deserialize<DataModel>(value);
    }
}