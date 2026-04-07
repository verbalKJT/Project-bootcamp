using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToEnding : MonoBehaviourPunCallbacks
{ 
    private string gameoverSceneName = "Clear";
    private bool isLeaving = false;
    void OnTriggerEnter(Collider other)
    {
        if(!BossHp.BossDead) return;
        if(isLeaving)  return;
        if(!other.CompareTag("Player")) return;
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        
        if (!player.photonView.IsMine) return;
        isLeaving = true;
        PhotonNetwork.LeaveRoom();
    }
    
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(gameoverSceneName);
    }
}

