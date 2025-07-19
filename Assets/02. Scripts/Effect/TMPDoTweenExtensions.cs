using UnityEngine;
using TMPro;
using DG.Tweening;

public static class TMPDoTweenExtensions
{
    // 원래 텍스트를 덮어씌우면서 DoText
    public static Tweener DoText(this TMP_Text text, string targetText, float duration)
    {
        // 현재 텍스트를 초기값으로 설정
        string currentText = text.text;

        // DOTween의 To 메서드를 사용하여 텍스트를 점진적으로 변경
        return DOTween.To(
            () => currentText, // 현재 텍스트 값
            x => text.text = x, // 텍스트 업데이트
            targetText, // 목표 텍스트
            duration 
        ).SetEase(Ease.Linear); // 디폴트는 Linear Ease로
    }
    
    // 원래 텍스트는 싹 지우고 DoText
    public static Tweener DoTextClean(this TMP_Text text, string targetText, float duration)
    {
        int currentLength = 0;
        return DOTween.To(
            () => currentLength,
            x =>
            {
                currentLength = x;
                text.text = targetText.Substring(0, currentLength);
            },
            targetText.Length,
            duration
        ).SetEase(Ease.Linear);
    }
}