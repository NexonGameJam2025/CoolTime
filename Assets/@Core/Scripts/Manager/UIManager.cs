using System.Collections.Generic;
using Core.Scripts.UI;
using UnityEngine;

namespace Core.Scripts.Manager
{
    public class UIManager
    {
        private int _order = 10;
        private Stack<UIPopup> _popupPool = new();
        private UILoading _loadingUI = null;


        public GameObject Root
        {
            get
            {
                GameObject root = GameObject.Find("@UI_Root");
                if (root == null)
                    root = new GameObject { name = "@UI_Root" };
                return root;
            }
        }
        
        public GameObject GlobalUIRoot
        {
            get
            {
                GameObject root = GameObject.Find("@UI_Global_Root");
                if (root == null)
                {
                    root = new GameObject { name = "@UI_Global_Root" };
                    GameObject.DontDestroyOnLoad(root);
                }
                return root;
            }
        }
    
        public void Init()
        {
            var root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                root = new GameObject { name = "@UI_Root" };
            }
        }

        public void SetCanvas(GameObject go, bool sort = true)
        {
            var canvas= Utils.GetOrAddComponent<Canvas>(go);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;

            if (sort)
            {
                canvas.sortingOrder = _order;
                _order++;
            }
            else
            {
                canvas.sortingOrder = 0;
            }
        }
    
        public T ShowPopupUI<T>(string name = null) where T : UIPopup
        {
            if(string.IsNullOrEmpty(name))
                name = typeof(T).Name;
        
            var go = Managers.Resource.Instantiate($"UI/Popup/{name}");
            var popup = Utils.GetOrAddComponent<T>(go);

            if (_popupPool.Count != 0)
            {
                var prevPopup = _popupPool.Peek();
                prevPopup.OnOffDim(false);
            }
        
            _popupPool.Push(popup);

            Debug.Log($"[UIManager.ShowPopupUI] {popup.name}를 생성하였습니다.");

            popup.transform.SetParent(Root.transform);
        
            return popup;
        }
    

        public void ClosePopupUI(UIPopup popup)
        {
            if (_popupPool.Count == 0)
            {
                Debug.Log($"[UIManager.ClosePopupUI] Count == 0 -> {popup.name}를 닫지 못했습니다.");
                return;
            }
        
            if(_popupPool.Peek() != popup)
            {
                Debug.Log($"[UIManager.ClosePopupUI] Peek() != popup -> {popup.name}를 닫지 못했습니다.");
                return;
            }

            ClosePopupUI();
        }
    
        public void ClosePopupUI()
        {
            if (_popupPool.Count == 0)
                return;
        
            var popup = _popupPool.Pop();
            if (_popupPool.Count != 0)
            {
                var currPopup = _popupPool.Peek();
                currPopup.Refresh();
                currPopup.OnOffDim(true);
            }

            Debug.Log($"[UIManager.ClosePopupUI] {popup.gameObject.name} 팝업을 닫았습니다.");
            Object.Destroy(popup.gameObject);
            popup = null;
            _order--;
        }
    
        public void CloseAllPopupUI()
        {
            while (_popupPool.Count>0)
                ClosePopupUI();
        }
    
        public UIPopup GetCurrentPopupUI()
        {
            return _popupPool.Count == 0 ? null : _popupPool.Peek();
        }
        
        public T ShowLoadingUI<T>(string name = null) where T : UILoading
        {
            if (_loadingUI && _loadingUI.name == typeof(T).Name)
            {
                Managers.UI._loadingUI.gameObject.SetActive(true);
                return (T)_loadingUI;
            }

            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;
        
            var go = Managers.Resource.Instantiate($"UI/Loading/{name}");
            var loadingUI = Utils.GetOrAddComponent<T>(go);
        
            _loadingUI = loadingUI;
        
            go.transform.SetParent(GlobalUIRoot.transform);
        
            SetCanvas(go, false);
        
            go.GetComponent<Canvas>().sortingOrder = 9999;
        
            Debug.Log($"[UIManager.ShowLoadingUI] {go.name}를 생성하였습니다.");

            return loadingUI;
        }
        
        public void CloseLoadingUI()
        {
            if (_loadingUI == null)
                return;
            
            Debug.Log($"[UIManager.CloseLoadingUI] {_loadingUI.name} 로딩 UI를 닫았습니다.");
            
            Managers.Resource.Destroy(_loadingUI.gameObject);
            _loadingUI = null;
            
        }
    }
}