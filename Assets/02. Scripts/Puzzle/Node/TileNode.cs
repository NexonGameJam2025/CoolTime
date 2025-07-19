using System;
using System.Collections;
using Core.Scripts.Helper;
using UnityEngine;
using DG.Tweening;

public enum EManaLevel
{
    One = 0,
    Two = 1,
    Three = 2,
    None = -1,
}

public enum ETileState
{
    Safe, Danger, Destroy
}

public class TileNode : MonoBehaviour
{
    private float _temperature;
    private float _temperatureDecreaseByBuilding = 0f;

    private bool _isDestroy = false;
    private bool _onMana = false;
    public bool OnMana => _onMana;
    private bool _onBuilding = false;
    private bool _onWall = false;
    private int _wallCount = 0;
    public bool OnBuilding => _onBuilding;

    private Action<EManaLevel> _onBuildingCollisionAction;
    private Building _currentBuilding;
    public bool IsCurrentConstructing => _currentBuilding?.IsConstructing ?? false;
    public Building CurrentBuilding => _currentBuilding;
    public bool HasBuilding => _currentBuilding != null;
    private Mana _currentMana;
    public Mana CurrentMana => _currentMana;

    [Header("Information")]
    [SerializeField] private Vector2 _coordinate;
    [SerializeField] private ETileState _tileState;
    [SerializeField] private GameObject _manaPrefab;

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
    [SerializeField] private float _dangerDelay = 3f;

    [Header("Effect")]
    [SerializeField] private float _shakeStrength = 0.08f;
    [SerializeField] private int _shakeFrequency = 30;

    [Header("Image")]
    [SerializeField] private Sprite _safeSprite;
    [SerializeField] private Sprite _dangerSprite;
    [SerializeField] private Sprite _destroySprite;

    [Header("Wall")]
    [SerializeField] private Wall _upWall;
    [SerializeField] private Wall _downWall;
    [SerializeField] private Wall _rightWall;
    [SerializeField] private Wall _leftWall;

    private bool _isUpwallEnable = false;
    private bool _isDownWallEnable = false;
    private bool _isLeftWallEnable = false;
    private bool _isRightWallEnable = false;

    public bool IsUpWallEnable => _upWall?.IsEnable ?? false;
    public bool IsDownWallEnable => _downWall?.IsEnable ?? false;
    public bool IsLeftWallEnable => _leftWall?.IsEnable ?? false;
    public bool IsRightWallEnable => _rightWall?.IsEnable ?? false;

    [Header("Coefficient")]
    [SerializeField] float _twoCoefficient = 0.0004f;
    [SerializeField] float _oneCoefficient = 0.03f;

