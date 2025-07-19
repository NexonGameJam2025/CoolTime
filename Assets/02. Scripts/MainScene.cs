using Core.Scripts;
using Core.Scripts.Manager;
using UnityEngine;

public class MainScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();

        Init();
    }
    
    public override void Init()
    {
        base.Init();

        Managers.UI.ShowPopupUI<UIMainPopup>();
    }
}
