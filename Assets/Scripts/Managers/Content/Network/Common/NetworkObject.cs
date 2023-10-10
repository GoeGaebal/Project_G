using Google.Protobuf.Protocol;
using Photon.Pun;
using UnityEngine;

public class NetworkObject : MonoBehaviourPun
{
    public Google.Protobuf.Protocol.ObjectInfo Info { get; set; } = new();
    public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
    public int Id
    {
        get { return Info.ObjectId; }
        set { Info.ObjectId = value; }
    }
    public PositionInfo PosInfo { get; private set; } = new PositionInfo();
    public GameRoom Room { get; set; }

    public void SyncPos()
    {
        if (Info.PosInfo != null) PosInfo = Info.PosInfo;
        transform.position = new Vector3(PosInfo.PosX, PosInfo.PosY);
    }
}