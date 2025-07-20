using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Mana : MonoBehaviour
{
    [SerializeField] private int _manaLevel = 1;
    public int ManaLevel => _manaLevel;

    [SerializeField] private Sprite[] _manaSprite = new Sprite[3];

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateColor();
    }

    public void SetLevel(int level)
    {
        if (level >= 1 && level <= 3)
        {
            _manaLevel = level;
            if (_manaLevel == 3)
            {
                GameManager.Instance.IctCollector++;
            }

            if (_manaLevel != 1)
            {
                GameManager.Instance.Phycho = false;
            }
            UpdateColor();
        }
        else
        {
            Debug.LogWarning("4 이상 레벨 수치 들어감");
        }
    }

    private void UpdateColor()
    {
        if (_spriteRenderer == null || _manaSprite.Length < _manaLevel)
        {
            return;
        }

        _spriteRenderer.sprite = _manaSprite[_manaLevel - 1];
    }
}