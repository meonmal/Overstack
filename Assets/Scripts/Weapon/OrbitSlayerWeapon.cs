using UnityEngine;
using System.Collections.Generic;

public class OrbitSlayerWeapon : WeaponBase
{
    [SerializeField]
    private GameObject orbitPrefab;
    [SerializeField]
    private WeaponStat weaponStat;

    private float radius;
    private float rotationSpeed;

    private List<GameObject> weapons = new List<GameObject>();
    private Player player;
    private WeaponRunTimeStat weaponRunTimeStat;

    public override WeaponRunTimeStat RunTimeStat => weaponRunTimeStat;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        weaponRunTimeStat = new WeaponRunTimeStat(weaponStat);
    }

    private void Start()
    {
        RefreshWeapons();
    }

    private void Update()
    {
        RotateCenter();
    }

    /// <summary>
    /// 현재 스탯 기준으로 필요한 무기 개수를 다시 맞추고,
    /// 원형 배치를 새로 갱신하는 함수.
    /// 
    /// 이 함수는 시작 시 1번 호출하고,
    /// ProjectileCount가 레벨업 되었을 때도 다시 호출하면 된다.
    /// </summary>
    public void RefreshWeapons()
    {
        int targetCount = Mathf.RoundToInt(
            player.runTimeStat.GetStat(StatType.ProjectileCount) *
            weaponRunTimeStat.GetStat(WeaponStatType.ProjectileCount)
        );

        // 최소 1개는 유지하도록 보정.
        targetCount = Mathf.Max(1, targetCount);

        // 현재 개수가 부족하면 새로 생성.
        while (weapons.Count < targetCount)
        {
            GameObject obj = Instantiate(orbitPrefab, transform);
            weapons.Add(obj);
        }

        // 현재 개수가 더 많으면 뒤에서부터 제거.
        while (weapons.Count > targetCount)
        {
            int lastIndex = weapons.Count - 1;

            if (weapons[lastIndex] != null)
            {
                Destroy(weapons[lastIndex]);
            }

            weapons.RemoveAt(lastIndex);
        }

        ArrangeInCircle();
    }

    /// <summary>
    /// 현재 보유한 무기들을 원형으로 균등 배치하는 함수.
    /// </summary>
    private void ArrangeInCircle()
    {
        if (weapons.Count == 0)
        {
            return;
        }

        float angleStep = 360f / weapons.Count;

        // 반지름은 필요하면 따로 스탯화해도 되고,
        // 일단은 개수에 비례하지 않게 고정값 또는 완만한 증가값이 더 자연스럽다.
        radius = 1.5f + (weapons.Count - 1) * 0.2f;

        for (int i = 0; i < weapons.Count; i++)
        {
            float angle = angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(
                Mathf.Cos(rad) * radius,
                Mathf.Sin(rad) * radius,
                0f
            );

            weapons[i].transform.localPosition = pos;
        }
    }

    /// <summary>
    /// 플레이어의 투사체 속도와 무기의 투사체 속도를 곱해서 회전 속도로 사용한다.
    /// </summary>
    private void RotateCenter()
    {
        rotationSpeed =
            player.runTimeStat.GetStat(StatType.ProjectileSpeed) *
            weaponRunTimeStat.GetStat(WeaponStatType.ProjectileSpeed);

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    public override void RefreshStatByLevelUp(WeaponStatType statType)
    {
        switch (statType)
        {
            case WeaponStatType.ProjectileCount:
                RefreshWeapons();
                break;
        }
    }
}
