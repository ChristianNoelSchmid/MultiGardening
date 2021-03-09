using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows cycling through all possible Plants that
/// can be set down by Player
/// </summary>
public class SelectedPlant : MonoBehaviour
{
    public EventHandler<Plant> OnPlantUpdated;

    [SerializeField]
    private Plant [] _plantSelection;

    [SerializeField]
    private int _selectedIndex = 0;
    public int Selected => _selectedIndex;
    public Plant Plant => _plantSelection[_selectedIndex];

    private void Start() => 
        OnPlantUpdated.Invoke(null, _plantSelection[_selectedIndex]);

    private void SelectUp()
    {
        if(PlayerControls.IsEnabled)
        {
            _selectedIndex += 1;
            if(_selectedIndex >= _plantSelection.Length)
                _selectedIndex = 0;

            OnPlantUpdated.Invoke(null, _plantSelection[_selectedIndex]);
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
            SelectUp();
    }
}
