using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BuilderSystem : MonoBehaviour
{
    [SerializeField] private List<Transform> transBuilderHomes;
    [SerializeField] private GameObject builderPrefab;
    private float BUILDER_MOVE_SPEED = 5f;
    private float BUILDER_BUILD_EFFECT_DURATION = 1f;
    private float BUILDER_FADE_DURATION = 0.3f;

    public void OnStartBuilder(int start, Transform endPos, Action onStartBuild = null, Action doneCallback = null)
    {
        var startPos = transBuilderHomes[start];
        
        var duration = Vector2.Distance(startPos.position, endPos.position) / BUILDER_MOVE_SPEED;
        
        var builder = Instantiate(builderPrefab, startPos.position, Quaternion.identity);
        var spriteRenderer = builder.GetComponent<SpriteRenderer>();
        
        var sequence = DOTween.Sequence();
        sequence.Append(builder.transform.DOMove(endPos.position, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                onStartBuild?.Invoke();
                spriteRenderer.DOFade(0f, BUILDER_FADE_DURATION);
            }));
        sequence.AppendInterval(BUILDER_BUILD_EFFECT_DURATION);
        sequence.Append(builder.transform.DOMove(startPos.position, duration)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                spriteRenderer.DOFade(1f, BUILDER_FADE_DURATION);
                spriteRenderer.flipX = true;
                doneCallback?.Invoke();
            })
            .OnComplete(() =>
            {
                Destroy(builder);
            }));
            
        
        sequence.SetLink(builder);
        sequence.Play();
    }
}
