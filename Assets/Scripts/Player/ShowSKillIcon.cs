using UnityEngine;

public class ShowSKillIcon : MonoBehaviour
{
    [Header("스킬 UI 연결")]
    [SerializeField] private SkillSlotUI fistSkillSlotUI;
    [SerializeField] private SkillSlotUI secondSkillSlotUI;

    private PlayerAttack pa;

    void Start()
    {
        pa = GetComponentInChildren<PlayerAttack>();
    }

    void Update()
    {
        fistSkillSlotUI.UpdateSkillSlot(pa.FirstSkillTime,pa.FirstSkillCool);
        secondSkillSlotUI.UpdateSkillSlot(pa.SecSkillTime, pa.SecSkillCool);
    }
}
