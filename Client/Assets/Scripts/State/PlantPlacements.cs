using System;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Server.Models;
using Server.Networking;

/// <summary>
/// Handles the placements of Plants on the map
/// </summary>
public class PlantPlacements : MonoBehaviour
{
    [SerializeField]
    private NetworkEventHandler _eventHandler;

    [SerializeField]
    private SelectedPlant _selectedPlant;

    private Dictionary<GridPosition, PlantPlacement> _placements;

    private GameObject[] _plants;

    private void Awake()
    {
        _placements = new Dictionary<GridPosition, PlantPlacement>();
        _plants = Resources.LoadAll<GameObject>("Plants")
            .OrderBy(plant => plant.GetComponent<PlantGrowth>().Plant.Index).ToArray();
    }

    private void Update()
    {
        // If the player has clicked a new location to plant,
        // inform the NetworkEventHandler to send a request
        // to the Server
        if(PlayerControls.PlantClicked)
        {
            _eventHandler.TryPlantPlacement(
                new PlantPlacement 
                {
                    Position = new GridPosition 
                    {
                        X = PlayerControls.GridPosition.x, 
                        Y = PlayerControls.GridPosition.y
                    }, 
                    PlantType = _selectedPlant.Selected,
                    TimeToComplete = DateTime.UtcNow
                }
            );
        }
    }

    /// <summary>
    /// Places a new Plant onto the map, using supplied PlantPlacement info,
    /// and the _plants objects
    /// </summary>
    /// <param name="placement">The Plant placement information</param>
    public void Place(PlantPlacement placement)
    {
        _placements.Add(placement.Position, placement);

        var newPlant = Instantiate(
            _plants[placement.PlantType], 
            new Vector2(placement.Position.X * 3, placement.Position.Y * 3),
            Quaternion.identity
        ).GetComponent<PlantGrowth>();

        newPlant.SetPlantStartTime(placement.TimeToComplete);
    }

    /// <summary>
    /// Imports all Plants from the Collection
    /// </summary>
    /// <param name="placements">The collection of PlantPlacements</param>
    public void ImportPlants(IEnumerable<PlantPlacement> placements)
    {
        foreach(var placement in placements)
            Place(placement);
    }
}
