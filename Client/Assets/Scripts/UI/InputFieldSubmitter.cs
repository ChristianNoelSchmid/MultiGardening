using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class StringEvent : UnityEvent<string> { }

/// <summary>
/// Allows ENTER to be pressed when using an InputField
/// to activate some action.
/// </summary>
public class InputFieldSubmitter : MonoBehaviour
{
    public StringEvent _onInputEntered;

    private InputField _inputField;

    void Awake() => _inputField = GetComponent<InputField>();

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return))
        {
            _onInputEntered.Invoke(_inputField.text);
        }
    }
}
