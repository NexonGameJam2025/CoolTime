using System;
using Core.Scripts;
using UnityEngine;

public class PuzzleScene : BaseScene
{
    [SerializeField] private Building startBuilding;
    [SerializeField] private TileNode startNode;
    
    protected override void Awake()
    {
        base.Awake();

        Init();
    }

    public void Start()
    {
        GameManager.Instance.WallCount = 0;
        
        var spawnedBuilding = Instantiate(startBuilding, startNode.transform.position, Quaternion.identity);
        spawnedBuilding.OnBuilderStart(startNode.Coordinate);
        spawnedBuilding.OnFinishBuildEvent += () =>
        {
            spawnedBuilding.OnCollisionMana(EManaLevel.Two);
        };
        startNode.SetBuilding(spawnedBuilding);
    }

    public override void Init()
    {
        base.Init();

    }
}
