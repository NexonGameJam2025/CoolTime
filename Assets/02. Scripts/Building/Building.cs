using System.Collections.Generic;
using Core.Scripts;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteBuilding;
    [SerializeField] private int cost;
    [SerializeField] private float previewImageOpacity = 0.5f;
    [SerializeField] private Define.EBuildingType buildingType = Define.EBuildingType.None;
    
    public Define.EBuildingType BuildingType => buildingType;
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
    }
    
    public void TogglePreviewImage(bool isOn)
    {
        var opacity = isOn ? previewImageOpacity : 1f;
        var color = spriteBuilding.color;
        color.a = opacity;
        spriteBuilding.color = color;
    }
    
    public abstract void OnCollision(EManaLevel manaLevel);
}