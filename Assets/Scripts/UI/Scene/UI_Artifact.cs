using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Artifact : UI_Scene
{
    private GameObject _content;
    private GameObject _ui;

    public static System.Action open;
    public static System.Action close;

    enum GameObjects
    {
        UI_Artifact,
        Content,
    }

    enum Buttons
    {
        CloseButton,
    }

    private void Awake()
    {
        open = () => { OpenArtifact(); };
        close = () => { CloseArtifact(); };
    }

    public override void Init()
    {
        //base.Init();
        Managers.UI.SetCanvas(gameObject, true);

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        _ui = Get<GameObject>((int)GameObjects.UI_Artifact);
        _ui.SetActive(false);
        _content = Get<GameObject>((int)GameObjects.Content);

        for (int i = 0; i < Managers.Artifact.artifactScrolls.Count; i++)
        {
            var slot = Managers.Artifact.MakeArtifactSlot(_content.transform);
            slot.Init();
            slot.SetArtifact(Managers.Artifact.artifactScrolls[i]);

            Managers.Artifact.artifactScrollSlots.Add(slot);

            for (int j = 0; j < 3; j++)
            {//착용중인 유물이면
                if(Managers.Artifact.artifacts[j] == slot.artifact)
                {
                    Managers.Artifact.equippedArtifactSlots[j] = slot;
                    slot.SetEquipped(true);
                    break;
                }
            }
        }

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(CloseButton);
    }

    private void OnDestroy()
    {
        Managers.Artifact.artifactScrollSlots.Clear();
    }

    public void OpenArtifact()
    {
        _ui.SetActive(true);
        Managers.Input.PlayerActionMap.Disable();
    }

    public void CloseArtifact()
    {
        _ui.SetActive(false);
        Managers.Input.PlayerActionMap.Enable();
    }

    public void CloseButton(PointerEventData evt)
    {
        _ui.SetActive(false);
        Managers.Input.PlayerActionMap.Enable();
    }
}