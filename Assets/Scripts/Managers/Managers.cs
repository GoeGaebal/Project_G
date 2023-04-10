using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{
    private static Managers s_instance; // 유일성이 보장된다
    private static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다
    
    #region Content
    private MapManager _map = new MapManager();
    private ObjectManager _object = new ObjectManager();
    private TimeSlotManager _timeSlot = new TimeSlotManager();
    public static MapManager Map { get { return Instance._map; } }
    public static ObjectManager Object { get { return Instance._object; } }
    public static TimeSlotManager TimeSlot { get { return Instance._timeSlot;} }
    #endregion

    #region Core
    private ResourceManager _resource = new();
    private SceneManagerEx _scene = new();
    private PoolManager _pool = new();
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static PoolManager Pool { get { return Instance._pool; } }

    #endregion
    
    void Start()
    {
        Init();
    }
    
    void Update()
    {
        TimeSlot.AddDelataTime(Time.deltaTime);
    }
    
    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
            
            s_instance._pool.Init();
        }
    }
    
    public static void Clear()
    {
        Scene.Clear();
        Pool.Clear();
    }

}
