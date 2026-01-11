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
    public TMP_InputField roomName; // 방 이름 입력 필드
    public GameObject roomItem;     // RoomItem 프리팹 (방 버튼)
    public GameObject scrollContents; // Scroll View 안에 RoomItem들이 들어갈 부모 오브젝트

    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>(); // 현재 로비에 표시된 방 목록

    private void Awake()
    {
        PhotonNetwork.GameVersion = "v1.0";

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // 서버 연결
        }

        // 유저 닉네임 설정 (PlayerPrefs에서 불러오거나 랜덤 생성)
        PhotonNetwork.NickName = PlayerPrefs.GetString("USER_ID", "USER_" + Random.Range(0, 999).ToString("000"));

        // 방 이름 초기값 설정
        roomName.text = "Room_" + Random.Range(0, 999).ToString("000");
    }

    void Start()
    {
        // BGM이나 초기 UI 세팅 가능 (현재 생략됨)
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

        // 랜덤 입장 실패 시, 기본 방 생성 (개발용)
        PhotonNetwork.CreateRoom("My Room", new RoomOptions { MaxPlayers = 3 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Enter Room");

        // 방 입장 후 TeamLobby 씬으로 전환
        StartCoroutine(LoadBattleField());
    }

    IEnumerator LoadBattleField()
    {
        PhotonNetwork.IsMessageQueueRunning = false;

        // 팀 선택/준비 UI가 있는 씬으로 이동
        AsyncOperation ao = SceneManager.LoadSceneAsync("TeamLobby");
        yield return ao;
    }

    // 랜덤 방 입장 버튼 클릭 시 호출
    public void OnClickJoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom(); // 랜덤 입장 시도
    }

    // 방 만들기 버튼 클릭 시 호출
    public void OnClickCreateRoom()
    {
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

                    RoomData roomData = room.GetComponent<RoomData>();
                    roomData.roomName = roomInfo.Name;
                    roomData.connectPlayer = roomInfo.PlayerCount;
                    roomData.maxPlayer = roomInfo.MaxPlayers;
                    roomData.DispRoomData();

                    // 방 클릭 시 입장 시도
                    room.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        OnClickRoomItem(roomData.roomName);
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
        PhotonNetwork.JoinRoom(roomName);
    }

    // 디버깅용: 현재 상태 표시
    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }
}
