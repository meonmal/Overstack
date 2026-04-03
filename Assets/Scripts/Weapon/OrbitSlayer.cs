using UnityEngine;

public class OrbitSlayer : MonoBehaviour
{
    [SerializeField]
    private WeaponStat weaponStat;

    private Player player;
    private WeaponRunTimeStat weaponRunTimeStat;

    private float damage;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        weaponRunTimeStat = new WeaponRunTimeStat(weaponStat);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        damage = player.runTimeStat.GetStat(StatType.Damage) * weaponRunTimeStat.GetStat(WeaponStatType.Damage);

        if (!collision.CompareTag("Monster"))
        {
            return;
        }

        IDamageable iDamageable = collision.GetComponent<IDamageable>();

        if(iDamageable == null)
        {
            return;
        }

        iDamageable.TakeDamage(damage);
    }
}
