using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
		if (component == null)
            component = go.AddComponent<T>();
        return component;
	}

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;
        
        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
		}
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static ulong Vector2ulong(Vector3 pos)
    {
        int y = (int) pos.y;
        int x = (int) pos.x;
        // return ((ulong)(uint)y << Define.INT_SIZE_IN_BITS) | (uint)x;
        return BitConverter.ToUInt64(BitConverter.GetBytes(y).Concat(BitConverter.GetBytes(x)).ToArray(), 0);
    }
    
    public static Vector2 Ulong2Vector(ulong val)
    {
        byte[] byteArray = BitConverter.GetBytes(val);
        int y = BitConverter.ToInt32(byteArray, 0);
        int x = BitConverter.ToInt32(byteArray, 4);
        return new Vector2(x:x,y:y);
    }
}
