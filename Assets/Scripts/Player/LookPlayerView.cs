using UnityEngine;

public class LookPlayerView : MonoBehaviour
{
    public Transform camPivot;
    public Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!camPivot || !player) return;
        
        // 메인 카메라 위치를 CamPivot으로
        transform.position = camPivot.transform.position;
        transform.rotation = camPivot.transform.rotation;
        // 플레이어 처다보도록
        transform.LookAt(player);
        
    }
}
