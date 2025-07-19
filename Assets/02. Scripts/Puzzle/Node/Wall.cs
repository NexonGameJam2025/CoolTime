using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Wall : Building
{
    [SerializeField] private SpriteRenderer rendererPillarOne;
    [SerializeField] private SpriteRenderer rendererPillarTwo;
    [SerializeField] private SpriteRenderer rendererWall;
    [SerializeField] private Sprite spriteBrokenWall;
    
    public bool IsEnable { get; private set; } = false;
    public event Action OnActivateWall;
    public event Action OnDeactivateWall;
    
    private static float WALL_LIFE_TIME = 60.0f;
    private static float WALL_FADE_TIME = 1.0f;
    private static Vector3 WALL_BROKEN_POWER = new(0.0f, 0.035f, 0.0f);
    private Coroutine _coWallLifeTimer;
    
    private void OnDisable()
    {
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

    public override void OnBuilderStart(Vector2 coordinate)
    {
        base.OnBuilderStart(coordinate);
        
        // Do Nothing
    }

    public override void OnFinishBuild()
    {
        base.OnFinishBuild();
        
        rendererWall.DOFade(1.0f, 0.5f);
        rendererPillarOne.DOFade(1.0f, 0.5f);
        rendererPillarTwo.DOFade(1.0f, 0.5f);
        
        IsEnable = true;
        OnActivateWall?.Invoke();
        
        _coWallLifeTimer = StartCoroutine(CO_WallLifeTimer(WALL_LIFE_TIME));
    }

    public override void OnDeActivate()
    {
        base.OnDeActivate();
        
        rendererWall.sprite = spriteBrokenWall;
        
        rendererPillarOne.transform.DOShakePosition(10.0f, WALL_BROKEN_POWER, fadeOut: false, vibrato: 7);
        rendererPillarTwo.transform.DOShakePosition(10.0f, WALL_BROKEN_POWER, fadeOut: false, vibrato: 7).OnComplete(() =>
        {
            rendererPillarOne.DOFade(0.0f, WALL_FADE_TIME);
            rendererPillarTwo.DOFade(0.0f, WALL_FADE_TIME);
            rendererWall.DOFade(0.0f, WALL_FADE_TIME);
            
            IsEnable = false;
            OnDeactivateWall?.Invoke();
        });
    }

    protected override void TogglePreviewImage(bool isOn)
    {
        base.TogglePreviewImage(isOn);
        
        var opacity = isOn ? PreviewImageOpacity : 1f;
        
        var colorOne = rendererPillarOne.color;
        var colorTwo = rendererPillarTwo.color;
        var colorWall = rendererWall.color;
        
        colorOne.a = opacity;
        colorTwo.a = opacity;
        colorWall.a = opacity;
        
        rendererPillarOne.color = colorOne;
        rendererPillarTwo.color = colorTwo;
        rendererWall.color = colorWall;
    }

    public override void OnCollisionMana(EManaLevel manaLevel)
    {
        // Do Nothing
    }
    
    private IEnumerator CO_WallLifeTimer(float duration, Action doneCallback = null)
    {
        yield return new WaitForSeconds(duration);

        OnDeActivate();
        doneCallback?.Invoke();
    }
}
