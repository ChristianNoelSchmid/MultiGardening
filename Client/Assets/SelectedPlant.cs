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
    private int _selectedIndex = 0;

    private void Start() => 
        OnPlantUpdated.Invoke(null, _plantSelection[_selectedIndex]);

    private void SelectUp()
    {
        _selectedIndex += 1;
        if(_selectedIndex >= _plantSelection.Length)
            _selectedIndex = 0;

        OnPlantUpdated.Invoke(null, _plantSelection[_selectedIndex]);
    }

    private void SelectDown()
    {
        _selectedIndex -= 1;
        if(_selectedIndex < 0)
            _selectedIndex = _plantSelection.Length - 1;

        OnPlantUpdated.Invoke(null, _plantSelection[_selectedIndex]);
    }

    void Update()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if(scrollDelta >= 0.5f) SelectUp();
        else if(scrollDelta <= -0.5f) SelectDown();
    }
}
