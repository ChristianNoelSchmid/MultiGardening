using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls Camera Movement and Zoom
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;

    [SerializeField]
    private float _zoomSpeed = 2.0f;

    private Camera _camera;
    private Vector2 _cameraUnitSize;

    private Vector2 _cameraPosition;
    private float _cameraZoom;

    private Bounds _cameraBounds;
        
    private float _minZoom = 5.0f;
    private float _maxZoom = 10.0f;

    private Rigidbody2D _rb2d;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        _rb2d = GetComponent<Rigidbody2D>();

        // Create the Camera bounds, modeled by
        // the map
        _cameraBounds = new Bounds();
        _cameraBounds.SetMinMax(
            new Vector2(-2.0f, -2.0f),
            new Vector2(44.0f, 30.0f)
        );

        _cameraPosition = _rb2d.position;
        _cameraZoom = _camera.orthographicSize;
        CalculateCameraUnitSize();
    }

    // Calculates Camera size, by orthographic size, converting
    // it to Unity units
    private void CalculateCameraUnitSize() => 
        _cameraUnitSize = new Vector2(
            _camera.orthographicSize * Screen.width / Screen.height,
            _camera.orthographicSize
        );

    void FixedUpdate()
    {
        // Calculate the Camera size, and move it's position / zoom
        CalculateCameraUnitSize();
        _rb2d.position = Vector2.Lerp(_rb2d.position, _cameraPosition, _speed * Time.fixedDeltaTime);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _cameraZoom, _zoomSpeed * Time.fixedDeltaTime);
    }

    public void IncrementZoom(float zoom)
    {
        _cameraZoom = Mathf.Clamp(_cameraZoom + zoom, _minZoom, _maxZoom);
    }

    public void MoveAxes(Vector2 axes)
    {
        axes *= (1.25f - (_maxZoom - _cameraZoom) / (_maxZoom - _minZoom));
        _cameraPosition = new Vector2 (
            Mathf.Clamp(_cameraPosition.x + axes.x, _cameraBounds.min.x + _cameraUnitSize.x, _cameraBounds.max.x - _cameraUnitSize.x),
            Mathf.Clamp(_cameraPosition.y + axes.y, _cameraBounds.min.y + _cameraUnitSize.y, _cameraBounds.max.y - _cameraUnitSize.y)
        );
    }
}
