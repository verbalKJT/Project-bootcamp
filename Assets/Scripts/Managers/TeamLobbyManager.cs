using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class TeamLobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_Text[] playerNameTexts; // Player1 ~ Player3 UI 텍스트
    public TMP_Text textLogMsg;        // 입장/퇴장 로그 텍스트 영역
    public GameObject startGameButton; // 방장만 보이는 시작 버튼

    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        UpdatePlayerList();
        ShowEnterLog(PhotonNetwork.NickName);
        // 방장만 게임 시작 버튼 활성화
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
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
        //ShowEnterLog(newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // 플레이어가 룸에서 나갔을 때
    {
        UpdatePlayerList();
        ShowExitLog(otherPlayer.NickName);
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

    public void OnClickExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby"); // 나가면 로비로 복귀
    }
}
