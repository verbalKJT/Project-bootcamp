using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class IntroManager : MonoBehaviour
{
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 로비 이동
    public void GameStart()
    {
        SceneManager.LoadScene("Lobby");
    }
    // 게임 종료
    public void ExitGame()
    {
        Application.Quit();
    }
}
