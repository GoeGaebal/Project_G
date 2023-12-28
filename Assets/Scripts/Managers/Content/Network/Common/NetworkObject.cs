using Google.Protobuf.Protocol;
using UnityEngine;

public class NetworkObject : MonoBehaviour
{
    public ObjectInfo Info { get; set; } = new();
    public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
    
    public int Id
    {
        get => Info.ObjectId;
        set => Info.ObjectId = value;
    }

    public PositionInfo PosInfo
    {
        get => Info.PosInfo;
        private set => Info.PosInfo = value;
    }

    public StatInfo StatInfo
    {
        get => Info.StatInfo;
        private set => Info.StatInfo = value;
    }

    public GameRoom Room { get; set; }

    protected virtual void Awake()
    {
        PosInfo ??= new PositionInfo();
        StatInfo ??= new StatInfo();
    }

    public virtual void SyncPos()
    {
        var t = transform;
        t.position = new Vector3(PosInfo.PosX, PosInfo.PosY);
        if (PosInfo.LocalScale != 0) t.localScale = PosInfo.LocalScale * Vector3.one;
    }
}