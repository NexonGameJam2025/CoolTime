using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ManaSystem : MonoBehaviour
{
    [SerializeField] private GameObject _manaGO;
    [SerializeField] private TileNodeSystem _nodeSystem;

    private float _manaGenerationTimer;

    private void Awake()
    {
        SoundManager.Instance.PlayBGMWithIntro("GamePlay_Intro", "GamePlay_Loop", 1f);
    }

    private void Start()
    {
        CreateManaOnRandomSafeTiles(6);
        ResetManaGenerationTimer();
    }

    private void Update()
    {
        _manaGenerationTimer -= Time.deltaTime;
        if (_manaGenerationTimer <= 0)
        {
            CreateManaOnRandomSafeTile();
            ResetManaGenerationTimer();
        }
    }

    private void ResetManaGenerationTimer()
    {
        float T = GameManager.Instance.Temperature;
        float interval = Mathf.Min(3.0f, Mathf.Max(1.25f, 1.0f + 0.05f * (T - 50f)));
        _manaGenerationTimer = interval;
    }

    private void CreateManaOnRandomSafeTile()
    {
        if (_nodeSystem == null || _manaGO == null) return;

        List<TileNode> availableTiles = new List<TileNode>();
        foreach (TileNode tile in _nodeSystem.TileNodeGrid)
        {
            if (tile.TileState == ETileState.Safe && !tile.OnMana && !tile.HasBuilding)
            {
                availableTiles.Add(tile);
            }
        }

        if (availableTiles.Count > 0)
        {
            TileNode randomTile = availableTiles[Random.Range(0, availableTiles.Count)];
            randomTile.SetMana();
        }
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
            if (tile.TileState == ETileState.Safe && !tile.OnMana && !tile.HasBuilding)
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