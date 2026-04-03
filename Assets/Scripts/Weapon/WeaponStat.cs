using UnityEngine;
using System.Collections.Generic;

public enum WeaponStatType
{
    Damage,
    CoolTime,
    ProjectileCount,
    ProjectileSpeed,
}

[CreateAssetMenu(fileName = "WeaponStat", menuName = "Scriptable Objects/WeaponStat")]
public class WeaponStat : ScriptableObject
{
    public List<float> damage;
    public List<float> coolTime;
    public List<float> projectileCount;
    public List<float> projectileSpeed;
}
