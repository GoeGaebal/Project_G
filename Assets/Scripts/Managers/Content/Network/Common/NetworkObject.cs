using System;
using Google.Protobuf.Protocol;
using Photon.Pun;
using UnityEngine;

public class NetworkObject : MonoBehaviour
{
    public Google.Protobuf.Protocol.ObjectInfo Info { get; set; } = new();
    public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
    public int Id
    {
        get { return Info.ObjectId; }
        set { Info.ObjectId = value; }
    }

    public PositionInfo PosInfo
    {
        get
        {
            Info.PosInfo ??= new PositionInfo();
            return Info.PosInfo;
        }
        private set => Info.PosInfo = value;
    }

    public GameRoom Room { get; set; }

    public virtual void SyncPos()
    {
        var t = transform;
        t.position = new Vector3(PosInfo.PosX, PosInfo.PosY);
    }
}