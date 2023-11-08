using Photon.Pun;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance; // 유일성이 보장된다
    private static Managers Instance { get { Init(); return _instance; } } // 유일한 매니저를 갖고온다

    #region PUN2
    private static PhotonView _photonView;
    private static PhotonView ManagersPhotonView { get { return _photonView; } } // 유일한 포톤뷰를 들고 온다.
    
    
    #endregion


    #region Content
    private readonly MapManager _map = new();
    private readonly ItemManager _item = new();
    private readonly DataManager _data = new();
    private readonly ObjectManager _object = new();
    private readonly TimeSlotManager _timeSlot = new();
    private readonly UIManager _ui = new();
    private readonly InputManager _input = new();
    private readonly SoundManager _sound = new();
    private readonly ArtifactManager _artifact = new();
    private readonly WorldMapManager _worldMap = new();
    private readonly WeatherManager _weather = new();
    private NetworkManager _network = new();

    public static MapManager Map => Instance._map;
    public static ItemManager Item => Instance._item;
    public static DataManager Data => Instance._data;
    public static ObjectManager Object => Instance._object;
    public static TimeSlotManager TimeSlot => Instance._timeSlot;
    public static UIManager UI => Instance._ui;
    public static InputManager Input => Instance._input;
    public static SoundManager Sound => Instance._sound;
    public static ArtifactManager Artifact => Instance._artifact;
    public static WeatherManager Weather => Instance._weather;
    public static WorldMapManager WorldMap => Instance._worldMap;
    public static NetworkManager Network => Instance._network;

    #endregion

    #region Core
    private readonly ResourceManager _resource = new();
    private readonly SceneManagerEx _scene = new();
    private readonly PoolManager _pool = new();
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static PoolManager Pool => Instance._pool;

    #endregion
    
    void Start()
    {
        Init();
    }
    
    void Update()
    {
        Network.Server.Update();
        while (Network.Server.JobQueue.Count > 0)
        {
            Network.Server.JobQueue.Dequeue().Invoke();
        }
        Network.Client.Update();
        if(Managers.Network.IsHost) TimeSlot.AddDelataTime(Time.deltaTime);
       WorldMap.UpdateWorldMap(Time.deltaTime);
       Scene.WaitLoading(Time.deltaTime);
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

            _instance._network.Init();
            _instance._input.Init();
            _instance._item.Init();
            _instance._data.Init();
            _instance._pool.Init();
            // _instance._object.Init();
            _instance._timeSlot.Init();
            _instance._sound.Init();
            _instance._artifact.Init();
            _instance._weather.Init();
            _instance._worldMap.Init();

            // _network.Server.Init();
            //_network.Client.Init();
        }
    }

    public static void MakeDontDestroyOnLoad(GameObject go)
    {
        DontDestroyOnLoad(go);
    }
    
    public static void Clear()
    {
        Object.Clear();
        Scene.Clear();
        Pool.Clear();
    }
}
