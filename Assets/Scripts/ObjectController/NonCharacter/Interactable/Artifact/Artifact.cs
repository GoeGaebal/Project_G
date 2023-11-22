using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Artifact/Artifact")]
public class Artifact : MonoBehaviour
{
    public int ID => _id;
    public string Name => _name;
    public string Tooltip => _tooltip;
    public Sprite Image => _Image;
    /*
    public float HpIncrease => _hpIncrease;
    public float DamageIncrease => _damageIncrease;
    public float ShipSpeedIncrease => _shipSpeedIncrease;
    */

    [SerializeField] private int _id = 0;
    [SerializeField] private string _name = "";
    [SerializeField] private string _tooltip = "";
    [SerializeField] private Sprite _Image;
    /*
    [SerializeField] private float _hpIncrease = 0f;
    [SerializeField] private float _damageIncrease = 0f;
    [SerializeField] private float _shipSpeedIncrease = 0f;
    */

    public virtual void Select()
    {

    }

    public virtual void Deselect()
    {

    }
    public virtual void Skill()
    {

    }
}
