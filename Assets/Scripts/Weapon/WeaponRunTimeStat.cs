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
            { WeaponStatType.ProjectileCount, new RunTimeStat(data.projectileCount) },
            { WeaponStatType.ProjectileSpeed, new RunTimeStat(data.projectileSpeed) },
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

    public float GetNextStat(WeaponStatType statType)
    {
        return stats[statType].GetNextValue();
    }

    public void LevelUpStat(WeaponStatType statType)
    {
        stats[statType].LevelUp();
    }

    public bool IsMax(WeaponStatType statType)
    {
        return stats[statType].IsMax;
    }

    public List<WeaponStatType> GetAvailableStats()
    {
        List<WeaponStatType> result = new List<WeaponStatType>();

        foreach(var pair in stats)
        {
            if (!pair.Value.IsMax)
            {
                result.Add(pair.Key);
            }
        }

        return result;
    }
}
