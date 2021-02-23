using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Transform _transform;

    void Awake()
    {
        _transform = transform;        
    }

    void Update()
    {
        _transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition)    
            + new Vector3(0, 0, 10.0f);
    }
}
