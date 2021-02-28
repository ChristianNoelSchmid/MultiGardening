using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    [SerializeField]
    private Plant _plant;
    public Plant Plant => _plant;

    private double _plantStartTime;
    private SpriteRenderer _plantSprite;
    private Transform _plantTransform;

    private void Awake()
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); 
        TimeSpan diff = DateTime.UtcNow - origin;

        _plantStartTime = diff.TotalSeconds;
        _plantTransform = transform.Find("Plant");
        _plantSprite = _plantTransform.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        int stage = 0; 
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        double currentSeconds = (DateTime.UtcNow - origin).TotalSeconds - _plantStartTime;

        while(stage < _plant.Stages.Count && currentSeconds > _plant.Stages[stage].timeIntervalSec)
        {
            currentSeconds -= _plant.Stages[stage].timeIntervalSec;
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
                1.0f, _plant.Stages[stage].minSize,
                (float)(_plant.Stages[stage].timeIntervalSec - currentSeconds) / (float)_plant.Stages[stage].timeIntervalSec
            );
            _plantTransform.localScale = new Vector3(scale, scale, 1.0f);
        }
    }
}
