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
    protected virtual void OnHpChanged() // 자식들이 쓸 껍데기
    { }
    protected virtual void OnDeath() // 죽었을 때
    { }
    protected virtual void OnRespawn() // 다시 살아날때
    { }
}