    private Coroutine _co_timer;
    private Coroutine _co_dangerTimer;
    private Coroutine _co_destroyTimer;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_upWall)
        {
            _upWall.OnActivateWall += OnActivateWallAction;
            _upWall.OnDeactivateWall += OnDeactivateWallAction;
        }
        if (_downWall)
        {
            _downWall.OnActivateWall += OnActivateWallAction;
            _downWall.OnDeactivateWall += OnDeactivateWallAction;
        }
        if (_rightWall)
        {
            _rightWall.OnActivateWall += OnActivateWallAction;
            _rightWall.OnDeactivateWall += OnDeactivateWallAction;
        }
        if (_leftWall)
        {
            _leftWall.OnActivateWall += OnActivateWallAction;
            _leftWall.OnDeactivateWall += OnDeactivateWallAction;
        }
    }
    private void FixedUpdate()
    {
        _isUpwallEnable = IsUpWallEnable;
        _isDownWallEnable = IsDownWallEnable;
        _isLeftWallEnable = IsLeftWallEnable;
        _isRightWallEnable = IsRightWallEnable;
    }
    public void ReceiveMana(Mana manaComponent)
    {
        if (_onMana && _currentMana != null)
        {
            Destroy(_currentMana.gameObject);
        }
        _currentMana = manaComponent;
        _onMana = true;
    }

    public void ClearMana()
    {
        _currentMana = null;
        _onMana = false;
    }

    public void SetMana(int level = 1)
    {
        if (_manaPrefab == null)
        {
            Debug.LogError("Mana Prefab�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }
        if (CurrentMana != null)
        {
            Destroy(CurrentMana.gameObject);
        }

        GameObject manaObject = Instantiate(_manaPrefab, this.transform.position, Quaternion.identity);

        _currentMana = manaObject.GetComponent<Mana>();
        _currentMana.SetLevel(level);

        if (_currentMana != null)
        {
            _onMana = true;
        }
        else
        {
            Debug.LogError("������ ���� �����տ� 'Mana' ��ũ��Ʈ�� �����ϴ�!");
        }
    }

    public void UnsetMana()
    {
        if (_currentMana != null)
        {
            Destroy(_currentMana.gameObject);
        }
        _currentMana = null;
        _onMana = false;
    }

    private void Update()
    {
        UpdateTemperature();
    }

    private void UpdateTemperature()
    {
        if (_isDestroy) return;

        float t = GameManager.Instance.ElapsedTime;
        _temperature = 35f + (_twoCoefficient * t * t) + (_oneCoefficient * t) + _temperatureDecreaseByBuilding;


        UpdateStateByTemperature();
    }

    private void UpdateSprite()
    {
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) return;
        switch (_tileState)
        {
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
            if (_co_dangerTimer != null)
            {
                StopCoroutine(_co_dangerTimer);
                _co_dangerTimer = null;
            }
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

            if (_tileState != ETileState.Danger && _co_dangerTimer == null)
            {
                _co_dangerTimer = StartCoroutine(CO_EnterDangerState());
            }
        }
        else
        {
            if (_co_destroyTimer != null)
            {
                StopCoroutine(_co_destroyTimer);
                _co_destroyTimer = null;
                transform.DOKill();
            }
            if (_co_dangerTimer != null)
            {
                StopCoroutine(_co_dangerTimer);
                _co_dangerTimer = null;
                transform.DOKill();
            }
            TileState = ETileState.Safe;
        }
    }
    private IEnumerator CO_EnterDangerState()
    {
        transform.DOShakePosition(
            duration: _dangerDelay,
            strength: new Vector3(_shakeStrength, 0, 0),
            vibrato: _shakeFrequency,
            randomness: 0,
            snapping: false,
            fadeOut: false
        );

        yield return new WaitForSeconds(_dangerDelay);

        if (_temperature > 40)
        {
            TileState = ETileState.Danger;
        }
        _co_dangerTimer = null;
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
            _currentBuilding.OnDeActivate();
            _isDestroy = true;
        }
        _co_destroyTimer = null;
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    private void OnActivateWallAction()
    {
        _wallCount++;
        _onWall = true;
    }

    private void OnDeactivateWallAction()
    {
        _wallCount--;
        if (_wallCount <= 0)
        {
            _onWall = false;
            _wallCount = 0;
        }
    }

    public void ToggleSortingLayerUp(bool isUp)
    {
        _spriteRenderer.sortingLayerName = isUp ? "Highlight" : "Default";
        if (_currentBuilding)
        {
            _currentBuilding.SetSortingLayer(isUp ? "Highlight" : "Building");
        }
    }

    public void SetBuilding(Building building)
    {
        _onBuildingCollisionAction += building.OnCollisionMana;
        _currentBuilding = building;
        _onBuilding = true;
    }

    public void ApplyTemperature(int temperature, int timer = 0, Action doneCallback = null)
    {
        _temperatureDecreaseByBuilding -= temperature;

        if (timer == 0)
            return;

        DOVirtual.DelayedCall(timer, () =>
        {
            _temperatureDecreaseByBuilding += temperature;
            doneCallback?.Invoke();
        });
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