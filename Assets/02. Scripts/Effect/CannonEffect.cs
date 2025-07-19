using DG.Tweening;
using UnityEngine;

public enum ECannonEffectType
{
    Single,
    Vertical,
    Horizontal,
    Cross
}

public class CannonEffect : MonoBehaviour
{
    [SerializeField] private GameObject objVertical;
    [SerializeField] private GameObject objHorizontal;
    [SerializeField] private GameObject objSingle;
    [SerializeField] private SpriteRenderer[] sprites;
    
    private static float EFFECT_DURATION = 0.5f;
    private static float EFFECT_SHOW_TIME = 0.2f;
    

    public void OnEffect(ECannonEffectType type)
    {
        foreach (var sprite in sprites)
        {
            var color = sprite.color;
            color.a = 0f;
            sprite.color = color;
        }
        
        switch (type)
        {
            case ECannonEffectType.Single:
                objSingle.SetActive(true);
                break;
            case ECannonEffectType.Vertical:
                objVertical.SetActive(true);
                break;
            case ECannonEffectType.Horizontal:
                objHorizontal.SetActive(true);
                break;
            case ECannonEffectType.Cross:
                objVertical.SetActive(true);
                objHorizontal.SetActive(true);
                break;
        }
        
        objSingle.transform.localScale = Vector3.zero;
        objVertical.transform.localScale = Vector3.zero;
        objHorizontal.transform.localScale = Vector3.zero;
        
        objSingle.transform.DOScale(Vector3.one, EFFECT_DURATION).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                objSingle.transform.DOScale(Vector3.zero, EFFECT_DURATION).SetEase(Ease.Linear).SetDelay(EFFECT_SHOW_TIME);
            });
        
        objVertical.transform.DOScale(Vector3.one, EFFECT_DURATION).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                objVertical.transform.DOScale(Vector3.zero, EFFECT_DURATION).SetEase(Ease.Linear).SetDelay(EFFECT_SHOW_TIME);
            });
        
        objHorizontal.transform.DOScale(Vector3.one, EFFECT_DURATION).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                objHorizontal.transform.DOScale(Vector3.zero, EFFECT_DURATION).SetEase(Ease.Linear).SetDelay(EFFECT_SHOW_TIME);
            });

        foreach (var sprite in sprites)
        {
            sprite.DOFade(1f, EFFECT_DURATION).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    sprite.DOFade(0f, EFFECT_DURATION).SetEase(Ease.Linear).SetDelay(EFFECT_SHOW_TIME);
                });
        }
    }
}
