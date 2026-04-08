using System.Collections;
using MagicPigGames;
using Photon.Pun;
using UnityEngine;

public class Shield : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private GameObject energyShield; // 방패 하위 Plane
    [SerializeField] private Renderer shieldRenderer;
    [SerializeField] private Collider energyShieldCollider;

    [Header("Shield Stat")] [SerializeField]
    private int maxShieldHp = 200;

    [SerializeField] private float recoverDelay = 10f;
    public int CurrentHp { get; private set; }
    public bool IsRaised { get; private set; }
    public bool IsBroken { get; private set; }
    public bool IsRecovering { get; private set; }

    [Header("쉴드 체력 바 UI")] [SerializeField]
    private ProgressBar hpBar;

    public bool CanRaise => !IsRecovering && !IsBroken;

    private RockAttack _rockAttack;

    void Awake()
    {
        _rockAttack = GetComponentInParent<RockAttack>();
        if (energyShieldCollider == null)
        {
            energyShieldCollider = GetComponent<Collider>();
        }

        if (shieldRenderer == null && energyShield != null)
        {
            shieldRenderer = energyShield.GetComponent<Renderer>();
        }
    }

    void Start()
    {
        CurrentHp = maxShieldHp;
        ApplyState();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(CurrentHp);
            stream.SendNext(IsRaised);
            stream.SendNext(IsBroken);
            stream.SendNext(IsRecovering);
        }
        else
        {
            CurrentHp = (int)stream.ReceiveNext();
            IsRaised = (bool)stream.ReceiveNext();
            IsBroken = (bool)stream.ReceiveNext();
            IsRecovering = (bool)stream.ReceiveNext();
            ApplyState();
        }
    }

    public void TryRaised()
    {
        if (!CanRaise) return;

        IsRaised = true;
        ApplyState();
    }

    public void Lower()
    {
        IsRaised = false;
        ApplyState();
    }

    public void TakeShieldDamage(int damage)
    {
        if (!CanRaise || !IsRaised) return;

        CurrentHp = Mathf.Max(0, CurrentHp - damage);

        if (CurrentHp <= 0)
        {
            BreakShield();
        }
        else
        {
            ApplyState();
        }
    }

    private void BreakShield()
    {
        if (IsBroken)
        {
            return;
        }

        IsBroken = true;
        IsRecovering = true;
        IsRaised = false;

        _rockAttack?.ForceShieldDown();
        ApplyState();

        StartCoroutine(RecoveryShield(recoverDelay));
    }

    IEnumerator RecoveryShield(float time)
    {
        yield return new WaitForSeconds(time);
        IsRecovering = false;
        IsBroken = false;
        IsRaised = false;
        CurrentHp = maxShieldHp; //회복
        ApplyState();
    }

    public void ApplyState()
    {
        // 들고 있고 파괴되지 않았으면 true
        bool visible = IsRaised && !IsBroken;

        if (energyShield != null)
        {
            energyShield.SetActive(visible);
        }

        if (energyShieldCollider != null)
        {
            energyShieldCollider.enabled = visible;
        }

        float ratio = (float)CurrentHp / maxShieldHp;
        // UI 갱신
        hpBar.SetProgress(ratio);
    }
}