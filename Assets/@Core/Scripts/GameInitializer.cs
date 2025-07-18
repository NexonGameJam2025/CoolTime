using System;
using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Manager;
using Core.Scripts.Table;
using TMPro;
using UnityEngine;
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
        [SerializeField] private TextMeshProUGUI textTip = null;

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
    
        // --------------------------------------------------
        // Functions - Event
        // --------------------------------------------------
        private IEnumerator Start()
        {
            slider.value = 0f;
            slider.gameObject.SetActive(true);
        
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
            textTip.gameObject.SetActive(false);
        
            // Start Button Set
            Managers.LoadingScene.LoadScene(nameof(Define.ESceneType.MainScene));
        }
    
        // ----- UI Group
        private void SetUpdateLoadingUI()
        {
            if (!_isLoadingStop && _currentValue > 0.4f)
            {
                textTip.text = "Loading...";
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
