using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToEnding : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameoverSceneName;
    private bool isLeaving;
    void OnTriggerEnter(Collider other)
    {
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

