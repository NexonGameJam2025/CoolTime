using System.Collections.Generic;
using UnityEngine;

public class ColdShieldGenerator : Building
{
    [SerializeField] private SpriteRenderer spriteRendererBuilding;
    [SerializeField] private Sprite[] spriteBuilding;
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
        { EManaLevel.Two, 17 },
        { EManaLevel.Three, 25 }
    };

    public override void SetSortingLayer(string layerName)
    {
        base.SetSortingLayer(layerName);
        
        spriteRendererBuilding.sortingLayerName = layerName;
    }
    
    public override void OnDragging()
    {
        base.OnDragging();
        
        spriteRendererBuilding.sprite = spriteBuilding[0];
    }
    
    public override void OnStartBuild(Vector2 coordinate)
    {
        base.OnStartBuild(coordinate);
        
        spriteRendererBuilding.sprite = spriteOnStartBuilding;
    }
    
    public override void OnFinishBuild()
    {
        base.OnFinishBuild();
        
        spriteRendererBuilding.sprite = spriteBuilding[0];
    }
    
    public override void OnCollisionMana(EManaLevel manaLevel)
    {
        Debug.Log($"OnCollisionMana: {manaLevel}");
        var timer = TimerInfo[manaLevel];
        var temperature = TemperatureInfo[manaLevel];
        var manaCost = ManaCostInfo[manaLevel];
        GameManager.Instance.AddGold(manaCost);

        Debug.Log($"CurrentManaLevel: {CurrentManaLevel}, InManaLevel: {manaLevel}");
        if ((int)CurrentManaLevel > (int)manaLevel)
        {
            return;
        }
        else if ((int)CurrentManaLevel == (int)manaLevel)
        {
            // TODO : 타이머 갱신
            return;
        }
        
        spriteRendererBuilding.sprite = spriteBuilding[(int)manaLevel + 1];
        
        // TODO: 이펙트 적용
        
        foreach (var (x, y) in NineDirections)
        {
            var dx = (int)Coordinate.x + x;
            var dy = (int)Coordinate.y + y;
            if (dx < 0 || dx >= MaxIndex || dy < 0 || dy >= MaxIndex)
                continue;
                    
            TileNodeSystem.TileNodeGrid[dy, dx].ApplyTemperature(temperature, timer, () =>
            {
                CurrentManaLevel = EManaLevel.None;
                spriteRendererBuilding.sprite = spriteBuilding[0];
            });
        }
    }
}
