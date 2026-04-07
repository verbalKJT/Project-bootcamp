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
    [SerializeField] private AudioClip difClip;
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
        if (!PhotonNetwork.IsMasterClient) return;
        if (other.tag == "Player")
        {
            Debug.Log(other.gameObject.name);
            LivingEnitiy player = other.GetComponent<PlayerHealth>();
            player.TakeDamagePlayer(damage);
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            // effect 생성.
            photonView.RPC("SpawnHitWire", RpcTarget.All, hitPoint);
            PhotonNetwork.Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            Shield shield = other.GetComponentInParent<Shield>();
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            if (shield != null)
                shield.TakeShieldDamage(damage);
            photonView.RPC("SpawnDifEffect", RpcTarget.All, hitPoint);
            
            PhotonNetwork.Destroy(gameObject);
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

    [PunRPC]
    public void SpawnDifEffect(Vector3 hitPoint)
    {
       GameObject effect = Instantiate(hitEffect, hitPoint, Quaternion.identity);
       AudioSource audioSource = effect.GetComponent<AudioSource>();
       if (audioSource != null)
       {
           audioSource.Stop();
           audioSource.PlayOneShot(difClip);
       }
    }

    [PunRPC]
    public void SpawnHitWire(Vector3 hitPoint)
    {
        Instantiate(hitEffect, hitPoint, Quaternion.identity);
    }
}