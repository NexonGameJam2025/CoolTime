using Core.Scripts;
using Core.Scripts.UI;
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
    
    public override void Init()
    {
        base.Init();
        
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
    }
}
