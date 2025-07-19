using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class FrozenBeamLauncher : Building
{
    [SerializeField] private SpriteRenderer spriteRendererBuilding;
    [SerializeField] private Sprite[] spriteBuilding;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private GameObject cannonEffectPrefab;
    
    private List<(int, int)> VerticalDirections = new()
    {
        (0, 1), (0, -1)
    };
    private List<(int, int)> HorizontalDirections = new()
    {
        (1, 0), (-1, 0)
    };
    
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
                
                switch (CurrentManaLevel)
                {
                    case EManaLevel.One:
                        OnCannonEffect(tileNode.transform.position, ECannonEffectType.Single);
                        tileNode.ApplyTemperature(temperature, 0);
                        break;
                
                    case EManaLevel.Two:
                        tileNode.ApplyTemperature(temperature, 0);
                        var isVertical = Random.value < 0.5f;
                        
                        if (isVertical)
                        {
                            OnCannonEffect(tileNode.transform.position, ECannonEffectType.Vertical);
                            foreach (var (ix, iy) in VerticalDirections)
                            {
                                var dx = (int)tileNode.Coordinate.x + ix;
                                var dy = (int)tileNode.Coordinate.y + iy;
                                TileNodeSystem.TileNodeGrid[dy, dx].ApplyTemperature(temperature, 0);
                            }
                        }
                        else
                        {
                            OnCannonEffect(tileNode.transform.position, ECannonEffectType.Horizontal);
                            foreach (var (ix, iy) in HorizontalDirections)
                            {
                                var dx = (int)tileNode.Coordinate.x + ix;
                                var dy = (int)tileNode.Coordinate.y + iy;
                                TileNodeSystem.TileNodeGrid[dy, dx].ApplyTemperature(temperature, 0);
                            }
                        }
                        break;
                
                    case EManaLevel.Three:
                        OnCannonEffect(tileNode.transform.position, ECannonEffectType.Cross);
                        foreach (var (ix, iy) in FiveDirections)
                        {
                            var dx2 = (int)tileNode.Coordinate.x + ix;
                            var dy2 = (int)tileNode.Coordinate.y + iy;
                            TileNodeSystem.TileNodeGrid[dy2, dx2].ApplyTemperature(temperature, 0);
                        }
                        break;
                    
                    default:
                        Debug.Log($"CurrentManaLevel is not valid: {CurrentManaLevel}");
                        break;
                }
                
                CurrentManaLevel = EManaLevel.None;

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

    private void OnCannonEffect(Vector3 position, ECannonEffectType type)
    {
        var cannonEffect = Instantiate(cannonEffectPrefab, position, Quaternion.identity);
        var cannonEffectScript = cannonEffect.GetComponent<CannonEffect>();
        cannonEffectScript.OnEffect(type);
    }
}
