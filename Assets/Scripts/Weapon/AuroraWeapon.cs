using System.Collections;
using UnityEngine;

public class AuroraWeapon : WeaponBase
{
    [SerializeField]
    private WeaponStat weaponStat;
    [SerializeField]
    private LayerMask monsterLayer;

    private Player player;
    private WeaponRunTimeStat weaponRunTimeStat;
    private CircleCollider2D auroraCollider;
    private Collider2D[] monsters;
    private float damage;
    private float coolTime;
    private float attackRange;

    public override WeaponRunTimeStat RunTimeStat => weaponRunTimeStat;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        weaponRunTimeStat = new WeaponRunTimeStat(weaponStat);
        auroraCollider = GetComponent<CircleCollider2D>();

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            attackRange = auroraCollider.radius * transform.lossyScale.x;

            monsters = Physics2D.OverlapCircleAll(transform.position, attackRange, monsterLayer);

            damage = player.runTimeStat.GetStat(StatType.Damage) * weaponRunTimeStat.GetStat(WeaponStatType.Damage);
            coolTime = player.runTimeStat.GetStat(StatType.CoolTime) * weaponRunTimeStat.GetStat(WeaponStatType.CoolTime);

            for (int i = 0; i < monsters.Length; i++)
            {
                IDamageable iDamageable = monsters[i].GetComponent<IDamageable>();

                if (iDamageable != null)
                {
                    iDamageable.TakeDamage(damage);
                }
            }

            yield return new WaitForSeconds(coolTime);
        }
    }
}
