using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class Extension
{
	public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
	{
		return Util.GetOrAddComponent<T>(go);
	}

	public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
	{
	 	UI_Base.AddUIEvent(go, action, type);
	}

	public static void AddEvent(this InputAction ia, Action<InputAction.CallbackContext> action)
	{
		ia.started -= action;
		ia.performed -= action;
		ia.canceled -= action;
		ia.started += action;
		ia.performed += action;
		ia.canceled += action;
	}

	// public static bool IsValid(this GameObject go)
	// {
	// 	return go != null && go.activeSelf;
	// }
}
