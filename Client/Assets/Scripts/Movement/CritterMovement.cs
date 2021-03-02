using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterMovement : MonoBehaviour
{
    [SerializeField]
    private int _index;
    public int Index => _index;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _bumbleLevel;

    [SerializeField]
    private float _margin;

    private Vector2 _target;

    private Transform _transform;
    private Animator _animator;
    private bool _reachedTarget = false;

    void Awake()
    {
        _transform = transform;
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        SetTarget(
            GameObject.Find("Square").transform.position
        );
    }

    void Update()
    {
        if(!_reachedTarget)
        {
            if(Vector2.Distance(_transform.position, _target) < _speed * Time.deltaTime)
                _reachedTarget = true;

            else
                _transform.position = Vector2.Lerp(
                    _transform.position, 
                    _transform.position + Vector3.ClampMagnitude((_target - (Vector2)_transform.position), 1.0f),
                    _speed * Time.deltaTime
                );
        } 
        else
        {
            var perlinNoise = new Vector2 (
                (2.0f * Mathf.PerlinNoise(Time.time, Time.time)) - 1.0f,
                (2.0f * Mathf.PerlinNoise(1000f + Time.time, 1000f + Time.time)) - 1.0f
            ) * _bumbleLevel;

            _transform.position = Vector2.Lerp(
                _transform.position,
                _target + perlinNoise,
                Time.deltaTime * _speed
            );
        }
        
        if(!_reachedTarget ^ _animator.GetBool("moving"))
            _animator.SetBool("moving", !_reachedTarget);
    }

    public void SetTarget(Vector2 target)
    {
        _target = target + new Vector2 (
            Random.Range(-_margin, _margin),
            Random.Range(-_margin, _margin)
        );
    }
}
