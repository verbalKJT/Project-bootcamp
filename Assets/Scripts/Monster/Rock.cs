using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class Rock : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    private int damage = 10;
    public Rigidbody rb { get; private set; }

    private EnemyRangeAttack era;

    [SerializeField] private GameObject hitEffect;

    void Start()
    {
        StartCoroutine(DestroySelf(6f));
        rb = GetComponent<Rigidbody>();
        era = GetComponentInParent<EnemyRangeAttack>();
    }

    // IPunInstantiateMagicCallback 콜백 함수
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // 생성 시 넘어온 데이터 읽기.
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            int parentViewID = (int)data[0];
            // 자신을 생성한 오브젝트를 포톤뷰 아이디를 확인해 찾기
            PhotonView pv = PhotonView.Find(parentViewID);

            if (pv != null)
            {
                // 모델의 FirPos를 찾아서 위치 저장
                Transform targetTransform = pv.transform.Find("FirPos");
                // 자식으로 위치
                transform.SetParent(targetTransform);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            LivingEnitiy player = other.GetComponent<PlayerHealth>();
            player.TakeDamagePlayer(damage);
            
            // effect 생성.
            
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else if (other.tag == "Shield")
        {
            Shield shield = other.GetComponent<Shield>();
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    IEnumerator DestroySelf(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}