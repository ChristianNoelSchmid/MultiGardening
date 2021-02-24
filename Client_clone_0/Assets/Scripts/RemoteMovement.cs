using Server.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;

    private Rigidbody2D _rb2d;
    private ActorMovement _movement;

    void Awake() => _rb2d = GetComponent<Rigidbody2D>(); 

    void FixedUpdate()
    {
        if(_movement != null)
            _rb2d.position = Vector2.Lerp(
                _rb2d.position,
                new Vector2(_movement.Position.Item1, _movement.Position.Item2),
                _speed * Time.fixedDeltaTime
            );
    }

    public void SetActorMovement(ActorMovement movement) => _movement = movement;
}
