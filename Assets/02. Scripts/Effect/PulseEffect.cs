// ----- C#
using DG.Tweening;

// ----- Unity
using UnityEngine;
using UnityEngine.EventSystems;

public class PulseEffect : MonoBehaviour
{
    // --------------------------------------------------
    // Components
    // --------------------------------------------------
    [Header("1. Transform Group")]
    [SerializeField] private Transform trans = null;
    
    // --------------------------------------------------
    // Variables
    // --------------------------------------------------
    // ----- Const
    private const float REPEAT_DURATION = 0.5f;
    private const float REPEAT_DELAY = 0.5f;
    private const float PULSE_SCALE = 0.9f;
    
    // ----- Private
    private Vector3 originScale = Vector3.one;
    
    // --------------------------------------------------
    // Functions - Event
    // --------------------------------------------------
    private void Awake()
    {
        originScale = trans.localScale;
    }

    private void OnEnable()
    {
        OnPulse();
    }

    private void OnDisable()
    {
        OffPulse();
    }

    private void OnDestroy()
    {
        OffPulse();
    }
    
    // --------------------------------------------------
    // Functions - Nomal
    // --------------------------------------------------
    private void OnPulse()
    {
        trans.localScale = originScale;
        DoPulse(REPEAT_DURATION);
    }

    private void OffPulse()
    {
        trans.localScale = originScale;
        trans.DOKill();
    }

    private void DoPulse(float duration)
    {
        var halfDuration = duration / 2;
        var targetScale = originScale * PULSE_SCALE;

        trans.DOScale(targetScale, halfDuration).SetLink(trans.gameObject).OnComplete(() =>
        {
            trans.DOScale(originScale, halfDuration).SetLink(trans.gameObject).OnComplete(() =>
            {
                DoPulse(duration);
            });
        }).SetDelay(REPEAT_DELAY);
    }
}
