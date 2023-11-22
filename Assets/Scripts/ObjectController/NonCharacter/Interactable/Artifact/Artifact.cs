using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Artifact/Artifact")]
public class Artifact : ScriptableObject
{
    public int ID => _id;
    public string Name => _name;
    public string Tooltip => _tooltip;
    public Sprite Image => _Image;

    [SerializeField] private int _id = 0;
    [SerializeField] private string _name = "";
    [SerializeField] private string _tooltip = "";
    [SerializeField] private Sprite _Image;

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
