using System.Collections.Generic;
using Core.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Building : MonoBehaviour
{
    [SerializeField] private int cost;
    [SerializeField] private Define.EBuildingType buildingType = Define.EBuildingType.None;
    [SerializeField] private GameObject builderPrefab;
    
    public bool IsInit { get; private set; } = false;
    public bool IsConstructing { get; private set; } = false;
    public Define.EBuildingType BuildingType => buildingType;
    public int Cost => cost;
    
    protected readonly float PreviewImageOpacity = 0.5f;
    protected TileNodeSystem TileNodeSystem;
    protected BuilderSystem BuilderSystem;
    protected Vector2 Coordinate;
    protected EManaLevel CurrentManaLevel = EManaLevel.None;

    protected readonly int MaxIndex = 7;
    protected readonly List<(int, int)> FiveDirections = new()
    {
        (0, 0), (0, 1), (1, 0), (0, -1), (-1, 0)
    };
    
    protected readonly List<(int, int)> NineDirections = new()
    {
        (0, 0), (0, 1), (1, 0), (0, -1), (-1, 0), (1, 1), (1, -1), (-1, 1), (-1, -1)
    };
    
    protected readonly Dictionary<EManaLevel, int> ManaCostInfo = new()
    {
        { EManaLevel.One, 1 },
        { EManaLevel.Two, 4 },
        { EManaLevel.Three, 15 }
    };
    
    protected virtual void Start()
    {
        TileNodeSystem = FindObjectOfType<TileNodeSystem>();
        BuilderSystem = FindObjectOfType<BuilderSystem>();
    }
    
    public virtual void SetSortingLayer(string layerName)
    {
        // Implement in derived classes
    }

    public virtual void OnDragging()
    {
        TogglePreviewImage(true);
    }

    public virtual void OnStartBuild(Vector2 coordinate)
    {
        IsConstructing = true;
        Coordinate = coordinate;
        
        int start;
        if (Coordinate.y < 2)
            start = 2;
        else if (Coordinate.y < 5)
            start = 1;
        else
            start = 0;
        BuilderSystem.OnStartBuilder(start, this.transform, OnFinishBuild);
    }

    public virtual void OnFinishBuild()
    {
        IsInit = true;
        IsConstructing = false;
        TogglePreviewImage(false);
    }


    public virtual void OnDeActivate()
    {
        
    }
    
    protected virtual void TogglePreviewImage(bool isOn)
    {
        
    }
    
    public abstract void OnCollisionMana(EManaLevel manaLevel);
}