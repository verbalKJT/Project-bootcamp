using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;
using ExitGames.Client.Photon;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public TMP_Text[] playerNameTexts;
    public Image[] playerCharacterImages;
    public Slider loadingSlider;

    [Header("캐릭터 스프라이트")]
    public Sprite[] characterSprites;

    private AsyncOperation asyncOperation;

    void Start()
    {
        UpdatePlayerInfo();
        StartCoroutine(LoadGameSceneAsync());
    }

    void UpdatePlayerInfo()
    {
        Player[] players = PhotonNetwork.PlayerList;
        
        // 플레이어 UI 이미지 개수만큼 반복
        for (int i = 0; i < playerCharacterImages.Length; i++)
        {
            if (i < players.Length)
            {
                playerCharacterImages[i].gameObject.SetActive(true); // 이미지를 켬
                playerNameTexts[i].text = players[i].NickName;

                if (players[i].CustomProperties.TryGetValue("SelectedChar", out object charIndex))
                {
                    int index = (int)charIndex;
                    if (index >= 0 && index < characterSprites.Length)
                    {
                        playerCharacterImages[i].sprite = characterSprites[index];
                    }
                }
            }
            else
            {
                playerCharacterImages[i].gameObject.SetActive(false); 
            }
        }
    }

    IEnumerator LoadGameSceneAsync()
    {
        asyncOperation = SceneManager.LoadSceneAsync("Map1");
        asyncOperation.allowSceneActivation = false;
        
        float progress = 0f; // 실제 UI에 보여줄 값

        while (!asyncOperation.isDone)
        {
            float targetProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            
            // 로딩 게이지 부드럽게 증가
            progress = 
                Mathf.MoveTowards(progress, targetProgress, Time.deltaTime * 0.5f);  // ← 이 값이 속도 조절
            
            loadingSlider.value = progress;
            
            // 로딩 100% 됐을 때
            if (progress >= 1f)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    // ⭐ 모든 클라이언트에게 씬 활성화 신호
                    photonView.RPC("ActivateScene", RpcTarget.AllBuffered);
                }
                yield break;
            }
            yield return null;
        }
    }

    [PunRPC]
    void ActivateScene()
    {
        if (asyncOperation != null)
        {
            asyncOperation.allowSceneActivation = true;
        }
    }
}
