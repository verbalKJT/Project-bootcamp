using UnityEngine;

public class PlayerAttack : PlayerInput
{
    // 하위 스크립트에서도 사용가능하게 protected

    public Animator animator;

    protected PlayerMovement pm;
    
    public virtual float FirstSkillTime => 0;
    public virtual float FirstSkillCool => 0;
    public virtual float SecSkillTime => 0;
    public virtual float SecSkillCool => 0;
    protected void Start()
    {
        animator = GetComponentInChildren<Animator>();
        pm = GetComponent<PlayerMovement>();
    }
}