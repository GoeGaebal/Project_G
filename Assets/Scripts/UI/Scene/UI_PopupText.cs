using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_PopupText : UI_Scene
{
    private Dictionary<Transform, UI_PopupNameItem> _nameItems;
    private UI_PopupNameItem[] _nameItemList;
}
