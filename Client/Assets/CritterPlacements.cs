using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Server.Networking;

public class CritterPlacements : MonoBehaviour
{
    [SerializeField]
    private NetworkEventHandler _eventHandler;

    private GameObject[] _critters;

    private void Awake()
    {
        _critters = Resources.LoadAll<GameObject>("Critters")
            .OrderBy(c => c.GetComponent<CritterMovement>().Index)
            .ToArray();
    }
}
