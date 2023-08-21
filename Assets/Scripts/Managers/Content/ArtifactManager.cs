using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager
{
    [HideInInspector] public Artifact[] artifacts = new Artifact[3];//실제 착용 중인 유물
    [HideInInspector] public Artifact[] artifactScrolls = new Artifact[0];//현재 보유 중인 유물 목록
    [HideInInspector] public UI_ArtifactSlot[] equippedArtifactSlots = new UI_ArtifactSlot[3];
    [HideInInspector] public int currentIndex = -1;

    public void Init()
    {
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
    }

    public void AddScroll(string path)//새로운 유물 획득
    {
        for(int i = 0; i < artifactScrolls.Length; i++)
        {
            if(artifactScrolls[i].name == path)
            {
                return;
            }
        }
        Array.Resize(ref artifactScrolls, artifactScrolls.Length + 1);
        artifactScrolls[artifactScrolls.Length - 1] = Managers.Resource.Load<Artifact>($"Prefabs/Artifacts/{path}");
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
    }

    public void DeselectArtifact()
    {
        artifacts[currentIndex] = null;
        ArtifactTileSet.resetImage(currentIndex);
    }
}
