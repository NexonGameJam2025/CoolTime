using Core.Scripts;
using Core.Scripts.Manager;
using Core.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIMainPopup : UIPopup
{
    [SerializeField] private Button buttonStart;
    [SerializeField] private Button buttonHelp;
    [SerializeField] private Button buttonExit;

    public override void OnAwake()
    {
        base.OnAwake();
        
        buttonStart.onClick.AddListener(() =>
        {
            Managers.LoadingScene.LoadScene(nameof(Define.ESceneType.PuzzleSceneJJM));
        });
        buttonHelp.onClick.AddListener(() => 
        {
            // Managers.UI.ShowPopupUI<UIHelpPopup>();
        });
        buttonExit.onClick.AddListener(() => 
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}
