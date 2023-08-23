using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Artifact/Artifact")]
public class Artifact : ScriptableObject
{
    public int ID => _id;
    public Sprite Image => _Image;
    public float HpIncrease => _hpIncrease;
    public float DamageIncrease => _damageIncrease;
    public float ShipSpeedIncrease => _shipSpeedIncrease;

    [SerializeField] private int _id = 0;
    [SerializeField] private Sprite _Image;
    [SerializeField] private float _hpIncrease = 0;
    [SerializeField] private float _damageIncrease = 0;
    [SerializeField] private float _shipSpeedIncrease = 0;
}
