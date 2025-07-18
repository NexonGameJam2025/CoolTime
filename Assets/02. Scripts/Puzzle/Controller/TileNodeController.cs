using UnityEngine;

public class TileNodeController : MonoBehaviour
{
    [SerializeField] private TileNode[] tileNodes;
    private TileNode[,] tileNodeGrid = new TileNode[7, 7];

    private void Awake()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                tileNodeGrid[i, j] = tileNodes[7*i + j];
            }
        }
    }
}
