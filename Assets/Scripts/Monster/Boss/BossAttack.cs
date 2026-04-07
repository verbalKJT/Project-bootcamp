using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class BossAttack : MonoBehaviourPun
{
    [Header("컴포넌트")] [SerializeField] private BossHp boss;
    [SerializeField] private Animator _animator;

    [Header("공격 스크립트")] [SerializeField] private FallAttack _fall;
    [SerializeField] private ElectricWireAttack _electric;

    // true면 전깃줄 생성 공격도 같이.
    public bool IsHalf = false;

    // 공격 쿨타임
    private float fallTimer = 3f;
    private const float fallCool = 6f;
    private float electricTimer = 18f; // 20
    private const float electricCool = 18f;

    private BossFeedBack feedback;

    void Start()
    {
        feedback = GetComponent<BossFeedBack>();
    }

    void Update()
    {
        // 보스 공격 쿨타임은 방장만 재기
        if (!PhotonNetwork.IsMasterClient) return;
        if (CriticalObj.isDestroyed)
        {
            feedback.CallSpawnClip();
            return;
        }
        fallTimer += Time.deltaTime;
        if (fallTimer >= fallCool)
        {
            photonView.RPC("FallCrystal", RpcTarget.All);
            fallTimer = 0f;
        }

        electricTimer += Time.deltaTime;
        if (IsHalf && electricTimer >= electricCool)
        {
            photonView.RPC("ElectricWire", RpcTarget.All);
            electricTimer = 0f;
        }
    }

    [PunRPC]
    public void FallCrystal()
    {
        feedback.CallFallClip();
        _animator.SetTrigger("Fall");
    }

    // 애니메이션 이벤트로 원하는 타이밍에 호출
    public void SpawnCrystal()
    {
        _fall.CallFallAttack();
        fallTimer = 0f; // 소환 후 쿨타임 재기.
    }

    [PunRPC]
    private void ElectricWire()
    {
        feedback.CallElecClip();
        _animator.SetTrigger("Electric");
    }

    // 애니메이션 이벤트로 원하는 타이밍에 호출
    public void SpwanElectric()
    {
        _electric.CallElectric();
        electricTimer = 0f;
    }
}