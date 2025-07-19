using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ManaSystem : MonoBehaviour
{
    [SerializeField] private GameObject _manaGO;
    [SerializeField] private TileNodeSystem _nodeSystem;

    private void Start()
    {
        CreateManaOnRandomSafeTiles(6);
    }

    public void CreateManaOnRandomSafeTiles(int count)
    {
        if (_nodeSystem == null || _manaGO == null)
        {
            Debug.LogWarning("NodeSystem 또는 Mana Prefab이 설정되지 않았습니다.");
            return;
        }

        List<TileNode> safeTiles = new List<TileNode>();
        foreach (TileNode tile in _nodeSystem.TileNodeGrid)
        {
            if (tile.TileState == ETileState.Safe)
            {
                safeTiles.Add(tile);
            }
        }

        List<TileNode> shuffledTiles = safeTiles.OrderBy(x => System.Guid.NewGuid()).ToList();

        int spawnCount = Mathf.Min(count, shuffledTiles.Count);
        for (int i = 0; i < spawnCount; i++)
        {
            TileNode randomTile = shuffledTiles[i];
            
            randomTile.SetMana();
        }
    }
}