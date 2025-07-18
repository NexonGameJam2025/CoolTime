using Core.Scripts.Manager;
using UnityEngine;

namespace Core.Scripts
{
    public class BaseScene : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Application.targetFrameRate = 60;
        }


        public virtual void Init()
        {
            Managers.UI.CloseLoadingUI();
        }
        
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Debug.Log("앱이 백그라운드로 전환됨");
            }
            else
            {
                Debug.Log("앱이 다시 활성화됨");
            }
        }
    }    
}
