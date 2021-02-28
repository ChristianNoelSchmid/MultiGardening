using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu]
public class Plant : ScriptableObject
{
    [SerializeField]
    private uint _index;
    public uint Index => _index;

    [SerializeField]
    private int _columnSize;
    public int ColumnSize => _columnSize;

    [SerializeField]
    private int _rowSize;
    public int RowSize => _rowSize;

    [SerializeField]
    private List<PlantStage> _stages;
    public ReadOnlyCollection<PlantStage> Stages => _stages.AsReadOnly();

    [SerializeField]
    private Sprite _finalSprite;   
    public Sprite FinalSprite => _finalSprite;
}
