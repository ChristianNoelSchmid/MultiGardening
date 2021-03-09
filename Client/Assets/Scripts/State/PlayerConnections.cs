using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Server.Networking.NetworkEvents;

/// <summary>
/// Handles adding new client markers, and moving those
/// markers to their synced positions
/// </summary>
public class PlayerConnections : MonoBehaviour
{
    [SerializeField]
    private GameObject _templateMarker;
    private Dictionary<int, RemoteMovement> _remoteMovements;

    void Awake() => _remoteMovements = new Dictionary<int, RemoteMovement>();     

    // Adds a new Marker if the Client given has
    // synced for the first time. Moves the marker to the
    // supplied GridPosition
    public void UpdateMarker(ClientMovement movement)
    {
        int id = movement.CallerInfo.CallerId;
        if (!_remoteMovements.ContainsKey(id))
        {
            var newRemote = SpawnNewMarker();
            _remoteMovements.Add(id, newRemote);
        }

        _remoteMovements[id].SetMovement(movement.CallerInfo.Value);
    }

    /// <summary>
    /// Removes a marker, based on Client Id, when a
    /// Client disconnects from the Server.
    /// </summary>
    /// <param name="id">The disconnected Client's Id</param>
    public void RemoveMarker(int id)
    {
        if(_remoteMovements.ContainsKey(id))
        {
            Destroy(_remoteMovements[id].gameObject);
            _remoteMovements.Remove(id);
        }
    }

    // Spawns a new Marker for the PlayerConnections
    // to manipulate.
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
