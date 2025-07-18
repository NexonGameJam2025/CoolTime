using UnityEngine;

namespace Core.Scripts
{
    public abstract class MonoBehaviourSingletonBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance = null;
        protected static bool instantiated = false;
        protected bool destroyed = false;
    
        public static T instance 
        { 
            get {
                if (_instance == null) 
                {
                    GetInstance();
                }

                return _instance;
            }
        }
    
        protected virtual void Awake()
        {
            if (destroyed)
                return;

            if (_instance == null) 
                SetInstance(this as T);
            else if (_instance != this)
            {
                Debug.LogError($"[{GetType().Name}.Awake] Instance already created. Destroying {GetType().Name}.");
                destroyed = true;
                Destroy(gameObject);
            }
        }

        private static void SetInstance(T ins)
        {
            _instance = ins;
            instantiated = true;
            (ins as MonoBehaviourSingletonBase<T>).destroyed = false;
            DontDestroyOnLoad(ins);
        }

        private static void GetInstance()
        {
            var objs = FindObjectsOfType<T>();

            if (objs.Length == 0) 
            {
                Debug.LogError($"[{typeof(T).Name}.GetInstance] Place the {typeof(T).Name} in the scene.");
                return;
            }
        
            if (objs.Length > 1)
            {
                Debug.LogError($"[{typeof(T).Name}.GetInstance] The scene contains more than one {typeof(T).Name}. Unintended behavior can be detected.");
                for (int i = 1; i < objs.Length; i++) 
                {
                    (objs[i] as MonoBehaviourSingletonBase<T>).destroyed = true;
                    Destroy(objs[i]);
                }
            }

            SetInstance(objs[0]);
        }
    }
}