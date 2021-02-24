using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Transform _targetTransform;

    private Transform _transform;

    void Awake() => _transform = transform;        

    void Update()
    {
        Vector3 position = Vector3.Lerp(
            _transform.position,
            _targetTransform.position,
            _speed * Time.deltaTime
        );
        position.z = -10.0f;

        _transform.position = position;
    }
}
