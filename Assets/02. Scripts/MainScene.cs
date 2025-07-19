using Core.Scripts;
using Core.Scripts.Manager;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MainScene : BaseScene
{
    [SerializeField] private Transform transPenguin;
    
    protected override void Awake()
    {
        base.Awake();

        Init();
    }
    
    public override void Init()
    {
        base.Init();

        Managers.UI.ShowPopupUI<UIMainPopup>();
        transPenguin.DOMove(new Vector3(-3.5f, 0.0f, 0.0f), 0.7f)
            .SetEase(Ease.OutCirc);
    }
}
