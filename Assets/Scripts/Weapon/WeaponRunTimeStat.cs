using System.Collections.Generic;
using UnityEngine;

public class WeaponRunTimeStat
{
    private Dictionary<WeaponStatType, RunTimeStat> stats;

    public WeaponRunTimeStat(WeaponStat data)
    {
        stats = new Dictionary<WeaponStatType, RunTimeStat>
        {
            { WeaponStatType.Damage, new RunTimeStat(data.damage) },
            { WeaponStatType.CoolTime, new RunTimeStat(data.coolTime) },
            { WeaponStatType.ProjectileCount, new RunTimeStat(data.projectileCount) }
        };
    }

    public float GetStat(WeaponStatType statType)
    {
        return stats[statType].Value;
    }

    public int GetIntStat(WeaponStatType statType)
    {
        return Mathf.RoundToInt(stats[statType].Value);
    }

    public float GetDeltaStat(WeaponStatType statType)
    {
        return stats[statType].GetDelta();
    }

    public void LevelUpStat(WeaponStatType statType)
    {
        stats[statType].LevelUp();
    }
}
