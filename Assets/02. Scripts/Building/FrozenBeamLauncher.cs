using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FrozenBeamLauncher : Building
{
    [SerializeField] private SpriteRenderer spriteRendererBuilding;
    [SerializeField] private Sprite spriteBuilding;
    [SerializeField] private Sprite spriteOnStartBuilding;
    [SerializeField] BoxCollider2D boxCollider2D;
    
    public bool IsOnMana { get; private set; } = false;

    private bool _isClicked = false;
    private TileNode _lastHoveredNode;
    private Action _afterSelectHandler;
    
    protected readonly Dictionary<EManaLevel, int> TemperatureInfo = new()
    {
        { EManaLevel.One, 10 },
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
        boxCollider2D.enabled = true;
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
                var temperature = TemperatureInfo[_currentManaLevel];
                
                _isClicked = false;
                IsOnMana = false;
                
                switch (_currentManaLevel)
                {
                    case EManaLevel.One:
                        tileNode.ApplyTemperature(temperature, 0, true);
                        break;
                
                    case EManaLevel.Two:
                        var randomIndex = UnityEngine.Random.Range(1, FiveDirections.Count);
                        var (x, y) = FiveDirections[randomIndex];
                        tileNode.ApplyTemperature(temperature, 0, true);
                        var dx = (int)Coordinate.x + x;
                        var dy = (int)Coordinate.y + y;
                        TileNodeSystem.TileNodeGrid[dy, dx].ApplyTemperature(temperature, 0, true);
                        break;
                
                    case EManaLevel.Three:
                        foreach (var (ix, iy) in FiveDirections)
                        {
                            var dx2 = (int)Coordinate.x + ix;
                            var dy2 = (int)Coordinate.y + iy;
                            TileNodeSystem.TileNodeGrid[dy2, dx2].ApplyTemperature(temperature, 0, true);
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
        
        if (_currentManaLevel >= manaLevel)
            return;
        
        IsOnMana = true;
        _currentManaLevel = manaLevel;
        
        // TODO : 마나 레벨에 따른 스프라이트 변경
    }

    public void OnClickHandler(Action doneCallback = null)
    {
        _isClicked = true;
        _afterSelectHandler = doneCallback;
    }
}
