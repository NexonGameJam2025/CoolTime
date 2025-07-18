using Core.Scripts.Manager;
using Core.Scripts.UI;
using UnityEngine;
using TMPro;

public class UILoading : UIBase
{
    [SerializeField] private TextMeshProUGUI textTip = null;

    public bool IsReady { get; private set; } = false;

    private void Awake() { Init(); }

    public virtual void Init()
    {
        textTip.text = "Loading...";
        
        Managers.UI.SetCanvas(gameObject,false);
    }

    public virtual void OpenLoading()
    {
        IsReady = true;
    }
}