using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the local Player's Marker position
/// </summary>
public class MarkerPosition : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;
    private Transform _transform;

    private void Awake() =>         
        _transform = transform;

    private void Update()
    {
        // Base position off of PlayerControl's
        // GridPosition, so the marker will snap to
        // the grid
        if(PlayerControls.IsEnabled)
        {
            _transform.position = Vector2.Lerp(
                _transform.position,
                PlayerControls.GridPosition * 3,
                Time.deltaTime * _speed 
            );
        }
        // If the PlayerControls are disabled, move the
        // marker off screen.
        else
        {
            _transform.position = new Vector2(-8, -8);
        }
    }
}