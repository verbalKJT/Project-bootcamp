using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Canvas endCanvas;
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Clear")
            StartCoroutine(ActiveRotine(3f));
    }

    IEnumerator ActiveRotine(float time)
    {
        yield return new WaitForSeconds(time);
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

