using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class StringEvent : UnityEvent<string> { }

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
            Debug.Log("Entered!");
        }
    }
}
