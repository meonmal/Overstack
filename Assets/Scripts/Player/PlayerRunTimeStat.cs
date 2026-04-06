using System.Collections.Generic;
using UnityEngine;

public class PlayerRunTimeStat
{
    public int Level { get; private set; } = 1;
    public float CurrentExp { get; private set; } = 0f;

    private Dictionary<StatType, RunTimeStat> stats;
    private PlayerStats data;

    public PlayerRunTimeStat(PlayerStats data)
    {
        this.data = data;

        stats = new Dictionary<StatType, RunTimeStat>
        {
            { StatType.MoveSpeed, new RunTimeStat(data.moveSpeed) },
            { StatType.Damage, new RunTimeStat(data.damage) },
            { StatType.CoolTime, new RunTimeStat(data.coolTime) },
            { StatType.PlayerHp, new RunTimeStat(data.playerHp) },
            { StatType.ProjectileCount, new RunTimeStat(data.projectileCount) },
            { StatType.ProjectileSpeed, new RunTimeStat(data.projectileSpeed) },
            { StatType.AbsorbRange, new RunTimeStat(data.absorbRange) },
        };
    }

    public List<StatType> GetAvailableStats()
    {
        List<StatType> result = new List<StatType>();

        foreach (var pair in stats)
        {
            if (!pair.Value.IsMax)
            {
                result.Add(pair.Key);
            }
        }

        return result;
    }

    public float GetNextStat(StatType type)
    {
        return stats[type].GetNextValue();
    }

    public float GetDelta(StatType type)
    {
        return stats[type].GetDelta();
    }

    /// <summary>
    /// 버프가 적용되지 않은 기본 런타임 스탯 값
    /// </summary>
    public float GetBaseStat(StatType type) => stats[type].Value;

    public float GetStat(StatType type)
    {
        float baseValue = stats[type].Value;

        return baseValue;
    }

    public int GetIntStat(StatType type)
    {
        return Mathf.RoundToInt(stats[type].Value);
    }

    /// <summary>
    /// 지정한 스탯의 레벨을 1 증가시킨다.
    /// </summary>
    /// <param name="type">레벨업 할 스탯의 타입</param>
    public void LevelUp(StatType type) => stats[type].LevelUp();

    /// <summary>
    /// 지정한 스탯이 최대 레벨인지 확인한다.
    /// </summary>
    /// <param name="type">확인할 스탯의 타입</param>
    /// <returns>최대 레벨이면 true</returns>
    public bool IsMax(StatType type) => stats[type].IsMax;

    public int AddExp(float amount)
    {
        CurrentExp += amount;

        int levelUpCount = 0;

        while (CurrentExp >= data.GetRequiredExp(Level))
        {
            CurrentExp -= data.GetRequiredExp(Level);
            Level++;
            levelUpCount++;
        }

        return levelUpCount;
    }

    public float GetExpProgress()
    {
        return CurrentExp / data.GetRequiredExp(Level);
    }
}
