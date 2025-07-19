using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ColdShieldGenerator : Building
{
    [SerializeField] private SpriteRenderer spriteRendererBuilding;
    [SerializeField] private Sprite[] spriteBuilding;
    [SerializeField] private Animator animator;
    
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
    
    public override void OnBuilderStart(Vector2 coordinate)
    {
        base.OnBuilderStart(coordinate);
        
        var color = spriteRendererBuilding.color;
        color.a = 0f;
        spriteRendererBuilding.color = color;
    }
    
    public override void OnStartBuild()
    {
        base.OnStartBuild();
        
        var color = spriteRendererBuilding.color;
        color.a = 1f;
        spriteRendererBuilding.color = color;
        
        OnStartBuildSpriteTween = DOTween.Sequence()
            .AppendCallback(() => spriteRendererBuilding.sprite = spriteOnStartBuilding[0])
            .AppendInterval(CONSTRUCT_ANIM_INTERVAL)
            .AppendCallback(() => spriteRendererBuilding.sprite = spriteOnStartBuilding[1])
            .AppendInterval(CONSTRUCT_ANIM_INTERVAL)
            .SetLoops(-1);

        OnStartBuildSpriteTween.SetLink(gameObject);
        OnStartBuildSpriteTween.Play();
    }
    
    public override void OnFinishBuild()
    {
        OnStartBuildSpriteTween.Kill();
        spriteRendererBuilding.sprite = spriteBuilding[0];
        
        base.OnFinishBuild();
    }

    public override void OnDeActivate()
    {
        base.OnDeActivate();
        
        spriteRendererBuilding.sprite = spriteBuilding[0];
    }

    public override void OnCollisionMana(EManaLevel manaLevel)
    {
        var timer = TimerInfo[manaLevel];
        var temperature = TemperatureInfo[manaLevel];
        var manaCost = ManaCostInfo[manaLevel];
        GameManager.Instance.AddGold(manaCost);

        if ((int)CurrentManaLevel > (int)manaLevel)
        {
            return;
        }
        else if ((int)CurrentManaLevel == (int)manaLevel)
        {
            // TODO : 타이머 갱신
            return;
        }
        
        CurrentManaLevel = manaLevel;
        spriteRendererBuilding.sprite = spriteBuilding[(int)manaLevel + 1];
        
        animator.gameObject.SetActive(false);
        animator.gameObject.SetActive(true);
        
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
                animator.gameObject.SetActive(false);
            });
        }
    }
}
