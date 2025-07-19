using System;
using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Manager;
using Core.Scripts.Table;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Core.Scripts
{
    public class GameInitializer : BaseScene
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("1. Loading Group")]
        [SerializeField] private Slider slider = null;
        [SerializeField] private TextMeshProUGUI textTipOne = null;
        [SerializeField] private TextMeshProUGUI textTipTwo = null;
        

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        // ----- Private
        private int _totalActionCount = 0;
        private int _actionIndex = 0;

        private float _startTime = 0;   
        private float _currentValue = 0;
    
        private List<Action> _actions = new();
    
        private bool _isInitialized = false;
        private bool _isLoadingStop = false;
        private bool _isCanLoadScene = false;
        
        private string _targetLoadingTextOne = "> 냉각 가스 채우는 중...\n> 얼음 총에 냉기 충전 중...\n> 고글 닦는 중...\n> 펭귄 부츠 미끄럼 방지 확인 중...\n> 출발 전 발 스트레칭 중...\n> 수달과 악수 중...\n> 얼음 스캔 중... 눈송이 개수 확인 중...\n> 적당히 차가운 농담 준비 중...";
        private string _targetLoadingTextTwo = "출발 준비 완료됐어요!";
    
        // --------------------------------------------------
        // Functions - Event
        // --------------------------------------------------

        private void Update()
        {
            if (_isCanLoadScene && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Managers.LoadingScene.LoadScene(nameof(Define.ESceneType.MainScene));
            }
        }
        private IEnumerator Start()
        {
            slider.value = 0f;

            textTipOne.DoTextClean(_targetLoadingTextOne, 5.0f).OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.3f, () =>
                {
                    textTipTwo.DoTextClean(_targetLoadingTextTwo, 0.4f)
                    .OnComplete(() => 
                    {            
                        _isCanLoadScene = true;
                    });
                });
            });
        
            SetAction();
            _totalActionCount = _actions.Count;

            StartInitialize();
            yield return null;
        }
    
        // --------------------------------------------------
        // Functions - Nomal
        // --------------------------------------------------
        // ----- Action Group
        private void SetAction()
        {
            // Managers Init
            _actions.Add(() =>
            {
                Debug.Log($"[GameInitializer.Action...] Action {_actionIndex + 1} Start | Managers Init");
                Managers.Init();
                InitDone();
            });
            
        
            // Tables ScriptableObject Create
            _actions.Add(() =>
            {
                Debug.Log($"[GameInitializer.Action...] Action {_actionIndex + 1} Start | Tables ScriptableObject Create");
                StartCoroutine(Tables.Create(InitDone));
            });
        
            // Sound Manager Init
            _actions.Add(() =>
            {
                Debug.Log($"[GameInitializer.Action...] Action {_actionIndex + 1} Start | Sound Manager Init");
                Managers.Sound.Init(InitDone);
            });
        }
    
        private void InitDone()
        {
            Debug.Log($"[GameInitializer.InitDone] Action Success Done {_actionIndex + 1}/{_actions.Count}");
        
            _actionIndex += 1;
        
            if (_actionIndex < _actions.Count)
            {
                SetUpdateLoadingUI();
                _actions[_actionIndex].Invoke();
            }
            else
            {
                _isInitialized = true;
            }
        }

        private void StartInitialize()
        {
            Debug.Log($"[GameInitializer.StartInitialize] Start Init");
            if (!_isInitialized)
            {
                _actions[_actionIndex].Invoke();
                StartCoroutine(Co_Loading());
            }
            else
            {
                OnInit();
            }
        }

        private void OnInit()
        {
            slider.value = 1f;
            slider.gameObject.SetActive(false);
        }
    
        // ----- UI Group
        private void SetUpdateLoadingUI()
        {
            if (!_isLoadingStop && _currentValue > 0.4f)
            {
                _isLoadingStop = true;
            }
            
            _currentValue = (float)_actionIndex / _totalActionCount;
            slider.value = _currentValue;
        }
    
        // --------------------------------------------------
        // Functions - Coroutine
        // --------------------------------------------------
        IEnumerator Co_Loading()
        {
            yield return new WaitUntil(() => _isInitialized);
            OnInit();
        }
    }
}
