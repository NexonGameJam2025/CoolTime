using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public enum EManaLevel
{
    One, Two, Three
}

public enum ETileState
{
    Melt, Safe, Danger, Destroy
}

public class TileNode : MonoBehaviour
{
    private float _temperature;
    private bool _isDestroy = false;
    private bool _onMana = false;
    private bool _onBuilding = false;
    private float _elapsedTime = 0f;
    private int _manaLevel = 1;
    private bool _onWall = false;
    private int _wallCount = 0;
    public bool OnBuilding => _onBuilding;
    
    private Action<EManaLevel> _onBuildingCollisionAction;
    private Mana _currentMana;

    [Header("Information")]
    [SerializeField] private Vector2 _coordinate;
    [SerializeField] private ETileState _tileState;

    public ETileState TileState
    {
        get => _tileState;
        set
        {
            if (_tileState == value) return;
            _tileState = value;
            UpdateSprite();
        }
    }
    public Vector2 Coordinate => _coordinate;

    [Header("Settings")]
    [SerializeField] private float _destroyDelay = 5f;

    [Header("Effect")]
    [SerializeField] private float _shakeStrength = 0.08f;
    [SerializeField] private int _shakeFrequency = 30;

    [Header("Image")]
    [SerializeField] private Sprite _meltSprite;
    [SerializeField] private Sprite _safeSprite;
    [SerializeField] private Sprite _dangerSprite;
    [SerializeField] private Sprite _destroySprite;

    [Header("Wall")]
    [SerializeField] private Wall _upWall;
    [SerializeField] private Wall _downWall;
    [SerializeField] private Wall _rightWall;
    [SerializeField] private Wall _leftWall;
    
    [Header("Coefficient")]
    [SerializeField] float _twoCoefficient = 0.0004f;
    [SerializeField] float _oneCoefficient = 0.03f;

    private Coroutine _co_timer;
    private Coroutine _co_destroyTimer;
    private int _currentAppliedTemperature = 0;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateTemperature(true);
        
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

    public void SetMana(Mana mana)
    {
        _onMana = true;
        _currentMana = mana;
    }

    private void Update()
    {
        UpdateTemperature();
    }

    private void UpdateTemperature(bool isInitial = false)
    {
        if (_isDestroy) return;

        if (!isInitial)
        {
            _elapsedTime += Time.deltaTime;
        }

        float t = _elapsedTime;
        _temperature = 35f + (_twoCoefficient * t * t) + (_oneCoefficient * t);

        UpdateStateByTemperature();
    }

    private void UpdateSprite()
    {
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) return;
        switch (_tileState)
        {
            case ETileState.Melt:
                _spriteRenderer.sprite = _meltSprite;
                break;
            case ETileState.Safe:
                _spriteRenderer.sprite = _safeSprite;
                break;
            case ETileState.Danger:
                _spriteRenderer.sprite = _dangerSprite;
                break;
            case ETileState.Destroy:
                _spriteRenderer.sprite = _destroySprite;
                break;
            default:
                _spriteRenderer.sprite = null;
                break;
        }
    }


    private void UpdateStateByTemperature()
    {
        if (_tileState == ETileState.Destroy) return;

        if (_temperature > 50)
        {
            TileState = ETileState.Danger;
            if (_co_destroyTimer == null)
            {
                _co_destroyTimer = StartCoroutine(CO_DestroyTile());
            }
        }
        else if (_temperature > 40)
        {
            if (_co_destroyTimer != null)
            {
                StopCoroutine(_co_destroyTimer);
                _co_destroyTimer = null;
                transform.DOKill();
            }
            TileState = ETileState.Danger;
        }
        else
        {
            if (_tileState == ETileState.Melt) return;

            if (_co_destroyTimer != null)
            {
                StopCoroutine(_co_destroyTimer);
                _co_destroyTimer = null;
                transform.DOKill();
            }
            TileState = ETileState.Safe;
        }
    }

    private IEnumerator CO_DestroyTile()
    {
        transform.DOShakePosition(
            duration: _destroyDelay,
            strength: new Vector3(_shakeStrength, 0, 0),
            vibrato: _shakeFrequency,
            randomness: 0,
            snapping: false,
            fadeOut: false
        );

        yield return new WaitForSeconds(_destroyDelay);

        if (_temperature > 50)
        {
            TileState = ETileState.Destroy;
            _isDestroy = true;
        }
        _co_destroyTimer = null;
    }
    
    private void OnDestroy()
    {
        if (_co_timer != null)
        {
            StopCoroutine(_co_timer);
            _co_timer = null;
            _currentAppliedTemperature = 0;
        }
        transform.DOKill();
    }

    private void OnDisable()
    {
        if (_co_timer != null)
        {
            StopCoroutine(_co_timer);
            _co_timer = null;
            _currentAppliedTemperature = 0;
        }
        transform.DOKill();
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
            UpdateStateByTemperature();
        }
        else
        {
            if (_co_timer != null)
            {
                StopCoroutine(_co_timer);
                _temperature += _currentAppliedTemperature;
            }
            _co_timer = StartCoroutine(CO_ApplyTemperature(temperature, timer));
        }
    }

    private IEnumerator CO_ApplyTemperature(int temperature, int timer, Action doneCallback = null)
    {
        if (_co_timer != null)
        {
            StopCoroutine(_co_timer);
            _temperature += _currentAppliedTemperature;
        }
        
        _temperature -= temperature;
        _currentAppliedTemperature = temperature;
        UpdateStateByTemperature();

        if (timer > 0)
        {
            yield return new WaitForSeconds(timer);
        }

        _temperature += temperature;
        _currentAppliedTemperature = 0;
        UpdateStateByTemperature();

        _co_timer = null;
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
        UpdateSprite();
    }
#endif
}