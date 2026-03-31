using UnityEngine;

[CreateAssetMenu(fileName = "MonsterState", menuName = "Scriptable Objects/MonsterState")]
public class MonsterState : ScriptableObject
{
    public float moveSpeed = 20f;
    public int hp = 100;
    public int nearDam = 8;
}
