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
    
    public Image[] playerCharacterImages; // Player 1~3의 캐릭터 이미지 UI
    public Sprite[] characterSprites;     // FireMan, StoneMan, GrassMan
    
    private PhotonView pv;
    // 플레이어의 준비 상태를 저장할 키값
    private const string IS_READY = "IsReady";
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // 씬 자동 동기화 설정
    }
    void Start()
    {
        pv = GetComponent<PhotonView>();
        // 방장/참여자 버튼 UI 초기화
        UpdateRoomUI();
        // 플레이어 리스트 UI 업데이트
        UpdatePlayerList();
        UpdateCharacterImages();
    }
    public override void OnJoinedRoom()
    {
        SetReadyStatus(false); // 방에 들어오면 내 준비 상태를 초기화 (False)
    }
    // 방장 변경, 입장/퇴장 시 UI 상태 업데이트
    void UpdateRoomUI()
    {
        bool isMaster = PhotonNetwork.IsMasterClient;

        // 방장일 경우: 시작 버튼 보임, 준비 버튼 숨김
        // 참여자일 경우: 시작 버튼 숨김, 준비 버튼 보임
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
                object isReadyVal;
                bool isReady = false;
                // 플레이어가 준비상태인지
                if(players[i].CustomProperties.TryGetValue(IS_READY, out isReadyVal))
                {
                    isReady = (bool)isReadyVal; // 커스텀프로퍼티는 object 타입이라 형변환 -> true로
                }
                // 방장이 아니면서 준비 완료된 상태면 (Ready) 표시 추가
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
        UpdateCharacterImages();
        ShowEnterLog(newPlayer.NickName);
        // 누군가 들어오면 방장의 시작 버튼을 다시 검사 
        if (PhotonNetwork.IsMasterClient) CheckAllPlayersReady();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer) // 플레이어가 룸에서 나갔을 때
    {
        UpdatePlayerList();
        UpdateCharacterImages();
        ShowExitLog(otherPlayer.NickName);
        // 누군가 나가면 남은 인원 기준으로 다시 검사
        if (PhotonNetwork.IsMasterClient) CheckAllPlayersReady();
    }
    // 모든 플레이어가 준비되었는지 확인하는 함수 (방장용)
    void CheckAllPlayersReady()
    {
        bool allReady = true;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            // 방장은 준비 검사에서 제외 
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
        // 버튼 활성화
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
            PhotonNetwork.LoadLevel("Loading");
        }
    }
    public void OnClickReady()
    {
        // 현재 준비 상태를 가져옴
        bool isReady = false;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(IS_READY, out object value))
        {
            isReady = (bool)value;
        }
        // 준비 상태 설정
        SetReadyStatus(!isReady);
    }
    // 내 준비 상태를 네트워크에 설정하는 함수
    void SetReadyStatus(bool ready)
    {
        // 내 준비 상태를 커스텀프로퍼티에 저장
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { IS_READY, ready } });
        
        // 버튼 텍스트 바꾸기 Ready <-> Cancel
        if(readyButton != null)
        {
            TMP_Text btnText = readyButton.GetComponentInChildren<TMP_Text>();
            if(btnText) btnText.text = ready ? "Cancel" : "Ready";
        }
    }
    // 플레이어의 커스텀 프로퍼티(준비 상태)가 변경되면 호출되는 콜백
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // 준비 상태가 변경된 것이라면
        if (changedProps.ContainsKey(IS_READY))
        {
            UpdatePlayerList(); // 이름 옆에 준비 표시를 하고 싶다면 여기서 갱신
            
            // 방장이라면 모든 인원이 준비되었는지 확인
            if (PhotonNetwork.IsMasterClient)
            {
                CheckAllPlayersReady();
            }
        }
        if (changedProps.ContainsKey("SelectedChar"))
        {
            UpdateCharacterImages();
        }
    }
    // 방장이 바뀌었을 때 UI 재설정 (준비버튼 -> 시작버튼)
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        UpdateRoomUI();
    }
    
    // 오른쪽 캐릭터 버튼에서 호출되는 함수
    public void OnClickCharacter(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= characterSprites.Length) return;

        // 커스텀 프로퍼티로 내 캐릭터 선택 정보 저장
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "SelectedChar", characterIndex } });

        // 내 캐릭터 이미지 갱신
        UpdateMyCharacterImage(characterIndex);
    }

    // 내 UI 이미지에 캐릭터 스프라이트 적용
    void UpdateMyCharacterImage(int characterIndex)
    {
        // 현재 방에 있는 플레이어 목록 가져오기
        Player[] players = PhotonNetwork.PlayerList;

        // 플레이어 수만큼 반복
        for (int i = 0; i < players.Length; i++)
        {
            playerCharacterImages[i].gameObject.SetActive(true);
            // players[i]가 나 자신이고, 이미지 배열 범위를 넘지 않는다면
            if (players[i] == PhotonNetwork.LocalPlayer && i < playerCharacterImages.Length)
            {
                // 내가 선택한 캐릭터 인덱스에 해당하는 스프라이트를 내 자리 UI 이미지에 적용
                playerCharacterImages[i].sprite = characterSprites[characterIndex];
                break; // 찾았으면 종료
            }
        }
    }
    void UpdateCharacterImages()
    {
        Player[] players = PhotonNetwork.PlayerList;

        // 플레이어 UI 이미지 개수만큼 반복
        for (int i = 0; i < playerCharacterImages.Length; i++)
        {
            if (i < players.Length)
            {
                playerCharacterImages[i].gameObject.SetActive(true); // 이미지를 켬
                
                // 해당 플레이어의 CustomProperties에서 "SelectedChar" 값을 가져옴
                if (players[i].CustomProperties.TryGetValue("SelectedChar", out object charIndex))
                {
                    // object → int 변환
                    int index = (int)charIndex;
                    if (index >= 0 && index < characterSprites.Length)
                    {
                        playerCharacterImages[i].sprite = characterSprites[index];
                    }
                }
            }
            // 접속자가 없는 빈 슬롯인 경우
            else
            {
                playerCharacterImages[i].gameObject.SetActive(false); 
            }
        }
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
