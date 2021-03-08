using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Server.Networking;

public class LoginHandler : MonoBehaviour
{
    [SerializeField]
    private NetworkDatagramHandler _handler;

    [SerializeField]
    private UpdateLogin _updateLogin;

    public void AttemptLogin(string ip)
    {
        _updateLogin.SetErrorDisplayed(false);

        if(_handler.StartHandler(ip))
        {
            _updateLogin.Hide();
            PlayerControls.IsEnabled = true;
        }
        else
            _updateLogin.SetErrorDisplayed(true);
    }
}
