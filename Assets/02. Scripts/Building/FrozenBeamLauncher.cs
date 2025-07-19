using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class FrozenBeamLauncher : Building
{
    [SerializeField] private SpriteRenderer spriteRendererBuilding;
    [SerializeField] private Sprite[] spriteBuilding;
    [SerializeField] BoxCollider2D boxCollider2D;
    
    public bool IsOnMana { get; private set; } = false;

    private bool _isClicked = false;
    private TileNode _lastHoveredNode;
    private Action _afterSelectHandler;
    
    protected readonly Dictionary<EManaLevel, int> TemperatureInfo = new()
    {
        { EManaLevel.One, 15 },
        { EManaLevel.Two, 20 },
        { EManaLevel.Three, 30 }
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
        color.a = 1.0f;
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
        base.OnFinishBuild();
        
        OnStartBuildSpriteTween.Kill();
        spriteRendererBuilding.sprite = spriteBuilding[0];
        boxCollider2D.enabled = true;
    }
    
    public override void OnDeActivate()
    {
        base.OnDeActivate();
        
        spriteRendererBuilding.sprite = spriteBuilding[0];
    }

    private void Update()
    {
        if (!_isClicked || !IsInit || !IsOnMana)
            return;
        
        var mousePos = Mouse.current.position.ReadValue();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        var hit = Physics2D.OverlapPoint(mousePos);
        
        TileNode currentNode = null;
        
        if (hit && hit.TryGetComponent<TileNode>(out var node))
        {
            currentNode = node;
        }
        
        if (_lastHoveredNode != currentNode)
        {
            if (_lastHoveredNode)
            {
                _lastHoveredNode.ToggleSortingLayerUp(false);
            }
        
            if (currentNode)
            {
                currentNode.ToggleSortingLayerUp(true);
            }
        }
        _lastHoveredNode = currentNode;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (hit.TryGetComponent<TileNode>(out var tileNode))
            {
                tileNode.ToggleSortingLayerUp(false);
                var temperature = TemperatureInfo[CurrentManaLevel];
                
                _isClicked = false;
                IsOnMana = false;
                spriteRendererBuilding.sprite = spriteBuilding[0];
                CurrentManaLevel = EManaLevel.None;
                
                switch (CurrentManaLevel)
                {
                    case EManaLevel.One:
                        tileNode.ApplyTemperature(temperature, 0);
                        break;
                
                    case EManaLevel.Two:
                        var randomIndex = UnityEngine.Random.Range(1, FiveDirections.Count);
                        var (x, y) = FiveDirections[randomIndex];
                        tileNode.ApplyTemperature(temperature, 0);
                        var dx = (int)Coordinate.x + x;
                        var dy = (int)Coordinate.y + y;
                        TileNodeSystem.TileNodeGrid[dy, dx].ApplyTemperature(temperature, 0);
                        break;
                
                    case EManaLevel.Three:
                        foreach (var (ix, iy) in FiveDirections)
                        {
                            var dx2 = (int)Coordinate.x + ix;
                            var dy2 = (int)Coordinate.y + iy;
                            TileNodeSystem.TileNodeGrid[dy2, dx2].ApplyTemperature(temperature, 0);
                        }
                        break;
                }
                
                _afterSelectHandler?.Invoke();
            }
        }
    }
    
    public override void OnCollisionMana(EManaLevel manaLevel)
    {
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
        
        IsOnMana = true;
        CurrentManaLevel = manaLevel;
        spriteRendererBuilding.sprite = spriteBuilding[(int)manaLevel + 1];
    }

    public void OnClickHandler(Action doneCallback = null)
    {
        _isClicked = true;
        _afterSelectHandler = doneCallback;
    }
}
