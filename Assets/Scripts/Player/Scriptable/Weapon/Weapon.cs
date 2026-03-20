using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    public int damage;
    public float reloadTime;
    public float speed;

    public WeaponEffect effect;
}
