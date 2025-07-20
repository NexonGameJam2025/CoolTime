using Core.Scripts;
using Core.Scripts.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIEndingPopup : UIPopup
{
    [SerializeField] private TextMeshProUGUI textClearTimeScore;
    [SerializeField] private TextMeshProUGUI textIceCollectScore;
    [SerializeField] private TextMeshProUGUI textBestTemperatureScore;
    [SerializeField] private TextMeshProUGUI textStabilizeScore;
    [SerializeField] private TextMeshProUGUI textConstructionScore;
    [SerializeField] private TextMeshProUGUI textDestructionScore;
    [SerializeField] private TextMeshProUGUI textTotalScore;
    [SerializeField] private RectTransform transStamp;
    [SerializeField] private RectTransform transStampOne;
    [SerializeField] private RectTransform transStampTwo;
    [SerializeField] private GameObject objMarkS;
    [SerializeField] private GameObject objMarkA;
    [SerializeField] private GameObject objMarkB;
    [SerializeField] private GameObject objMarkC;
    [SerializeField] private GameObject objMarkF;
    [SerializeField] private GameObject objIctCollector;
    [SerializeField] private GameObject objSuperSpeedIce;
    [SerializeField] private GameObject objTemperatureControlMaster;
    [SerializeField] private GameObject objPerfectBuildingDesigner;
    [SerializeField] private GameObject objMasterBuilder;
    [SerializeField] private GameObject objSurviveFromHellFire;
    [SerializeField] private GameObject objPhycho;
    [SerializeField] private GameObject objDestructionKing;

    
    public override void OnAwake()
    {
        base.OnAwake();
        Init();
    }
    public override void Init()
    {
        base.Init();
        buttonClose.gameObject.SetActive(false);
        
        var totalScore = GameManager.Instance.ClearTimeScore +
                         GameManager.Instance.IceCollectScore +
                         GameManager.Instance.BestTemperatureScore +
                         GameManager.Instance.StabilizeScore +
                         GameManager.Instance.ConstructionScore +
                         GameManager.Instance.DestructionScore;
        
        StartCoroutine(Utils.NumberCountingEffect(textClearTimeScore, 0, GameManager.Instance.ClearTimeScore));
        StartCoroutine(Utils.NumberCountingEffect(textIceCollectScore, 0, GameManager.Instance.IceCollectScore));
        StartCoroutine(Utils.NumberCountingEffect(textBestTemperatureScore, 0, GameManager.Instance.BestTemperatureScore));
        StartCoroutine(Utils.NumberCountingEffect(textStabilizeScore, 0, GameManager.Instance.StabilizeScore));
        StartCoroutine(Utils.NumberCountingEffect(textConstructionScore, 0, GameManager.Instance.ConstructionScore));
        StartCoroutine(Utils.NumberCountingEffect(textDestructionScore, 0, GameManager.Instance.DestructionScore));
        StartCoroutine(Utils.NumberCountingEffect(textTotalScore, 0, totalScore));

        OnStampEffect(totalScore);
        ShowSpecialMission();
    }

    private void OnStampEffect(int totalScore)
    {
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(2.0f);
        sequence.Append(transStamp.DOAnchorPos(transStampOne.anchoredPosition, 0.3f).SetEase(Ease.InSine));
        sequence.AppendCallback(() => ShowMark(totalScore) );
        sequence.AppendInterval(0.5f);
        sequence.Append(transStamp.DOAnchorPos(transStampTwo.anchoredPosition, 0.3f).SetEase(Ease.InSine))
            .OnComplete(() => buttonClose.gameObject.SetActive(true));

        sequence.SetLink(gameObject);
        sequence.Play();
    }

    private void ShowMark(int totalScore)
    {
        switch (totalScore)
        {
            case >= 10000:
                objMarkS.SetActive(true);
                break;
            case >= 8000:
                objMarkA.SetActive(true);
                break;
            case >= 6000:
                objMarkB.SetActive(true);
                break;
            case >= 3000:
                objMarkC.SetActive(true);
                break;
            case < 3000:
                objMarkF.SetActive(true);
                break;
        }
    }

    private void ShowSpecialMission()
    {
        objIctCollector.SetActive(GameManager.Instance.IctCollector >= 10);
        objSuperSpeedIce.SetActive(GameManager.Instance.ElapsedTime <= 300f);
        objTemperatureControlMaster.SetActive(GameManager.Instance.MaxTemperature <= 70.0f);
        objPerfectBuildingDesigner.SetActive(GameManager.Instance.PerfectBuildingDesigner);
        objMasterBuilder.SetActive(
            GameManager.Instance.MasterBuilder.Wall >= 10 &&
            GameManager.Instance.MasterBuilder.Shield >= 1 &&
            GameManager.Instance.MasterBuilder.Cannon >= 1);
        objSurviveFromHellFire.SetActive(GameManager.Instance.MaxTemperature >= 70.0f);
        objPhycho.SetActive(GameManager.Instance.Phycho);
        objDestructionKing.SetActive(GameManager.Instance.DestructionKing >= 5);
    }
}
