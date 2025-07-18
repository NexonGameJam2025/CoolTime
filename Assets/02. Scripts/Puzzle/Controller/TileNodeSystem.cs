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
        if(inputKey == InputKey.Right)
        {
            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    if(TileNodeGrid[x, y].OnMana)
                    {
                        Search(inputKey, TileNodeGrid[x, y]);
                        break;
                    }
                }
            }
        }

        if (inputKey == InputKey.Left)
        {
            for (int y = 0; y < 7; y++)
            {
                for (int x = 7; x < 0; x--)
                {
                    if (TileNodeGrid[x, y].OnMana)
                    {
                        Search(inputKey, TileNodeGrid[x, y]);
                        break;
                    }
                }
            }
        }

        if (inputKey == InputKey.Up)
        {
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    if (TileNodeGrid[x, y].OnMana)
                    {
                        Search(inputKey, TileNodeGrid[x, y]);
                        break;
                    }
                }
            }
        }

        if (inputKey == InputKey.Down)
        {
            for (int x = 0; x < 7; x++)
            {
                for (int y = 7; y < 0; y++)
                {
                    if (TileNodeGrid[x, y].OnMana)
                    {
                        Search(inputKey, TileNodeGrid[x, y]);
                        break;
                    }
                }
            }
        }
    }

    private void Search(InputKey inputKey, TileNode tileNode) // tileNode는 마나를 가지고 있음
    {
        if (inputKey == InputKey.Right)
        {
            if (tileNode.OnMana)
            {
                for (int x = (int)tileNode.Coordinate.x; x < 7; x++)
                {
                    if (_tileNodeGrid[x, (int)tileNode.Coordinate.y].OnMana
                        && tileNode.CurrentMana.ManaLevel == _tileNodeGrid[x, (int)tileNode.Coordinate.y].CurrentMana.ManaLevel)
                    {
                        // 마나가 있고, 또 만난 마나가 레벨이 같음
                        // 1 1 1 1
                        // 2 2 -> 첫상태 기준
                        // 1 1 1
                        // 3 3 -> 3 3
                        // 2 1 or 1 2
                    }
                    else if (_tileNodeGrid[x, (int)tileNode.Coordinate.y].OnMana)
                    {
                        // 마나가 있고, 또 만난 마나가 레벨이 다름
                    }
                }
            }
            for (int x = 6; x > (int)tileNode.Coordinate.x; x--)

            {
            }
        }

        if (inputKey == InputKey.Left)
        {

        }

        if (inputKey == InputKey.Up)
        {

        }

        if (inputKey == InputKey.Down)
        {

        }
    }

    private void TransferMana()
    {

    }
}