using System;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Server.Models;
using Server.Networking;

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
}
