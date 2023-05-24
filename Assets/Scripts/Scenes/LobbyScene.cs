using System.Collections.Generic;
using Photon.Pun; // 유니티용 포톤 컴포넌트들
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class LobbyScene : BaseScene
{
    private string gameVersion = "1"; // 게임 버전
    public Text connectionInfoText; // 네트워크 정보를 표시할 텍스트
    public Button joinButton; // 로비 접속 버튼
    public TMP_InputField inputfield;
    private List<RoomInfo> _roomList = new(); // 룸 접속 버튼
    public List<Button> roomButtons; // 룸 접속 버튼
    
    // 게임 실행과 동시에 마스터 서버 접속 시도
    private void Start() {
        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = gameVersion;
        // 설정한 정보를 가지고 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();

        // 룸 접속 버튼을 잠시 비활성화
        foreach(Button btn in roomButtons)
            btn.interactable = false;
        joinButton.interactable = false;
        // 접속을 시도 중임을 텍스트로 표시
        connectionInfoText.text = "마스터 서버에 접속중...";
    }
    
    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster() {
        // 접속 정보 표시
        connectionInfoText.text = "온라인 : 마스터 서버와 연결됨\n";
        // 그대로 로비로 접속
        PhotonNetwork.JoinLobby();
    }
    
    // 룸 접속 시도
    public void CreateRoomButton() {
        // 중복 접속 시도를 막기 위해, 접속 버튼 잠시 비활성화
        joinButton.interactable = false;

        // 마스터 서버에 접속중이라면
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.CreateRoom(inputfield.text);
            connectionInfoText.text = "온라인 : 마스터 서버와 연결됨\n새로운 방 생성...";
        }
        else
        {
            // 마스터 서버에 접속중이 아니라면, 마스터 서버에 접속 시도
            connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
            // 마스터 서버로의 재접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    
    // 룸 접속 시도
    public void Connect() {
        // 중복 접속 시도를 막기 위해, 접속 버튼 잠시 비활성화
        foreach (Button btn in roomButtons)
            btn.interactable = false;
        joinButton.interactable = false;

        // 마스터 서버에 접속중이라면
        if (PhotonNetwork.IsConnected)
        {
            // 로비로 접속
            PhotonNetwork.JoinLobby();
        }
        else
        {
            // 마스터 서버에 접속중이 아니라면, 마스터 서버에 접속 시도
            connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
            // 마스터 서버로의 재접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinedLobby()
    {
        foreach(Button btn in roomButtons)
            btn.interactable = true;
        // 방 생성 버튼을 활성화
        joinButton.interactable = true;
        // 접속 정보 표시
        connectionInfoText.text = "온라인 : 로비로 접속함";
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; ++i)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!_roomList.Contains(roomList[i])) _roomList.Add(roomList[i]);
                else _roomList[_roomList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (_roomList.IndexOf(roomList[i]) != -1 ) _roomList.RemoveAt(_roomList.IndexOf(roomList[i]));
        }
        myRoomListUpdate();
    }

    private void myRoomListUpdate()
    {
        int maxCount = Mathf.Min(_roomList.Count, roomButtons.Count);
        for (int i = 0; i < maxCount; i++)
        {
            TextMeshProUGUI tmp = roomButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = _roomList[i].Name;
            }
        }
    }

    public void OnConnectRoomButton(Button button)
    {
        foreach (Button btn in roomButtons)
            btn.interactable = false;
        joinButton.interactable = false;
        
        PhotonNetwork.JoinRoom(button.GetComponentInChildren<TextMeshProUGUI>().text);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // 접속 상태 표시
        connectionInfoText.text = "방 입장 실패...";
        
        foreach (Button btn in roomButtons)
            btn.interactable = true;
        joinButton.interactable = true;
    }

    // // 룸 접속 시도
    // public void Connect() {
    //     // 중복 접속 시도를 막기 위해, 접속 버튼 잠시 비활성화
    //     joinButton.interactable = false;
    //
    //     // 마스터 서버에 접속중이라면
    //     if (PhotonNetwork.IsConnected)
    //     {
    //         // 룸 접속 실행
    //         connectionInfoText.text = "룸에 접속...";
    //         PhotonNetwork.JoinRandomRoom();
    //     }
    //     else
    //     {
    //         // 마스터 서버에 접속중이 아니라면, 마스터 서버에 접속 시도
    //         connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
    //         // 마스터 서버로의 재접속 시도
    //         PhotonNetwork.ConnectUsingSettings();
    //     }
    // }
    //
    
    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message) {
        // 접속 상태 표시
        connectionInfoText.text = "빈 방이 없음, 새로운 방 생성...";
        // 최대 4명을 수용 가능한 빈방을 생성
        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = 4});
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom() {
        // 접속 상태 표시
        connectionInfoText.text = "방 참가 성공";
        Managers.Network.AllocateViewId();
        // 모든 룸 참가자들이 Game 씬을 로드하게 함
        Managers.Scene.LoadLevel(Define.Scene.Game);
    }
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Lobby;
    }
    public override void Clear()
    {
        
    }
}
