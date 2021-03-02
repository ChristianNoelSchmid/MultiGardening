using Server.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;

    private Transform _transform;
    private Vector2 _position;
    private bool _isFlipped;

    void Awake() => _transform = transform;

    void Update()
    {
        _transform.position = Vector2.Lerp(
            _transform.position, _position,
            _speed * Time.deltaTime
        );
    }

    public void SetMovement(ActorMovement movement)
    {
        _position = new Vector2(movement.Position.Item1, movement.Position.Item2);
        _isFlipped = movement.IsFlipped;
    }
    public void SetMovement(GridPosition position) 
    {
        _position = new Vector2(position.X, position.Y) * 3.0f;
        _isFlipped = false;
    }
}
