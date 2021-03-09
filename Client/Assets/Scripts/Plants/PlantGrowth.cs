using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles a Plant's Growth over time, using supplied
/// time to completion info to dictate its size and sprite
/// </summary>
public class PlantGrowth : MonoBehaviour
{
    [SerializeField]
    private Plant _plant;
    public Plant Plant => _plant;

    private DateTime _timeToCompletion;
    private SpriteRenderer _plantSprite;
    private Transform _plantTransform;

    private void Awake()
    {
        _plantTransform = transform.Find("Plant");
        _plantSprite = _plantTransform.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Get the time past by adding together all time intervals in the Plant,
        // and subtracting that value from _timeToCompletion - Now.
        int stage = 0;
        float timePast = _plant.Stages.Sum(stage => stage.timeIntervalSec) 
            - (float)(_timeToCompletion - DateTime.UtcNow).TotalSeconds;
        float intervalTime = 0.0f;

        // Increment stage until it's summed seconds matches timePast
        while(stage < _plant.Stages.Count && timePast > _plant.Stages[stage].timeIntervalSec)
        {
            timePast -= _plant.Stages[stage].timeIntervalSec;
            stage += 1;
        }

        // If the plant is already grown, simply set its sprite and size to finished
        if(stage >= _plant.Stages.Count) 
        {
            _plantTransform.localScale = Vector3.one;
            _plantSprite.sprite = _plant.FinalSprite;
        }

        // Otherwise, update the sprite and scale
        else 
        {
            _plantSprite.sprite = _plant.Stages[stage].sprite;
            float scale = Mathf.Lerp(
                _plant.Stages[stage].minSize, 1.0f,
                timePast / _plant.Stages[stage].timeIntervalSec 
            );
            _plantTransform.localScale = new Vector3(scale, scale, 1.0f);
        }
    }

    /// <summary>
    /// Sets the Plant's growth start time to the specified value.
    /// </summary>
    /// <param name="time">The time the Plant started growing</param>
    public void SetPlantStartTime(DateTime time) =>
        _timeToCompletion = time;
}
