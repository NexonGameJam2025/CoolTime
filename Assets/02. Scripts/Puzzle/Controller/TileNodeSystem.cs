using UnityEngine;

public class TileNodeSystem : MonoBehaviour
{
    [SerializeField] private TileNode[] _tileNodes;
    private TileNode[,] _tileNodeGrid = new TileNode[7, 7];
    public TileNode[,] TileNodeGrid => _tileNodeGrid;

    private void Awake()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                _tileNodeGrid[i, j] = _tileNodes[7*i + j];
            }
        }
    }

    public void Action(InputKey inputKey)
    {
        foreach (var tileNode in _tileNodes)
        {
            Search(inputKey, tileNode.Coordinate);
        }
    }

    private void Search(InputKey inputKey, Vector2 position)
    {
        
    }
}