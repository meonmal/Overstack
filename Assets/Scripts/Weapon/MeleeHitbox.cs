using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    [SerializeField]
    private WeaponStat weaponStat;
    private PlayerRunTimeStat playerRunTimeStat;
    private WeaponRunTimeStat weaponRunTimeStat;
    private HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();

    private void Start()
    {
        weaponRunTimeStat = new WeaponRunTimeStat(weaponStat);
        playerRunTimeStat = GetComponentInParent<Player>().runTimeStat;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Monster"))
        {
            return;
        }

        if (hitTargets.Contains(collision))
        {
            return;
        }

        IDamageable iDamageable = collision.GetComponent<IDamageable>();

        float damage = playerRunTimeStat.GetStat(StatType.Damage) * weaponRunTimeStat.GetStat(WeaponStatType.Damage);

        if(iDamageable != null)
        {
            iDamageable.TakeDamage(damage);
        }
    }
}
