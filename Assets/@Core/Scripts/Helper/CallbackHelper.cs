using System;
using UnityEngine;

namespace Core.Scripts.Helper
{
    public class CallbackHelper : MonoBehaviour
    {
        private static MonoBehaviour _instance;
        public event Action CallbackAction;
    
        public static void Init(Action callback = null)
        {
            if (_instance != null)
            {
                callback?.Invoke();
                return;
            }
            
            _instance = new GameObject($"[{nameof(CallbackHelper)}]")
                .AddComponent<CallbackHelper>();
            DontDestroyOnLoad(_instance.gameObject);
            callback?.Invoke();
        }

        private void Update()
        {
            if (CallbackAction == null) return;
        
            CallbackAction.Invoke();
            CallbackAction = null;
        }
    }
}