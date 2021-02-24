using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _jumpStrength;

    private Rigidbody2D _rb2d;
    private bool _isGrounded;

    void Awake() => _rb2d = GetComponent<Rigidbody2D>();    

    void FixedUpdate()
    {
        var moveAxes = Input.GetAxis("Horizontal");
        var jumpToggled = Input.GetKey(KeyCode.Space);

        _rb2d.AddForce(new Vector2(moveAxes * _speed, 0.0f));

        var position = _rb2d.position;
        position.x += moveAxes * _speed * Time.deltaTime;
        _rb2d.position = position;
        
        _rb2d.AddTorque(-moveAxes * _speed * 50.0f * Time.fixedDeltaTime);
        
        if(_isGrounded && jumpToggled)
        {
            _isGrounded = false;
            _rb2d.AddForce(new Vector2(0.0f, _jumpStrength));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        _isGrounded = true;  
    }
}
