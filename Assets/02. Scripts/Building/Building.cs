using System;
using System.Collections.Generic;
using Core.Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Building : MonoBehaviour
{
    [SerializeField] private int cost;
    [SerializeField] private Define.EBuildingType buildingType = Define.EBuildingType.None;
    [SerializeField] private GameObject builderPrefab;
    [SerializeField] protected Sprite[] spriteOnStartBuilding;
    [SerializeField] private ParticleSystem particleSystem;
    
    public bool IsInit { get; private set; } = false;
    public bool IsConstructing { get; private set; } = false;
    public Define.EBuildingType BuildingType => buildingType;
    public int Cost => cost;
    public event Action OnFinishBuildEvent;
    
    protected readonly float CONSTRUCT_ANIM_INTERVAL = 0.2f;
    protected readonly float PreviewImageOpacity = 0.5f;
    protected TileNodeSystem TileNodeSystem;
    protected BuilderSystem BuilderSystem;
    protected Vector2 Coordinate;
    protected EManaLevel CurrentManaLevel = EManaLevel.None;
    protected Sequence OnStartBuildSpriteTween;

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
    
    protected virtual void Awake()
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

    public virtual void OnBuilderStart(Vector2 coordinate)
    {
        Coordinate = coordinate;
        
        IsConstructing = true;
        
        int start;
        if (Coordinate.y < 2)
            start = 2;
        else if (Coordinate.y < 5)
            start = 1;
        else
            start = 0;
        BuilderSystem.OnStartBuilder(start, this.transform, OnStartBuild, OnFinishBuild);
    }

    public virtual void OnStartBuild()
    {
        if (BuildingType != Define.EBuildingType.Wall)
        {
            particleSystem.Play();
        }
    }

    public virtual void OnFinishBuild()
    {
        IsInit = true;
        IsConstructing = false;
        TogglePreviewImage(false);
        if (BuildingType != Define.EBuildingType.Wall)
        {
            particleSystem.Stop();
        }

        GameManager.Instance.ConstructionScore += 1;
        
        OnFinishBuildEvent?.Invoke();
    }


    public virtual void OnDeActivate()
    {
        // TODO: 건물 파괴 로직 작성
        CurrentManaLevel = EManaLevel.None;
        GameManager.Instance.DestructionScore += 1;
        GameManager.Instance.PerfectBuildingDesigner = false;
        GameManager.Instance.DestructionKing++;
    }
    
    protected virtual void TogglePreviewImage(bool isOn)
    {
        
    }
    
    public virtual void OnCollisionMana(EManaLevel manaLevel)
    {
        switch (manaLevel)
        {
            case EManaLevel.One:
                GameManager.Instance.IceCollectInfo.LevelOne += 1;
                break;
            case EManaLevel.Two:
                GameManager.Instance.IceCollectInfo.LevelTwo += 1;
                break;
            case EManaLevel.Three:
                GameManager.Instance.IceCollectInfo.LevelThree += 1;
                break;
        }
    }
}