using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BuilderSystem : MonoBehaviour
{
    [SerializeField] private List<Transform> transBuilderHomes;
    [SerializeField] private GameObject builderPrefab;
    [SerializeField] private float BUILDER_MOVE_SPEED = 5f;
    [SerializeField] private float BUILDER_BUILD_EFFECT_DURATION = 1f;

    public void OnStartBuilder(int start, Transform endPos, Action doneCallback = null)
    {
        var startPos = transBuilderHomes[start];
        
        var duration = Vector2.Distance(startPos.position, endPos.position) / BUILDER_MOVE_SPEED;
        
        var builder = Instantiate(builderPrefab, startPos.position, Quaternion.identity);
        
        var sequence = DOTween.Sequence();
        sequence.Append(builder.transform.DOMove(endPos.position, duration));
        sequence.AppendInterval(BUILDER_BUILD_EFFECT_DURATION);
        sequence.Append(builder.transform.DOMove(startPos.position, duration)
            .OnStart(() =>
            {
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
