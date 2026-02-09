using UnityEngine;

public class PlayerHealth : LivingEnitiy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void OnHpChanged() // 자식들이 쓸 껍데기
    { }
    protected override void OnDeath() // 죽었을 때
    { }
    protected override void OnRespawn() // 다시 살아날때
    { }
}
