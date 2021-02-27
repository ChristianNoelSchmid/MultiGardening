using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePlantImage : MonoBehaviour
{
    [SerializeField]
    private SelectedPlant _selectedPlant;

    private Image _plantImage;

    void Awake()
    {
        _plantImage = transform.Find("Image").GetComponent<Image>();
        _selectedPlant.OnPlantUpdated += (_, plant) =>
            _plantImage.sprite = plant.FinalSprite;
    }
}
