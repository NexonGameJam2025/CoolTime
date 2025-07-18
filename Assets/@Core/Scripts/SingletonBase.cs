using System;

namespace Core.Scripts
{
    public class SingletonBase<T> where T : class, new()
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();

                return instance;
            }
        }
        
        public static void Init(Action doneCallback = null)
        {
            if (instance == null)
                instance = new T();
            
            doneCallback?.Invoke();
        }
    }
}

