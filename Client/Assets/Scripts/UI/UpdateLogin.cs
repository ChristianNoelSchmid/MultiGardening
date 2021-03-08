using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateLogin : MonoBehaviour
{
    private CanvasGroup _group;
    private Text _errorText;

    void Awake()
    {
        _group = GetComponent<CanvasGroup>();
        _errorText = transform.Find("Error").GetComponent<Text>();
    }

    public void SetErrorDisplayed(bool displayed)
        =>  _errorText.enabled = displayed;

    public void Hide()
    {
        _group.alpha = 0.0f;
        _group.blocksRaycasts = false;
        _group.interactable = false;
    }
}
