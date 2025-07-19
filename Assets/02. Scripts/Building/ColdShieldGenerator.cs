using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ColdShieldGenerator : Building
{
    [SerializeField] private SpriteRenderer spriteRendererBuilding;
    [SerializeField] private Sprite[] spriteBuilding;
    [SerializeField] private Animator animator;
    
    private bool _isActivated = false;
    private List<Tween> _currentTweens = new List<Tween>();
    
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
        GameManager.Instance.ShieldCount++;
        
        base.OnFinishBuild();
    }

    public override void OnDeActivate()
    {
        base.OnDeActivate();
        
        spriteRendererBuilding.sprite = spriteBuilding[0];
    }

    public override void OnCollisionMana(EManaLevel manaLevel)
    {
        base.OnCollisionMana(manaLevel);
        
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
            UpdateTimer(temperature, timer);
            return;
        }

        if (_isActivated)
        {
            RemoveOriginPower();
        }

        _isActivated = true;
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
                    
            var tween = TileNodeSystem.TileNodeGrid[dy, dx].ApplyTemperature(temperature, timer, () =>
            {
                CurrentManaLevel = EManaLevel.None;
                spriteRendererBuilding.sprite = spriteBuilding[0];
                animator.gameObject.SetActive(false);
                _isActivated = false;
            });
            _currentTweens.Add(tween);
        }
    }

    private void RemoveOriginPower()
    {
        _currentTweens.Clear();
        var temperature = TemperatureInfo[CurrentManaLevel];
        foreach (var (x, y) in NineDirections)
        {
            var dx = (int)Coordinate.x + x;
            var dy = (int)Coordinate.y + y;
            if (dx < 0 || dx >= MaxIndex || dy < 0 || dy >= MaxIndex)
                continue;
                    
            TileNodeSystem.TileNodeGrid[dy, dx].ApplyTemperature(-temperature);
        }
    }

    private void UpdateTimer(int temperature, int timer)
    {
        foreach (var tween in _currentTweens)
        {
            tween.Kill();
        }
        _currentTweens.Clear();
        
        foreach (var (x, y) in NineDirections)
        {
            var dx = (int)Coordinate.x + x;
            var dy = (int)Coordinate.y + y;
            if (dx < 0 || dx >= MaxIndex || dy < 0 || dy >= MaxIndex)
                continue;
                    
            var tween = TileNodeSystem.TileNodeGrid[dy, dx].ApplyTemperature(0, timer, () =>
            {
                TileNodeSystem.TileNodeGrid[dy, dx].TemperatureDecreaseByBuilding += temperature;
                CurrentManaLevel = EManaLevel.None;
                spriteRendererBuilding.sprite = spriteBuilding[0];
                animator.gameObject.SetActive(false);
                _isActivated = false;
            });
            _currentTweens.Add(tween);
        }
    }
}
