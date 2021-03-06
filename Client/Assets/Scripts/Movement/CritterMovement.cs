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

    private int perlinSeed;

    void Awake()
    {
        _transform = transform;
        _animator = GetComponent<Animator>();
        perlinSeed = Random.Range(0, 2000);
    }

    void Update()
    {
        if(!_reachedTarget)
        {
            if(Vector2.Distance(_transform.position, _target) < _speed * Time.deltaTime)
                _reachedTarget = true;

            else
            {
                if(_transform.position.x < _target.x)
                    _transform.localScale = Vector3.one;
                else
                    _transform.localScale = new Vector3(-1, 1, 1);

                _transform.position = Vector2.Lerp(
                    _transform.position, 
                    _transform.position + Vector3.ClampMagnitude((_target - (Vector2)_transform.position), 1.0f),
                    _speed * Time.deltaTime
                );
            }
        } 
        else
        {
            var perlinNoise = new Vector2 (
                (2.0f * Mathf.PerlinNoise(Time.time + perlinSeed, Time.time + perlinSeed)) - 1.0f,
                (2.0f * Mathf.PerlinNoise(1000f + Time.time + perlinSeed, 1000f + Time.time + perlinSeed)) - 1.0f
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
        _reachedTarget = false;
    }
}
