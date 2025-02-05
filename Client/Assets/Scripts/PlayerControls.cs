using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class with static methods representing the Player's
/// control input at any given time, and the Player's position
/// on the map.
/// </summary>
public class PlayerControls : MonoBehaviour
{
    public static bool IsEnabled { get; set; } = false;

    [SerializeField]
    private CameraMovement _cameraMovement;
    private static Camera _mainCamera;

    public static bool PlantClicked { get; private set; }

    private void Awake() => _mainCamera = Camera.main;

    private void FixedUpdate()
    {
        if(IsEnabled)
        {
            _cameraMovement.IncrementZoom(-Input.mouseScrollDelta.y / 3.0f);
            _cameraMovement.MoveAxes(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * 0.5f);
        }
    }

    private void Update() => PlantClicked = IsEnabled ? Input.GetMouseButtonDown(0) : false;

    public static Vector2Int GridPosition
    {
        get
        {
            var mousePos = 
                _mainCamera.ScreenToWorldPoint(
                    Input.mousePosition
                ) / 3.0f;

            return new Vector2Int(
                Mathf.Clamp(Mathf.RoundToInt(mousePos.x), 0, 14),
                Mathf.Clamp(Mathf.RoundToInt(mousePos.y), 0, 5)
            );
        }
    }
}
