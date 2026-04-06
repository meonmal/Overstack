using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    /// <summary>
    /// 플레이어 스탯의 원본 SO
    /// </summary>
    [SerializeField]
    private PlayerStats playerStats;

    /// <summary>
    /// 플레이어 이동 스크립트.
    /// </summary>
    private PlayerMovement playerMovement;

    /// <summary>
    /// 플레이어 런타임 스탯.
    /// </summary>
    public PlayerRunTimeStat runTimeStat;

    /// <summary>
    /// 경험치 흡수 범위를 담당하는 콜라이더.
    /// </summary>
    private CircleCollider2D coll;

    /// <summary>
    /// 현재 흡수 범위 값.
    /// </summary>
    private float absorbRange;

    /// <summary>
    /// 레벨업 선택창 시스템 참조.
    /// </summary>
    [SerializeField]
    private LevelUpSystem levelUpSystem;

    private List<WeaponBase> weapons = new List<WeaponBase>();
    public List<WeaponBase> Weapons => weapons;


    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        runTimeStat = new PlayerRunTimeStat(playerStats);
        playerMovement.Init(runTimeStat);
        coll = GetComponent<CircleCollider2D>();

        weapons = new List<WeaponBase>(GetComponentsInChildren<WeaponBase>());

        RefreshAbsorbRange();

        if (levelUpSystem != null)
        {
            levelUpSystem.Init(this);
        }
        else
        {
            Debug.LogWarning("씬에 LevelUpSystem이 없음");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("ExpOrb"))
        {
            return;
        }

        ExpOrb orb = collision.GetComponent<ExpOrb>();

        if (orb != null)
        {
            orb.StartAbsorb(transform);
        }
    }

    /// <summary>
    /// 플레이어가 경험치를 획득하는 함수.
    /// 경험치를 추가한 뒤 레벨업이 발생했다면 레벨업 창을 연다.
    /// </summary>
    /// <param name="amount">획득할 경험치 양</param>
    public void AddExp(float amount)
    {
        int levelUpCount = runTimeStat.AddExp(amount);

        Debug.Log($"경험치 획득 : {amount}, 레벨업 횟수 : {levelUpCount}, 현재 레벨 : {runTimeStat.Level}");

        if (levelUpCount > 0)
        {
            if (levelUpSystem != null)
            {
                levelUpSystem.Open();
            }
            else
            {
                Debug.LogWarning("LevelUpSystem 참조가 없어서 레벨업 창을 열 수 없음");
            }
        }
    }

    /// <summary>
    /// 특정 스탯이 레벨업된 뒤 실제 플레이어 컴포넌트에 반영해야 하는 값을 갱신하는 함수.
    /// </summary>
    /// <param name="statType">레벨업된 스탯 타입</param>
    public void RefreshStatsByLevelUp(StatType statType)
    {
        switch (statType)
        {
            case StatType.AbsorbRange:
                RefreshAbsorbRange();
                break;

            case StatType.ProjectileCount:
                foreach(var weapon in weapons)
                {
                    weapon.RefreshStatByLevelUp(WeaponStatType.ProjectileCount);
                }
                break;
        }
    }

    /// <summary>
    /// 현재 런타임 스탯의 흡수 범위를 읽어와
    /// 플레이어의 CircleCollider2D 반경에 반영하는 함수.
    /// </summary>
    private void RefreshAbsorbRange()
    {
        absorbRange = runTimeStat.GetStat(StatType.AbsorbRange);
        coll.radius = absorbRange;
    }
}
