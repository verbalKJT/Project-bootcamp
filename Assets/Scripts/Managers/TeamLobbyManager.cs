using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using ExitGames.Client.Photon; // Hashtable 사용을 위해 추가

public class TeamLobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_Text[] playerNameTexts; // Player1 ~ Player3 UI 텍스트
    public TMP_Text textLogMsg;        // 입장/퇴장 로그 텍스트 영역
    public GameObject startGameButton; // 방장만 보이는 시작 버튼
    public GameObject readyButton;     // 일반 유저용 Ready 버튼
    public GameObject exitButton;
    private PhotonView pv;
    // 플레이어의 준비 상태를 저장할 키값
    private const string IS_READY = "IsReady";
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    void Start()
    {
        pv = GetComponent<PhotonView>();
        // 방장/참여자 버튼 UI 초기화
        UpdateRoomUI();
        
        UpdatePlayerList();
        ShowEnterLog(PhotonNetwork.NickName);
    }
    public override void OnJoinedRoom()
    {
        SetReadyStatus(false); // 방에 들어오면 내 준비 상태를 초기화 (False)
    }
    // 방장 변경, 입장/퇴장 시 UI 상태 업데이트
    void UpdateRoomUI()
    {
        bool isMaster = PhotonNetwork.IsMasterClient;

        // 1. 방장일 경우: 시작 버튼 보임, 준비 버튼 숨김
        // 2. 참여자일 경우: 시작 버튼 숨김, 준비 버튼 보임
        if (startGameButton) startGameButton.SetActive(isMaster);
        if (readyButton) readyButton.SetActive(!isMaster);

        // 방장이라면 현재 준비 상태 체크해서 버튼 활성화 여부 결정
        if (isMaster)
        {
            CheckAllPlayersReady();
        }
    }
    // 접속한 플레이어 UI 표시
    void UpdatePlayerList() 
    {
        Player[] players = PhotonNetwork.PlayerList; // 방에 접속한 플레이어 배열로 가져옴

        for (int i = 0; i < playerNameTexts.Length; i++) 
        {
            if (i < players.Length)
            {
                string nickname = players[i].NickName;
                // (선택사항) 이름 옆에 준비 상태 텍스트 표시
                // 방장이 아니면서 준비 완료된 상태면 (Ready) 표시 추가
                object isReadyVal;
                bool isReady = false;
                if(players[i].CustomProperties.TryGetValue(IS_READY, out isReadyVal))
                {
                    isReady = (bool)isReadyVal;
                }

                if (!players[i].IsMasterClient && isReady)
                {
                    nickname += " <color=green>(Ready)</color>";
                }
                playerNameTexts[i].text = nickname;
            }
            else // 접속안 한 자리는 빈칸 표시
            {
                playerNameTexts[i].text = " ";
            }
        }
    }

    void ShowEnterLog(string nickname)
    {
        string msg = $"\n<color=#ff0000>[{nickname}]'s Legend Entered Room!</color>";
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);
    }

    void ShowExitLog(string nickname)
    {
        string msg = $"\n<color=#ff0000>[{nickname}]'s Legend Out!!</color>";
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);
    }

    // RPC로 로그 출력
    [PunRPC]
    void LogMsg(string msg)
    {
        textLogMsg.text += msg;
    }
    public void LogMessageRPC(string msg)
    {
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) // 새로운 플레이어가 룸 접속했을 때 
    {
        UpdatePlayerList();
        ShowEnterLog(newPlayer.NickName);
        // 누군가 들어오면 방장의 시작 버튼을 다시 검사 (새로 온 사람은 준비 안 된 상태이므로 버튼 비활성화 될 것임)
        if (PhotonNetwork.IsMasterClient) CheckAllPlayersReady();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer) // 플레이어가 룸에서 나갔을 때
    {
        UpdatePlayerList();
        ShowExitLog(otherPlayer.NickName);
        // 누군가 나가면 남은 인원 기준으로 다시 검사
        if (PhotonNetwork.IsMasterClient) CheckAllPlayersReady();
    }
    // 플레이어의 커스텀 프로퍼티(준비 상태 등)가 변경되면 호출되는 콜백
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // 준비 상태가 변경된 것이라면
        if (changedProps.ContainsKey(IS_READY))
        {
            UpdatePlayerList(); // (선택사항) 이름 옆에 준비 표시를 하고 싶다면 여기서 갱신
            
            // 방장이라면 모든 인원이 준비되었는지 확인
            if (PhotonNetwork.IsMasterClient)
            {
                CheckAllPlayersReady();
            }
        }
    }
    // 모든 플레이어가 준비되었는지 확인하는 로직 (방장용)
    void CheckAllPlayersReady()
    {
        bool allReady = true;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            // 방장은 준비 검사에서 제외 (혹은 방장도 준비가 필요하면 조건 제거)
            if (p.IsMasterClient) continue;

            // 플레이어의 프로퍼티에서 상태 가져오기 (없으면 false)
            object isReadyValue;
            if (p.CustomProperties.TryGetValue(IS_READY, out isReadyValue))
            {
                if (!(bool)isReadyValue)
                {
                    allReady = false;
                    break;
                }
            }
            else
            {
                // 프로퍼티가 세팅 안된 유저가 있다면 준비 안 된 것으로 간주
                allReady = false;
                break;
            }
        }

        // 혼자일 때는 바로 시작 가능하게 할지, 아니면 혼자라도 준비가 필요한지 정책에 따라 결정
        // 여기서는 "다른 플레이어가 없으면(혼자면) 시작 버튼 활성화"로 가정
        if (PhotonNetwork.PlayerList.Length == 1) allReady = true;

        // 시작 버튼의 상호작용(클릭 가능 여부) 설정
        Button btn = startGameButton.GetComponent<Button>();
        if (btn != null)
        {
            btn.interactable = allReady;
        }
    }
    public void OnClickStartGame()
    {
        // 마스터 클라이언트만 실행
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            // 모든 클라이언트가 게임씬으로 전환
            PhotonNetwork.LoadLevel("InGame");
        }
    }
    public void OnClickReady()
    {
        bool isReady = false;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(IS_READY, out object value))
        {
            isReady = (bool)value;
        }
        SetReadyStatus(!isReady);
    }
    // 내 준비 상태를 네트워크에 설정하는 함수
    void SetReadyStatus(bool ready)
    {
        Hashtable props = new Hashtable() { { IS_READY, ready } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        
        // (선택사항) 준비 버튼 텍스트를 "Ready" <-> "Cancel"로 바꾸고 싶다면 여기서 처리
        if(readyButton != null)
        {
            TMP_Text btnText = readyButton.GetComponentInChildren<TMP_Text>();
            if(btnText) btnText.text = ready ? "Cancel" : "Ready";
        }
    }
    // 방장이 바뀌었을 때 UI 재설정 (예: 내가 방장이 되면 준비버튼->시작버튼으로 교체)
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        UpdateRoomUI();
    }
    public void OnClickExitRoom()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsConnectedAndReady)
        {
            exitButton.SetActive(false); // 중복 클릭 방지
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Debug.LogWarning("Cannot leave room: not in room or not ready.");
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby"); // 나가면 로비로 복귀
    }
}
