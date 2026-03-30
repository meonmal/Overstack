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
}

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public List<float> moveSpeed;
    public List<float> damage;
    public List<float> coolTime;
    public List<float> playerHp;
    public List<float> projectileCount;
}
