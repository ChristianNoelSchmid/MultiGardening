using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField]
    private CameraMovement _cameraMovement;
    private static Camera _mainCamera;

    public static bool PlantClicked { get; private set; }

    private void Awake() => _mainCamera = Camera.main;

    private void FixedUpdate()
    {
        _cameraMovement.IncrementZoom(-Input.mouseScrollDelta.y / 3.0f);
        _cameraMovement.MoveAxes(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * 0.5f);
    }

    private void Update() => PlantClicked = Input.GetMouseButtonDown(0);

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
