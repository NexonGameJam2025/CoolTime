using System.Runtime.CompilerServices;
using UnityEngine;

public enum InputKey
{
    Up, Down, Left, Right
}

public class InputController : MonoBehaviour
{
    [SerializeField] private TileNodeSystem _tileNodeSystem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _tileNodeSystem.Action(InputKey.Up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _tileNodeSystem.Action(InputKey.Down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _tileNodeSystem.Action(InputKey.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _tileNodeSystem.Action(InputKey.Right);
        }
    }
}