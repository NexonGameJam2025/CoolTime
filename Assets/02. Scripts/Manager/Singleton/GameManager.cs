using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts;
using Core.Scripts.Manager;
using TMPro;
using UnityEngine;

[System.Serializable]
public class CoolingEffectData
{
    public EManaLevel level;
    public float immediateCooling;
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private TextMeshProUGUI textGold;
    private bool _isCooling = false;
    public bool IsCooling => _isCooling;
    public bool IsCannonReady = false;

    private float _elapsedTime = 0f;
    private bool _isPaused = false;
    private float _temperature = 0f;
    public float Temperature => _temperature;
    private float _maxTemperature = 0;
    public float MaxTemperature => _maxTemperature;
    private bool _isGameEnd = false;
    
    [Header("Temperature Coefficient")]
    [SerializeField] private float _twoCoefficient = 0.00010556f;
    [SerializeField] private float _oneCoefficient = 0.02f;

    [Header("Cooling Effects by Mana Level")]
    [SerializeField] private List<CoolingEffectData> _coolingEffectDataList;

    public int Gold { get; private set; } = 100;
    public int WallCount { get; set; } = 0;
    public int ShieldCount { get; set; } = 0;
    public int CannonCount { get; set; } = 0;

    private Coroutine _coCountingEffect;
    
    [SerializeField] float _currentCoolingNumber = 0f;

    public struct IceCollectData
    {
        public int LevelOne;
        public int LevelTwo;
        public int LevelThree;
    }

    public int ClearTimeScore = 0;
    public int IceCollectScore = 0;
    public IceCollectData IceCollectInfo = new()
    {
        LevelOne = 0,
        LevelTwo = 0,
        LevelThree = 0
    };
    public int BestTemperatureScore = 0;
    public int StabilizeScore = 0;
    public int ConstructionScore = 0;
    public int DestructionScore = 0;


    public int IctCollector = 0;
    public bool PerfectBuildingDesigner = true;
    public struct MasterBuilderData
    {
        public int Wall;
        public int Shield;
        public int Cannon;
    }
    public MasterBuilderData MasterBuilder = new()
    {
        Wall = 0,
        Shield = 0,
        Cannon = 0
    };

    public bool Phycho = true;
    public int DestructionKing = 0;
    

    /// <summary>
    /// ���� ���� �� ��� �ð� (��)
    /// </summary>
    public float ElapsedTime => _elapsedTime;
    public bool IsPaused => _isPaused;

    protected void Awake()
    {
        base.Awake();
        textGold = GameObject.Find("Text_Currency").GetComponent<TextMeshProUGUI>();
        textGold.text = Gold.ToString();
        _temperature = 50f; // 초기 온도 설정
    }

    private void Update()
    {
        if (!_isPaused)
        {
            _elapsedTime += Time.deltaTime;

            _temperature = 50f + _twoCoefficient * _elapsedTime * _elapsedTime + _oneCoefficient * _elapsedTime - _currentCoolingNumber;
            _maxTemperature = Mathf.Max(_maxTemperature, _temperature);

            if (_temperature < 0.0f && !_isGameEnd)
            {
                _isGameEnd = true;
                EndGame();
            }
        }
    }

    public void ApplyCooling(EManaLevel level)
    {
        _isCooling = true;
        var effectData = _coolingEffectDataList.FirstOrDefault(e => e.level == level);

        if (effectData == null)
        {
            Debug.LogWarning($"Cooling effect for ManaLevel '{level}' not found.");
            return;
        }
        
        _currentCoolingNumber += effectData.immediateCooling;
    }

    public void PauseGame()
    {
        _isPaused = true;
    }

    public void ResumeGame()
    {
        _isPaused = false;
    }

    public bool IsCanBuyBuilding(int cost)
    {
        return Gold >= cost;
    }

    public void AddGold(int cost)
    {
        if (Gold - cost < 0)
        {
            Debug.LogWarning("Not enough gold to add.");
            return;
        }

        var originGold = Gold;
        var targetGold = Gold + cost;

        if (_coCountingEffect != null)
        {
            StopCoroutine(_coCountingEffect);
            _coCountingEffect = null;
            originGold = Convert.ToInt32(textGold.text);
        }

        _coCountingEffect = StartCoroutine(Utils.NumberCountingEffect(textGold, originGold, targetGold));

        Gold += cost;
    }

    private void EndGame()
    {
        ClearTimeScore = Math.Max(0, ((600 - ClearTimeScore) / 420) * 3000);
        IceCollectScore = IceCollectInfo.LevelOne * 10 + 
                          IceCollectInfo.LevelTwo * 30 + 
                          IceCollectInfo.LevelThree * 100;
        BestTemperatureScore = Math.Max(0, (75 - (int)_maxTemperature) * 20);
        StabilizeScore *= 50;
        ConstructionScore *= 100;
        DestructionScore *= 300;
        
        Managers.UI.ShowPopupUI<UIEndingPopup>();
    }
}