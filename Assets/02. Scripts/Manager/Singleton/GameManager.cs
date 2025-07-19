using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private float _elapsedTime = 0f;
    private bool _isPaused = false;

    /// <summary>
    /// ���� ���� �� ��� �ð� (��)
    /// </summary>
    public float ElapsedTime => _elapsedTime;

    /// <summary>
    /// ���� �Ͻ����� ����
    /// </summary>
    public bool IsPaused => _isPaused;

    private void Update()
    {
        // ������ �������� ���� ���� �ð� ���
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