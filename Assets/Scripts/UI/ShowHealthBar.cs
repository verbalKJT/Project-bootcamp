using UnityEngine;

public class ShowHealthBar : MonoBehaviour
{
    Transform mainCam;
    void Awake()
    {
        mainCam = Camera.main.transform; // 메인 카메라 위치
    }
    
    void Update()
    {
        transform.LookAt(mainCam); // 메인카메라 처다보게
    }
}
