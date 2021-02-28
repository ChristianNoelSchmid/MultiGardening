using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedPlant : MonoBehaviour
{
    public EventHandler<Plant> OnPlantUpdated;

    [SerializeField]
    private Plant [] _plantSelection;

    [SerializeField]
    private uint _selectedIndex = 0;
    public uint Selected => _selectedIndex;

    private void Start() => 
        OnPlantUpdated.Invoke(null, _plantSelection[_selectedIndex]);

    private void SelectUp()
    {
        _selectedIndex += 1;
        if(_selectedIndex >= _plantSelection.Length)
            _selectedIndex = 0;

        OnPlantUpdated.Invoke(null, _plantSelection[_selectedIndex]);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
            SelectUp();
    }
}
