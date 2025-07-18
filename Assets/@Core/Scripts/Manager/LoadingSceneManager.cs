using System.Collections;
using Core.Scripts.Helper;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Scripts.Manager
{
    public class LoadingSceneManager
    {
        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private string _previousScene = string.Empty;
        private string _currentScene = string.Empty;
        private float _time = 0f;
        private UILoading _loading = null;
        
        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public bool IsRunLoad { get; private set; } = false;
        
        // --------------------------------------------------
        // Functions - Nomal
        // --------------------------------------------------
        public void Init()
        {
            _currentScene = SceneManager.GetActiveScene().name;
        }
        
        public void LoadScene(string SceneName, UILoading uiLoading = null, System.Action callback = null, System.Action sceneStartAction = null)
        {
            if(IsRunLoad)
                return;
            
            _previousScene = SceneManager.GetActiveScene().name;
            _currentScene = SceneName;
            
            if (uiLoading == null)
            {
                _loading = Managers.UI.ShowLoadingUI<UILoading>();
                CoroutineHelper.StartCoroutine(LoadAsyncSceneCoroutine(SceneName, true, callback, sceneStartAction));
            }
            else
            {
                _loading = uiLoading;
                CoroutineHelper.StartCoroutine(LoadAsyncSceneCoroutine(SceneName, false, callback, sceneStartAction));
            }

        }
        
        // --------------------------------------------------
        // Functions - Coroutine
        // --------------------------------------------------
        private IEnumerator LoadAsyncSceneCoroutine(string sceneName, bool isOpenLoading, System.Action callback, System.Action sceneStartAction = null)
        {
            IsRunLoad = true;
            
            if(isOpenLoading)
                _loading.OpenLoading();

            yield return new WaitUntil(() => _loading.gameObject && _loading.IsReady);
            
            Managers.UI.CloseAllPopupUI();

            yield return new WaitForSeconds(0.2f);
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
            
            _time = 0;
            while (_time < 0.5f) 
            {
                _time += Time.deltaTime;
                if (_time > 0.5f)
                    operation.allowSceneActivation = true;
                yield return null;
            }

            yield return new WaitUntil(() => operation.isDone);
            if (operation.isDone)
            {
                callback?.Invoke();
            }
            
            yield return new WaitUntil(() => _loading == null);
            
            IsRunLoad = false; 
            
            sceneStartAction?.Invoke(); 
        }
    }
}