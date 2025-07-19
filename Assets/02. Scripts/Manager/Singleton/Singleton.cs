using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = (T)FindFirstObjectByType(typeof(T));
                if (null == _instance)
                {
                    SetupInstance();
                }
            }
            return _instance;
        }
    }

    protected void Awake()
    {
        RemoveDuplicate();
        Init();
    }

    private static void SetupInstance()
    {
        _instance = (T)FindFirstObjectByType(typeof(T));
        if (null == _instance)
        {
            GameObject obj = new GameObject();
            obj.name = typeof(T).Name;
            _instance = obj.AddComponent<T>();
            DontDestroyOnLoad(obj);
        }
    }

    private void RemoveDuplicate()
    {
        if (null == _instance)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Init() { }
}