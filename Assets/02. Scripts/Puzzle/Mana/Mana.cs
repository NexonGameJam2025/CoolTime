using UnityEngine;

public class Mana : MonoBehaviour
{
    [SerializeField] private int _manaLevel = 1;
    public int ManaLevel => _manaLevel;
}