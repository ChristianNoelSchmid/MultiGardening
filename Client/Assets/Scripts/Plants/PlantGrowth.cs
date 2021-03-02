using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

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
        int stage = 0;
        float timePast = _plant.Stages.Sum(stage => stage.timeIntervalSec) 
            - (float)(_timeToCompletion - DateTime.UtcNow).TotalSeconds;
        float intervalTime = 0.0f;

        while(stage < _plant.Stages.Count && timePast > _plant.Stages[stage].timeIntervalSec)
        {
            timePast -= _plant.Stages[stage].timeIntervalSec;
            stage += 1;
        }

        if(stage >= _plant.Stages.Count) 
        {
            _plantTransform.localScale = Vector3.one;
            _plantSprite.sprite = _plant.FinalSprite;
        }
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

    public void SetPlantStartTime(DateTime time) =>
        _timeToCompletion = time;
}
