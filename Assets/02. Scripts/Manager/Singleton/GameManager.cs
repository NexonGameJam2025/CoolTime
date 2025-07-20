using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts;
using TMPro;
using UnityEngine;

[System.Serializable]
public class CoolingEffectData
{
    public EManaLevel level;
    public float duration; // 지속 시간 (초)
    public float coolingRatePerSecond; // 초당 온도 변화량 (음수 값)
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private TextMeshProUGUI textGold;
    private bool _isCooling = false;
    public bool IsCooling => _isCooling;
    

    private float _elapsedTime = 0f;
    private bool _isPaused = false;
    private float _temperature = 0f;
    public float Temperature => _temperature;

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
    private Coroutine _coCoolingEffect;
    private float _currentCoolingRate = 0f;

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

            float totalChangeRate = -_currentCoolingRate;

            _temperature = 50f + _twoCoefficient * _elapsedTime * _elapsedTime + _oneCoefficient * _elapsedTime + totalChangeRate * Time.deltaTime;
        }
    }

    /// <summary>
    /// 마나 레벨에 맞는 냉각 효과를 적용합니다.
    /// </summary>
    public void ApplyCooling(EManaLevel level)
    {
        _isCooling = true;
        CoolingEffectData effectData = _coolingEffectDataList.FirstOrDefault(e => e.level == level);

        if (effectData == null)
        {
            Debug.LogWarning($"Cooling effect for ManaLevel '{level}' not found.");
            return;
        }

        if (_coCoolingEffect != null)
        {
            StopCoroutine(_coCoolingEffect);
        }
        _coCoolingEffect = StartCoroutine(CO_ApplyCooling(effectData.duration, effectData.coolingRatePerSecond));
    }

    private IEnumerator CO_ApplyCooling(float duration, float coolingRate)
    {
        _currentCoolingRate = coolingRate; // 냉각 효과 적용 시작
        yield return new WaitForSeconds(duration);
        _currentCoolingRate = 0f; // 냉각 효과 종료
        _isCooling = false;
        _coCoolingEffect = null;
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
}