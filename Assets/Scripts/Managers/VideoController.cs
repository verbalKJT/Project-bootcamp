using Photon.Pun;
using UnityEngine;
using UnityEngine.Video; 

public class VideoController : MonoBehaviourPunCallbacks
{
    public VideoPlayer myVideoPlayer;

    [SerializeField] private Canvas videoCanvas;
    [SerializeField] private Canvas endCanvas;
   

    void Start()
    {
        myVideoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer videoPlayer)
    {
        endCanvas.gameObject.SetActive(true);
    }
}