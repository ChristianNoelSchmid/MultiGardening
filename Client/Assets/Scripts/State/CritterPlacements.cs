using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Server.Networking;
using Server.Networking.NetworkEvents;

/// <summary>
/// Handles the placement and movement of Critters on the map
/// </summary>
public class CritterPlacements : MonoBehaviour
{
    [SerializeField]
    private NetworkEventHandler _eventHandler;

    private GameObject[] _critters;

    private Dictionary<int, CritterMovement> _critterMovements;

    private void Awake()
    {
        _critters = Resources.LoadAll<GameObject>("Critters")
            .OrderBy(c => c.GetComponent<CritterMovement>().Index)
            .ToArray();
        _critterMovements = new Dictionary<int, CritterMovement>();
    }

    /// <summary>
    /// Creates a new Critter by CritterPlacement info, using the _critters
    /// objects to instantiate the particular one.
    /// </summary>
    /// <param name="created">The CritterPlacement represented the created Critter</param>
    /// <returns></returns>
    public CritterMovement CreateCritter(CritterPlacement created)
    {
        var spawnPos = new Vector2(22.5f, -3.0f);

        var newMovement = Instantiate(_critters[created.Type], spawnPos, Quaternion.identity)
            .GetComponent<CritterMovement>(); 
        newMovement.SetTarget(new Vector2(created.Position.X, created.Position.Y) * 3.0f);

        _critterMovements.Add(created.Id, newMovement); 

        return newMovement;
    }

    /// <summary>
    /// Sets the target the specified Critter (by id) to a new position
    /// </summary>
    /// <param name="moved">The data representing the Critter's Id and new position</param>
    public void MoveCritter(CritterPlacement moved)
    {
        _critterMovements[moved.Id].SetTarget(
            new Vector2(moved.Position.X, moved.Position.Y) * 3.0f
        );
    }

    /// <summary>
    /// Imports all Critters from the collection of critters,
    /// spawning them at their specific locations, rather than off the map
    /// </summary>
    /// <param name="placement">The collection of CritterPlacements</param>
    public void ImportCritters(IEnumerable<CritterPlacement> placement)
    {
        foreach(var critter in placement)
        {
            var newCritter = CreateCritter(critter); 
            newCritter.transform.position = new Vector2(critter.Position.X, critter.Position.Y) * 3.0f;
        }    
    }
}
