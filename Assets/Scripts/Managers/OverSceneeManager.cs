using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Video; 

public class OverSceneeManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Canvas endCanvas;
    
    [SerializeField] private PlayableDirector director;
    void Start()
    {
        director.stopped += OnCanvas;
        
        Cursor.visible = true;
    }

    private void OnCanvas(PlayableDirector obj)
    {
        endCanvas.gameObject.SetActive(true);
    }

    // 버튼에 달 OnClick 메소드
    public void onClickToLobby()
    {
        PhotonNetwork.LeaveRoom(); // 방 나가고
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("IntroUI");
    }

    public void onClickExit()
    {
        Application.Quit();
    }
}