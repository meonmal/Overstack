using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    MoveSpeed,
    Damage,
    CoolTime,
    PlayerHp,
    ProjectileCount,
    ProjectileSpeed,
    AbsorbRange,
}

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public List<float> moveSpeed;
    public List<float> damage;
    public List<float> coolTime;
    public List<float> playerHp;
    public List<float> projectileCount;
    public List<float> projectileSpeed;
    public List<float> absorbRange;

    public float GetRequiredExp(int level)
    {
        return 10f * Mathf.Pow(1.25f, level - 1);
    }
}
