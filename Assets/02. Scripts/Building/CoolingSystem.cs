using UnityEngine;

public class CoolingSystem : MonoBehaviour
{
    [SerializeField] private Sprite _before;
    [SerializeField] private Sprite _after;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    void Update()
    {
        if (!GameManager.Instance.IsCooling)
        {
            _spriteRenderer.sprite = _before;
        }
        else
        {
            _spriteRenderer.sprite = _after;
        }
    }
}
