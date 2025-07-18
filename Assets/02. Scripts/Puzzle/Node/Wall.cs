using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Wall : Building
{
    public bool IsEnable { get; private set; } = false;
    public event Action OnEnableWall;
    public event Action OnDisableWall;
    
    private static float WALL_LIFE_TIME = 60.0f;
    private static float WALL_FADE_TIME = 1.0f;
    private Coroutine _coWallLifeTimer;

    private void OnEnable()
    {
        IsEnable = true;
        OnEnableWall?.Invoke();
        
        _coWallLifeTimer = StartCoroutine(CO_WallLifeTimer(WALL_LIFE_TIME, () =>
        {
            spriteBuilding.DOFade(0.0f, WALL_FADE_TIME).OnComplete(() => gameObject.SetActive(false));
        }));
    }
    
    private void OnDisable()
    {
        IsEnable = false;
        OnDisableWall?.Invoke();
        
        if (_coWallLifeTimer != null)
        {
            StopCoroutine(_coWallLifeTimer);
            _coWallLifeTimer = null;
        }
    }

    private void OnDestroy()
    {
        if (_coWallLifeTimer != null)
        {
            StopCoroutine(_coWallLifeTimer);
            _coWallLifeTimer = null;
        }
    }

    public override void OnCollision(EManaLevel manaLevel)
    {
        // Do Nothing
    }
    
    private IEnumerator CO_WallLifeTimer(float duration, Action doneCallback)
    {
        yield return new WaitForSeconds(duration);
        
        doneCallback?.Invoke();
    }
}
