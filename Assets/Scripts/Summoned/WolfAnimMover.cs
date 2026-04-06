using UnityEngine;

public class WolfAnimMover : MonoBehaviour
{
    private Wolf wolf;
    private WolfFeedback feedback;
    void Start()
    {
        wolf = GetComponent<Wolf>();
        feedback = GetComponent<WolfFeedback>();
    }

    public void WolfAttack()
    {
        // 공격 타이밍에 IsTrigger true
        wolf.collider.isTrigger = true;
        feedback.WolfBarkSound();
    }

    public void WolfRestAttack()
    {
        // 공겨 애니메이션 끝날 때쯤 호출
        wolf.collider.isTrigger = false;
    }
}
