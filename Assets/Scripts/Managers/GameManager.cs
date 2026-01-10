using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;// 싱글톤

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject); // 씬 넘어가도 삭제 안되도록
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
