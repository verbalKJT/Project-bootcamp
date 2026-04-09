using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;

// Photon 서버 초기화 및 로비 UI 기능 담당
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField userIdInput;
    public TMP_InputField roomName; 
    public GameObject roomItem;     
    public GameObject scrollContents; 

    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>(); // 현재 로비에 표시된 방 목록

    private void Awake()
    {
        // 기존 게임 버전 정하는 PhotonNetwork.GameVersion = "v1.0"; 삭제

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // 서버 연결
        }

        // 유저 닉네임 설정 (닉네임 설정 안할 시 임의 닉네임 설정 -> Player 1~999)
        string nickname = PlayerPrefs.GetString("USER_ID", "Player_" + Random.Range(1, 999));
        PhotonNetwork.NickName = nickname;

        if (userIdInput != null)
            userIdInput.text = nickname; // UI에도 기본 닉네임 표시

        // 방 이름 초기값 설정
        roomName.text = "Room_" + Random.Range(0, 999).ToString("000");
    }

    void Start()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(); // 마스터 서버 연결 후 로비 입장
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Entered Team Lobby");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No rooms!!");

        // 랜덤 입장 실패 시, 기본 방 생성 
        PhotonNetwork.CreateRoom("My Room", new RoomOptions { MaxPlayers = 3 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Enter Room");

        // 방 입장 후 TeamLobby 씬으로 전환
        PhotonNetwork.LoadLevel("TeamLobby");
    }

    // 랜덤 방 입장 버튼 클릭 시 호출
    public void OnClickJoinRandomRoom()
    {
        SavePlayerName(); // 닉네임 저장
        PhotonNetwork.JoinRandomRoom(); // 랜덤 입장 시도
    }

    // 방 만들기 버튼 클릭 시 호출
    public void OnClickCreateRoom()
    {
        SavePlayerName(); // 닉네임 저장
        
        string _roomName = roomName.text;
        if (string.IsNullOrEmpty(_roomName))
        {
            _roomName = "Room_" + Random.Range(0, 999).ToString("000");
        }
        
        RoomOptions roomOptions = new RoomOptions
        {
            IsOpen = true,
            IsVisible = true,
            MaxPlayers = 3
        };

        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 만들기 실패 : " + message);
    }
    // 닉네임 저장
    void SavePlayerName()
    {
        string inputName = userIdInput.text;

        if (string.IsNullOrEmpty(inputName))
            inputName = "Player_" + Random.Range(1, 999);

        PlayerPrefs.SetString("USER_ID", inputName);
        PlayerPrefs.Save();

        PhotonNetwork.NickName = inputName;
    }
    // 방 목록 업데이트 이벤트
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach (var roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
            {
                // 방이 사라졌을 경우 기존 RoomItem 제거
                if (rooms.TryGetValue(roomInfo.Name, out tempRoom))
                {
                    Destroy(tempRoom);
                    rooms.Remove(roomInfo.Name);
                }

                // UI 정렬 갱신
                scrollContents.GetComponent<GridLayoutGroup>().constraintCount = rooms.Count;
                scrollContents.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 20);
            }
            else
            {
                // 새로 생긴 방 추가
                if (!rooms.ContainsKey(roomInfo.Name))
                {
                    GameObject room = Instantiate(roomItem);
                    room.transform.SetParent(scrollContents.transform, false);
                    
                    // Photon에서 받은 정보 세팅
                    RoomData roomData = room.GetComponent<RoomData>();
                    roomData.roomName = roomInfo.Name;
                    roomData.connectPlayer = roomInfo.PlayerCount;
                    roomData.maxPlayer = roomInfo.MaxPlayers;
                    roomData.DispRoomData(); // UI 텍스트에 표시

                    // 방 클릭 시 입장 시도
                    room.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        OnClickRoomItem(roomData.roomName); // 방 이름 넘겨서 입장
                    });

                    rooms.Add(roomInfo.Name, room);
                    scrollContents.GetComponent<GridLayoutGroup>().constraintCount = rooms.Count;
                    scrollContents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 20);
                }
                else
                {
                    // 기존 방 정보 갱신
                    if (rooms.TryGetValue(roomInfo.Name, out tempRoom))
                    {
                        RoomData roomData = tempRoom.GetComponent<RoomData>();
                        roomData.roomName = roomInfo.Name;
                        roomData.connectPlayer = roomInfo.PlayerCount;
                        roomData.maxPlayer = roomInfo.MaxPlayers;
                        roomData.DispRoomData();
                    }
                }
            }
        }
    }
    // RoomItem 버튼 클릭 시 호출 → 방 입장
    void OnClickRoomItem(string roomName)
    {
        SavePlayerName(); // 닉네임 저장
        PhotonNetwork.JoinRoom(roomName);
    }

    // 디버깅용: 현재 상태 표시
    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }
}
