using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ResourceManager
{
    /// <summary>
    /// Resources 폴더 내 자료를 Hierarchy창에 올린다.
    /// 만일 Object라면 이미 Pool에 올라와 있는 경우 Pool에서
    /// 꺼내 쓰도록 한다.
    /// </summary>
    /// <param name="path">
    /// Resources 폴더 내의 경로를 의미한다.
    /// </param>
    /// <typeparam name="T">
    /// UnityEngine의 Object 클래스에 한해서만 받는다
    /// </typeparam>
    /// <returns></returns>
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);
            
            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }

        return Resources.Load<T>(path);
    }

    /// <summary>
    /// Prefabs 폴더에 존재하는 프리팹을 인스턴스화 한다.
    /// </summary>
    /// <remarks> PUN2의 PhotonNetwork.Instantiate가 아닌 Resources.Instantiatea를 사용하였다.</remarks>
    /// <param name="path">Resources/Prefab 폴더 내의 경로를 의미한다.</param>
    /// <param name="position">생성할 프리팹의 위치를 의미한다</param>
    /// <param name="rotation">생성할 프리팹의 rotation을 의미한다</param>
    /// <param name="parent">만일 어떤 객체의 자식을 할당할 것이라면 그 부모가 될 객체이다.</param>
    /// <returns></returns>
    public GameObject Instantiate(string path, Vector3 position, Quaternion rotation ,Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        // TODO : PhotonNetwork는 자체적으로 Pool을 사용하므로 추후에 문제가 없는지 확인할 것
        GameObject go = Object.Instantiate(original, position, rotation, parent);
        // GameObject go = PhotonNetwork.Instantiate($"Prefabs/{path}", position, rotation);
        go.name = original.name;
        return go;
    }
    
    /// <summary>
    /// 객체가 Pool
    /// </summary>
    /// <param name="go">
    /// 파괴할 객체를 의미한다.
    /// </param>
    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}