using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private float _elapsedTime = 0f;
    private bool _isPaused = false;

    /// <summary>
    /// 게임 시작 후 경과 시간 (초)
    /// </summary>
    public float ElapsedTime => _elapsedTime;

    /// <summary>
    /// 게임 일시정지 상태
    /// </summary>
    public bool IsPaused => _isPaused;

    private void Update()
    {
        // 게임이 멈춰있지 않을 때만 시간 계산
        if (!_isPaused)
        {
            _elapsedTime += Time.deltaTime;
        }
    }

    public void PauseGame()
    {
        _isPaused = true;
    }

    public void ResumeGame()
    {
        _isPaused = false;
    }
}