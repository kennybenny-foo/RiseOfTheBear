using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponStats", menuName = "Weapons/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    public float damage = 25f;
    public float range = 100f;
    public int maxAmmo = 12;

    public float trailDuration = 0.02f;
    public float trailWidth = 0.02f;
}