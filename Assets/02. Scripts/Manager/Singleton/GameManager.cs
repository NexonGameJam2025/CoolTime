using System;
using Core.Scripts;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private TextMeshProUGUI textGold;
    
    private float _elapsedTime = 0f;
    private bool _isPaused = false;
    public int Gold { get; private set; } = 100; // TODO : Temperary 100 gold

    private Coroutine _coCountingEffect;

    /// <summary>
    /// 게임 시작 후 경과 시간 (초)
    /// </summary>
    public float ElapsedTime => _elapsedTime;

    /// <summary>
    /// 게임 일시정지 상태
    /// </summary>
    public bool IsPaused => _isPaused;

    private void Start()
    {
        textGold = GameObject.Find("Text_Currency").GetComponent<TextMeshProUGUI>();
        textGold.text = Gold.ToString();
    }

    private void Update()
    {
        // 게임이 멈춰있지 않을 때만 시간 계산
        if (!_isPaused)
        {
            _elapsedTime += Time.deltaTime;
        }
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