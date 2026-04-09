using System;
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

    private void Awake()
    {
        // 전송 주기 올리기
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

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
        // 각 클라이언트가 로컬로 Map1을 여는 방식이 아닌
        //asyncOperation = SceneManager.LoadSceneAsync("Map1");
        
        float progress = 0f; // 실제 UI에 보여줄 값

        while (progress < 1f)
        { 
            // 로딩 게이지 부드럽게 증가
            progress = 
                Mathf.MoveTowards(progress, 1f, Time.deltaTime * 0.5f);  // ← 이 값이 속도 조절
            loadingSlider.value = progress;
            yield return null;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Map1"); // 같이 넘어감
        }
    }
}
