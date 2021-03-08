using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerPosition : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;
    private Transform _transform;

    private void Awake() =>         
        _transform = transform;

    private void Update()
    {
        if(PlayerControls.IsEnabled)
        {
            _transform.position = Vector2.Lerp(
                _transform.position,
                PlayerControls.GridPosition * 3,
                Time.deltaTime * _speed 
            );
        }
        else
        {
            _transform.position = new Vector2(-8, -8);
        }
    }
}