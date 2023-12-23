using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : ScriptableObject
{
    public int ID => _id;
    public string Name => _name;
    public string Tooltip => _tooltip;
    public Sprite Image => _Image;

    public int DamageAdd => _damageAdd;
    public float DamageMul => _damageMul;

    public int HPAdd => _hpAdd;
    public float HPMul => _hpMul;


    [SerializeField] private int _id = 0;
    [SerializeField] private string _name = "";
    [SerializeField] private string _tooltip = "";
    [SerializeField] private Sprite _Image;

    [SerializeField] private int _damageAdd = 0;
    [SerializeField] private float _damageMul = 0;

    [SerializeField] private int _hpAdd = 0;
    [SerializeField] private float _hpMul = 0;

    public virtual void Select()
    {
        Managers.Network.LocalPlayer.ArtifactSkills[Managers.Artifact.currentIndex] = Skill;
        Managers.Network.LocalPlayer.ArtifactSkills[Managers.Artifact.currentIndex].Invoke();
    }

    public virtual void Deselect()
    {
        Managers.Network.LocalPlayer.ArtifactSkills[Managers.Artifact.currentIndex] = null;
    }
    public virtual void Skill()
    {

    }
}
