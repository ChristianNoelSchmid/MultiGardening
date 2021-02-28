using System;
using System.Collections.Immutable;
using Server.Models;

namespace Server
{
    public class State
    {
        private readonly Range rangeX = 0..14;
        private readonly Range rangeY = 0..5;

        private ImmutableDictionary<GridPosition, PlantPlacement> _plantPlacements;
        private ImmutableDictionary<GridPosition, uint> _critterPlacements;

        public State() 
        {
            _plantPlacements = ImmutableDictionary<GridPosition, PlantPlacement>.Empty;
            _critterPlacements = ImmutableDictionary<GridPosition, uint>.Empty;
        }

        public bool TryAddPlant(PlantPlacement placement)
        {
            if(_plantPlacements.ContainsKey(placement.Position)) 
                return false;
            if(placement.Position.X < rangeX.Start.Value || placement.Position.X > rangeX.End.Value &&
               placement.Position.Y < rangeY.Start.Value && placement.Position.Y > rangeY.End.Value)
                return false;

            _plantPlacements = _plantPlacements.Add(placement.Position, placement);
            return true;
        }     
    }
}