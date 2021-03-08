using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Server.Networking;
using Server.Networking.NetworkEvents;

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

    public CritterMovement CreateCritter(CritterPlacement created)
    {
        float x = Mathf.Sign(Random.Range(-10, 10));
        float y = Mathf.Sign(Random.Range(-10, 10));
        var spawnPos = new Vector2(22.5f, 11.5f) + (new Vector2(25.5f, 14.5f) * new Vector2(x, y));

        var newMovement = Instantiate(_critters[created.Type], spawnPos, Quaternion.identity)
            .GetComponent<CritterMovement>(); 
        newMovement.SetTarget(new Vector2(created.Position.X, created.Position.Y) * 3.0f);

        _critterMovements.Add(created.Id, newMovement); 

        return newMovement;
    }

    public void MoveCritter(CritterPlacement moved)
    {
        _critterMovements[moved.Id].SetTarget(
            new Vector2(moved.Position.X, moved.Position.Y) * 3.0f
        );
    }

    public void ImportCritters(IEnumerable<CritterPlacement> placement)
    {
        foreach(var critter in placement)
        {
            var newCritter = CreateCritter(critter); 
            newCritter.transform.position = new Vector2(critter.Position.X, critter.Position.Y) * 3.0f;
        }    
    }
}
