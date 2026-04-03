using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class OrbitSlayerWeapon : MonoBehaviour
{
    [SerializeField]
    private GameObject orbitPrefab;
    [SerializeField]
    private WeaponStat weaponStat;

    private int count;
    private float radius;
    private float rotationSpeed;

    private List<GameObject> weapons = new List<GameObject>();
    private Player player;
    private WeaponRunTimeStat weaponRunTimeStat;

    private void Awake()
    {
        player = GetComponentInParent<Player>();

        weaponRunTimeStat = new WeaponRunTimeStat(weaponStat);
    }

    private void Start()
    {
        CreateWeapons();
        ArrangeInCircle();
    }

    private void Update()
    {
        RotateCenter();
    }

    private void CreateWeapons()
    {
        count = (int)player.runTimeStat.GetStat(StatType.ProjectileCount) * (int)weaponRunTimeStat.GetStat(WeaponStatType.ProjectileCount);

        for(int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(orbitPrefab, transform);
            weapons.Add(obj);
        }
    }

    private void ArrangeInCircle()
    {
        float angleStep = 360f / weapons.Count;

        radius = weapons.Count;

        for(int i = 0; i < weapons.Count; i++)
        {
            float angle = angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0f);

            weapons[i].transform.localPosition = pos;
        }
    }

    private void RotateCenter()
    {
        rotationSpeed = player.runTimeStat.GetStat(StatType.ProjectileSpeed) * weaponRunTimeStat.GetStat(WeaponStatType.ProjectileSpeed);

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
