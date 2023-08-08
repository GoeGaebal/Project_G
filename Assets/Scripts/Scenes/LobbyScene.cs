using System.Collections.Generic;
using Photon.Pun; // 유니티용 포톤 컴포넌트들
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Button = UnityEngine.UI.Button;

public class LobbyScene : BaseScene
{
    private string gameVersion = "1"; // 게임 버전
    public Text connectionInfoText; // 네트워크 정보를 표시할 텍스트
    public Button joinButton; // 로비 접속 버튼
    public TMP_InputField inputfield;
    private List<RoomInfo> _roomList = new(); // 룸 접속 버튼
    public List<Button> roomButtons; // 룸 접속 버튼
    private UI_Start _startScreen;
    private UI_Lobby _lobbyScreen;
    private UI_CreateRoomSetting _createRoomSettingPopup;
    private UI_Room _roomScreen;
    
    private void Start() {
    }

    public void ConnectToMaster()
    {
        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = gameVersion;
        // 설정한 정보를 가지고 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();
        _startScreen._loadingText.text = "마스터 서버에 접속중...";
        _startScreen.LoadingIcon.SetActive(true);
        _startScreen.SetInteractableButtons(false);
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster() {
        // 접속 정보 표시
        _startScreen._loadingText.text  = "온라인 : 마스터 서버와 연결됨\n";
        // 그대로 로비로 접속
        PhotonNetwork.JoinLobby();
    }
    
    // 룸 접속 시도
    public void CreateRoom() {
        // 마스터 서버에 접속중이라면
        _createRoomSettingPopup.SetInteractableButtons(false);
        if (PhotonNetwork.IsConnected)
        {
            string roomNameText = _createRoomSettingPopup.RoomName.text;
            string userNameText = _createRoomSettingPopup.UserName.text;
            
            if (roomNameText.Length < 2)
                _createRoomSettingPopup.WarningText.text = "주의: 방 이름은 최소 두 글자 이상이어야 합니다.";
            else if (userNameText.Length < 2)
                _createRoomSettingPopup.WarningText.text = "주의: 유저 이름은 최소 두 글자 이상이어야 합니다.";
            else
            {
                foreach (var _roomInfo in _roomList)
                {
                    if (_roomInfo.Name == roomNameText)
                    {
                        _createRoomSettingPopup.WarningText.text = "주의: 방 이름이 겹칩니다";
                        _createRoomSettingPopup.SetInteractableButtons(true);
                        return;
                    }
                }
                
                PhotonNetwork.CreateRoom(roomNameText, new RoomOptions(){ MaxPlayers = 4});
                _createRoomSettingPopup.LoadingSet.SetActive(true);
            }
        }
        else
        {
            // 마스터 서버에 접속중이 아니라면, 마스터 서버에 접속 시도
            _createRoomSettingPopup.WarningText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
            // 마스터 서버로의 재접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
        _createRoomSettingPopup.SetInteractableButtons(true);
    }
    
    // 룸 접속 시도
    public void Connect() {
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
        if (_lobbyScreen == null)
        {
            _lobbyScreen = Managers.UI.ShowPopupUI<UI_Lobby>();
            InitLobbyScreen();
        }
        _startScreen.SetInteractableButtons(true);
        _startScreen._loadingText.text  = "";
        _startScreen.LoadingIcon.SetActive(false);
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
        _lobbyScreen.LoadingPane.SetActive(true);
        if (_roomList.Count > _lobbyScreen.RoomList.Count)
        {
            int count = _roomList.Count - _lobbyScreen.RoomList.Count;
            for (int i = 0; i < count; i++)
            {
                var room = _lobbyScreen.AddRoom();
                room.Init();
                room.RoomBtn.onClick.AddListener(() => { PhotonNetwork.JoinRoom(room.Name.text);});
            }
        }
        else if (_roomList.Count < _lobbyScreen.RoomList.Count)
        {
            int count = _lobbyScreen.RoomList.Count - _lobbyScreen.RoomList.Count;
            for(int i = 0; i < count; i++)
                _lobbyScreen.RemoveRoom();
        }

        for (int i = 0; i < _roomList.Count; i++)
        {
            _lobbyScreen.RoomList[i].Name.text = _roomList[i].Name;
            _lobbyScreen.RoomList[i].NoP.text = $"{_roomList[i].PlayerCount}/4";
        }
        _lobbyScreen.LoadingPane.SetActive(false);
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
        // 최대 4명을 수용 가능한 빈 방을 생성
        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = 4});
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom() {
        
        Managers.Network.AllocateViewId();
        if (_createRoomSettingPopup != null)
        {
            PhotonNetwork.LocalPlayer.NickName = _createRoomSettingPopup.UserName.text;
            Managers.UI.ClosePopupUI(_createRoomSettingPopup);
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = "Guest";
        }
        // _roomScreen = Managers.UI.ShowPopupUI<UI_Room>();
        Managers.Network.SpawnLocalPlayer();
        Managers.Map.LoadMap(5);
        // InitRoomScreen();
        // // 모든 룸 참가자들이 Game 씬을 로드하게 함
        // Managers.Scene.LoadLevel(Define.Scene.Ship);
    }

    public override void OnLeftRoom()
    {
        Managers.Map.DestoryMap();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
            _roomScreen.Friends[i].Name.text = PhotonNetwork.PlayerListOthers[i].NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
            _roomScreen.Friends[i].Name.text = PhotonNetwork.PlayerListOthers[i].NickName;
    }

    private void InitRoomScreen()
    {
        if (_roomScreen == null)
        {
            Debug.Log("방 화면 오류");
            return;
        }
        _roomScreen.Init();
        _roomScreen.MyName.text = PhotonNetwork.LocalPlayer.NickName;
        _roomScreen.StartBtn.onClick.RemoveAllListeners();
        _roomScreen.ExitBtn.onClick.RemoveAllListeners();
        _roomScreen.StartBtn.onClick.AddListener(() =>
        {
            Managers.Scene.LoadScene(Define.Scene.Ship);
        });
        _roomScreen.ExitBtn.onClick.AddListener(() =>
        {
            PhotonNetwork.LeaveRoom();
            Managers.UI.ClosePopupUI(_roomScreen);
        });
        
        for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
            _roomScreen.Friends[i].Name.text = PhotonNetwork.PlayerListOthers[i].NickName;
    }
    
    private void InitLobbyScreen()
    {
        if (_lobbyScreen == null)
        {
            Debug.Log($"로비화면 오류");
            return;
        }

        _lobbyScreen.Init();
        _lobbyScreen.CreateBtn.onClick.RemoveAllListeners();
        _lobbyScreen.CreateBtn.onClick.AddListener(() =>
        {
            _createRoomSettingPopup = Managers.UI.ShowPopupUI<UI_CreateRoomSetting>();
            _createRoomSettingPopup.Init();
            
            _createRoomSettingPopup.CreateRoomBtn.onClick.RemoveAllListeners();
            _createRoomSettingPopup.CreateRoomBtn.onClick.AddListener(CreateRoom);
        });
        _lobbyScreen.RedoBtn.onClick.RemoveAllListeners();
        _lobbyScreen.RedoBtn.onClick.AddListener(() =>
        {
            PhotonNetwork.Disconnect();
            Managers.UI.ClosePopupUI(_lobbyScreen);
        });
    }
    
    private void InitStartScreen()
    {
        _startScreen.Init();
    }

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Lobby;
        _startScreen = Managers.UI.ShowSceneUI<UI_Start>();
    }
    public override void Clear()
    {
        
    }
}
