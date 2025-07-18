using System;
using System.Collections;
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
    private bool _onWall = false;
    private int _wallCount = 0;
    public bool OnBuilding => _onBuilding;
    
    private Action<EManaLevel> _onBuildingCollisionAction;

    [Header("Information")]
    [SerializeField] private Vector2 _coordinate;

    [Header("Wall")]
    [SerializeField] private Wall _upWall;
    [SerializeField] private Wall _downWall;
    [SerializeField] private Wall _rightWall;
    [SerializeField] private Wall _leftWall;
    
    private Coroutine _coTimer;
    private int _currentAppliedTemperature = 0;

    private void Awake()
    {
        if (_upWall)
        {
            _upWall.OnEnableWall += OnEnableWallAction;
            _upWall.OnDisableWall += OnDisableWallAction;
        }
        if (_downWall)
        {
            _downWall.OnEnableWall += OnEnableWallAction;
            _downWall.OnDisableWall += OnDisableWallAction;
        }
        if (_rightWall)
        {
            _rightWall.OnEnableWall += OnEnableWallAction;
            _rightWall.OnDisableWall += OnDisableWallAction;
        }
        if (_leftWall)
        {
            _leftWall.OnEnableWall += OnEnableWallAction;
            _leftWall.OnDisableWall += OnDisableWallAction;
        }
    }

    private void OnDestroy()
    {
        if (_coTimer != null)
        {
            StopCoroutine(_coTimer);
            _coTimer = null;
            _currentAppliedTemperature = 0;
        }
    }

    private void OnDisable()
    {
        if (_coTimer != null)
        {
            StopCoroutine(_coTimer);
            _coTimer = null;
            _currentAppliedTemperature = 0;
        }
    }

    private void OnEnableWallAction()
    {
        _wallCount++;
        _onWall = true;
    }
    
    private void OnDisableWallAction()
    {
        _wallCount--;
        if (_wallCount <= 0)
        {
            _onWall = false;
            _wallCount = 0;
        }
    }

    public void SetBuilding(Building building)
    {
        _onBuildingCollisionAction += building.OnCollision;
        _onBuilding = true;
    }

    public void ApplyTemperature(int temperature, int timer = 0, bool isPermanent = false)
    {
        if (isPermanent)
        {
            _temperature -= temperature;
        }
        else
        {
            _coTimer = StartCoroutine(CO_ApplyTemperature(temperature, timer));
        }
    }

    private IEnumerator CO_ApplyTemperature(int temperature, int timer, Action doneCallback = null)
    {
        if (_coTimer != null)
        {
            StopCoroutine(_coTimer);
            _temperature += _currentAppliedTemperature;
        }
        
        _temperature -= temperature;
        _currentAppliedTemperature = temperature;
        
        if (timer > 0)
        {
            yield return new WaitForSeconds(timer);
        }
        
        _temperature += temperature;
        
        doneCallback?.Invoke();
    }

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