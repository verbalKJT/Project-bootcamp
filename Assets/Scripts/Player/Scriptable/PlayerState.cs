using UnityEngine;

[CreateAssetMenu(fileName = "PlayerState", menuName = "Scriptable Objects/PlayerState")]
public class PlayerState : ScriptableObject
{
    [SerializeField] public int hp = 100;
    [SerializeField] public float speed = 20f;
    [SerializeField] public float jumpForce = 30f;
    [SerializeField] public float rotationSpeed = 50f;
}
