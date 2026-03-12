using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerAttack : PlayerInput
{
    // 하위 스크립트에서도 사용가능하게 protected

    protected Animator animator;
   
    protected void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
}