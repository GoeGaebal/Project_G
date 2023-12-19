using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ArtifactManager
{
    [HideInInspector] public Artifact[] artifacts = new Artifact[3];//실제 착용 중인 유물
    [HideInInspector] public List<Artifact> artifactScrolls = new List<Artifact>();//현재 보유 중인 유물 목록
    
    //UI_Artifact의 Init()이 실행될 때,
    //ArtifactManager의 artifacts, artifactScrolls 배열에서 아티팩트를 가져와 슬롯을 생성, 저장시켜둠
    [HideInInspector] public UI_ArtifactSlot[] equippedArtifactSlots = new UI_ArtifactSlot[3];//현재 착용 중인 유물의 UI 상에서의 슬롯
    [HideInInspector] public List<UI_ArtifactSlot> artifactScrollSlots = new List<UI_ArtifactSlot>();//보유 중인 유물의 UI 상에서의 슬롯
    [HideInInspector] public int currentIndex = -1;

    Player player;

    public void Init()
    {
        player = Managers.Network.LocalPlayer;

        GameObject root = GameObject.Find("@Artifact");
        if (root == null)
        {
            root = new GameObject { name = "@Artifact" };
            UnityEngine.Object.DontDestroyOnLoad(root);
        }

        for(int i = 0; i < 3; i++)
        {
            if(artifacts[i] != null)
            {
                ArtifactTileSet.setImage(i, artifacts[i].Image);
            }
        }

        AddScroll("Artifact_0");
        AddScroll("Artifact_1");
        AddScroll("Artifact_2");
        AddScroll("Artifact_3");
    }

    public void AddScroll(string path)//새로운 유물 획득
    {
        foreach (var artifact in artifactScrolls)
        {
            if (artifact != null && artifact.name == path)
            {
                return;
            }
        }
        artifactScrolls.Add(Managers.Resource.Load<Artifact>($"Prefabs/Objects/NonCharacter/Interactable/Artifact/Artifacts/{path}"));
    }

    public UI_ArtifactSlot MakeArtifactSlot(Transform parent = null)
    {//유물 목록을 실제 UI에 생성
        string path = "UI_ArtifactSlot";
        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{path}", Vector3.zero, Quaternion.identity);
        go.transform.SetParent(parent);
        return Util.GetOrAddComponent<UI_ArtifactSlot>(go); ;
    }
    
    public void SetCurrentIndex(int idx)
    {
        currentIndex = idx;
        //Debug.Log(idx);
    }

    public void SelectArtifact(Artifact artifact)
    {//유물 목록 중 한 가지를 idx번째 슬롯에 장착
        artifacts[currentIndex] = artifact;
        ArtifactTileSet.setImage(currentIndex, artifact.Image);
        artifact.Select();
    }

    public void DeselectArtifact()
    {
        artifacts[currentIndex]?.Deselect();
        artifacts[currentIndex] = null;
        equippedArtifactSlots[currentIndex] = null;
        ArtifactTileSet.resetImage(currentIndex);
    }

    public void SetSlotEquipped(Artifact artifact, bool flag)
    {
        artifactScrollSlots[FindArtifactIndex(artifact)].SetEquipped(flag);
    }

    public int FindArtifactIndex(Artifact artifact)
    {
        for(int i = 0; i < artifactScrolls.Count; i++)
        {
            if(artifactScrolls[i] == artifact)
            {
                return i;
            }
        }

        return -1;
    }
}
