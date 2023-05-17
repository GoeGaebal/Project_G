using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private int _order = 10;
    
    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    private UI_Scene _sceneUI = null;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject() { name = "@UI_Root" };
            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;  // 부모 캔버스가 존재하더라도 독립적인 sortingOrder를 갖는다.

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        
        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}", Vector3.zero, Quaternion.identity);
        
        if (parent != null)
            go.transform.SetParent(parent);
        
        return Util.GetOrAddComponent<T>(go);
    }
    
    /// <summary>
    /// UI_Scene 객체를 생성하는 함수
    /// </summary>
    /// <remarks>만일 생성할 클래스의 이름과 실제 프리팹 이름이 같다면 이름을 변수로 넣을 필요가 없다</remarks>>
    /// <param name="name">보여줄 UI_Scene 이름</param>
    /// <typeparam name="T">UI_Scene을 상속받는 모든 클래스</typeparam>
    /// <returns></returns>
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        
        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}", Vector3.zero, Quaternion.identity);
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);
        
        return sceneUI;
    }

    /// <summary>
    /// UI_Popup 객체를 생성하고 스택에 삽입하는 함수
    /// </summary>
    /// <remarks>만일 생성할 클래스의 이름과 실제 프리팹 이름이 같다면 이름을 변수로 넣을 필요가 없다</remarks>>
    /// <param name="name">보여줄 UI_Popup을 이름</param>
    /// <typeparam name="T">UI_Popup을 상속받는 모든 클래스</typeparam>
    /// <returns></returns>
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        
        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}", Vector3.zero, Quaternion.identity);
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);
        
        
        
        go.transform.SetParent(Root.transform);
        
        return popup;
    }
    
    /// <summary>
    /// UI_Popup 객체를 파괴할 때, 현재 객체가 가장 최상위에 존재하는 지 확인하고 지우는 함수
    /// </summary>
    /// <param name="popup">매니저의 UI_Popup 스택 중에 현재 가장 최상위 객체로 판단되는 객체</param>
    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }
        
        ClosePopupUI();
    }

    /// <summary>
    /// 스택에서 최상위에 존재하는 UI_Popup 객체를 삭제하는 함수
    /// </summary>
    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 스택에 존재하는 모든 UI_Popup 객체를 삭제하는 함수
    /// </summary>
    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
    }
}
