using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Displays the disconnection text if the Player
/// disconnects from the Server
/// </summary>
public class DisplayDisconnection : MonoBehaviour
{
    private CanvasGroup _group;
    private bool _display = false;

    private void Awake() => _group = GetComponent<CanvasGroup>();
    private void Update()
    {
        if(_display && _group.alpha < 1.0f)
            _group.alpha = 1.0f;
    }

    public void Display() => _display = true;
}
