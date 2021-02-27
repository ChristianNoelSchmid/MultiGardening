using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Server.Networking.NetworkEvents;

public class PlayerConnections : MonoBehaviour
{
    [SerializeField]
    private GameObject _templateMarker;
    private Dictionary<int, RemoteMovement> _remoteMovements;

    void Awake() => _remoteMovements = new Dictionary<int, RemoteMovement>();     

    public void UpdateMarker(Pinged pinged)
    {
        int id = pinged.CallerInfo.CallerId;
        if (!_remoteMovements.ContainsKey(id))
        {
            var newRemote = SpawnNewMarker();
            _remoteMovements.Add(id, newRemote);
        }

        _remoteMovements[id].SetActorMovement(pinged.CallerInfo.Value);
    }

    public void RemoveMarker(int id)
    {
        if(_remoteMovements.ContainsKey(id))
        {
            Destroy(_remoteMovements[id].gameObject);
            _remoteMovements.Remove(id);
        }
    }

    private RemoteMovement SpawnNewMarker()
    {
        _templateMarker.SetActive(true);

        var newRemote = Instantiate(
            _templateMarker, 
            _templateMarker.transform.parent
        ).GetComponent<RemoteMovement>();

        _templateMarker.SetActive(false);

        return newRemote;
    }
}
