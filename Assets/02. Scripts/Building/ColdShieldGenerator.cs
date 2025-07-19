using System.Collections.Generic;
using UnityEngine;

public class ColdShieldGenerator : Building
{
    [SerializeField] private SpriteRenderer spriteRendererBuilding;
    [SerializeField] private Sprite spriteBuilding;
    [SerializeField] private Sprite spriteOnStartBuilding;
    
    protected readonly Dictionary<EManaLevel, int> TimerInfo = new()
    {
        { EManaLevel.One, 20 },
        { EManaLevel.Two, 30 },
        { EManaLevel.Three, 40 }
    };
    
    protected readonly Dictionary<EManaLevel, int> TemperatureInfo = new()
    {
        { EManaLevel.One, 7 },
        { EManaLevel.Two, 13 },
        { EManaLevel.Three, 18 }
    };

    public override void SetSortingLayer(string layerName)
    {
        base.SetSortingLayer(layerName);
        
        spriteRendererBuilding.sortingLayerName = layerName;
    }
    
    public override void OnDragging()
    {
        base.OnDragging();
        
        spriteRendererBuilding.sprite = spriteBuilding;
    }
    
    public override void OnStartBuild(Vector2 coordinate)
    {
        base.OnStartBuild(coordinate);
        
        spriteRendererBuilding.sprite = spriteOnStartBuilding;
    }
    
    public override void OnFinishBuild()
    {
        base.OnFinishBuild();
        
        spriteRendererBuilding.sprite = spriteBuilding;
    }
    
    public override void OnCollisionMana(EManaLevel manaLevel)
    {
        var timer = TimerInfo[manaLevel];
        var temperature = TemperatureInfo[manaLevel];
        var manaCost = ManaCostInfo[manaLevel];
        GameManager.Instance.AddGold(manaCost);

        // TODO: 이펙트 적용
        switch (manaLevel)
        {
            case EManaLevel.One:
                foreach (var (x, y) in FiveDirections)
                {
                    
                    TileNodeSystem.TileNodeGrid[x, y].ApplyTemperature(temperature, timer);
                }
                break;
            
            case EManaLevel.Two:
            case EManaLevel.Three:
                foreach (var (x, y) in NineDirections)
                {
                    TileNodeSystem.TileNodeGrid[x, y].ApplyTemperature(temperature, timer);
                }
                break;
            default:
                Debug.Log($"Invalid mana level: {manaLevel}");
                break;
        }
        
    }
}
