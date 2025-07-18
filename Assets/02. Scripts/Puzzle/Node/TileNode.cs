using System;
using UnityEngine;

public enum EManaLevel
{
    One, Two, Three
}

public class TileNode : MonoBehaviour
{
    
    private float _temperature;
    private bool _isDestroy = false;
    private bool _onMana = false;
    private bool _onBuilding = false;
    private int _manaLevel = 1;

    [Header("Information")]
    [SerializeField] private Vector2 _coordinate;

    [Header("Wall")]
    [SerializeField] private Wall _upWall;
    [SerializeField] private Wall _downWall;
    [SerializeField] private Wall _rightWall;
    [SerializeField] private Wall _leftWall;

#if UNITY_EDITOR
    private void OnValidate()
    {
        try
        {
            string name = gameObject.name;
            int startIndex = name.IndexOf('(');
            int endIndex = name.IndexOf(')');

            if (startIndex == -1 || endIndex == -1) return;

            string coordinateString = name.Substring(startIndex + 1, endIndex - startIndex - 1);

            string[] parts = coordinateString.Split(',');

            if (parts.Length != 2) return;

            int x = int.Parse(parts[0].Trim());
            int y = int.Parse(parts[1].Trim());

            _coordinate = new Vector2(x, y);
        }
        catch (Exception)
        {
        }
    }
#endif
}