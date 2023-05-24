using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviourPun
{
    private static Managers _instance; // 유일성이 보장된다
    private static Managers Instance { get { Init(); return _instance; } } // 유일한 매니저를 갖고온다
    
    #region Content
    private MapManager _map = new();
    private DataManager _data = new();
    private ObjectManager _object = new();
    private NetworkManager _network = new();
    private TimeSlotManager _timeSlot = new();
    private UIManager _ui = new();
    private InputManager _input = new();
    public static MapManager Map { get { return Instance._map; } }
    public static DataManager Data { get { return Instance._data; } }
    public static ObjectManager Object { get { return Instance._object; } }
    public static NetworkManager Network { get { return Instance._network; } }
    public static TimeSlotManager TimeSlot { get { return Instance._timeSlot;} }
    public static UIManager UI { get { return Instance._ui; } }
    public static InputManager Input { get { return Instance._input;} }
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
       if(PhotonNetwork.IsMasterClient) TimeSlot.AddDelataTime(Time.deltaTime);
    }
    
    static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();

            _instance._input.Init();
            _instance._data.Init();
            _instance._pool.Init();
            _instance._object.Init();
            _instance._network.Init();
            _instance._timeSlot.Init();
        }
    }
    
    public static void Clear()
    {
        Scene.Clear();
        Pool.Clear();
    }
}
