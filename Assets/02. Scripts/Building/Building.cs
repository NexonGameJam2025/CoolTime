using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Building : MonoBehaviour
{
    [SerializeField] private Image imageBuilding;
    [SerializeField] private int cost;
    [SerializeField] private float previewImageOpacity = 0.5f;

    public Action<EManaLevel> OnCollisionAction;
    protected TileNodeSystem _tileNodeSystem;
    
    protected readonly List<(int, int)> FourDirections = new()
    {
        (0, 0), (0, 1), (1, 0), (0, -1), (-1, 0)
    };
    
    protected readonly List<(int, int)> NineDirections = new()
    {
        (0, 0), (0, 1), (1, 0), (0, -1), (-1, 0), (1, 1), (1, -1), (-1, 1), (-1, -1)
    };

    protected void Start()
    {
        _tileNodeSystem = FindObjectOfType<TileNodeSystem>();
        OnCollisionAction += OnCollision;
    }
    
    public void SetPreviewImage()
    {
        var color = imageBuilding.color;
        color.a = previewImageOpacity;
        imageBuilding.color = color;
    }
    
    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    protected abstract void OnCollision(EManaLevel manaLevel);
}
