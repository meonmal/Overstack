using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class BulletSpawner : WeaponBase
{
    [SerializeField]
    private WeaponStat weaponStat;
    [SerializeField]
    private Bullet bulletPrefab;

    private WeaponRunTimeStat weaponRunTimeStat;
    private RangeWeapon rangeWeapon;
    private Player player;
    private float spawnTime;
    private float damage;

    private IObjectPool<Bullet> pool;

    public override WeaponRunTimeStat RunTimeStat => weaponRunTimeStat;

    private void Awake()
    {
        rangeWeapon = GetComponent<RangeWeapon>();
        player = GetComponentInParent<Player>();

        pool = new ObjectPool<Bullet>(
            SpawnBullet,
            OnGet,
            OnRelease,
            DestroyBullet,
            true,
            20,
            maxSize: 100);
    }

    private void Start()
    {
        weaponRunTimeStat = new WeaponRunTimeStat(weaponStat);

        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            if(!rangeWeapon.HasValidTarget)
            {
                yield return null;
                continue;
            }

            spawnTime = player.runTimeStat.GetStat(StatType.CoolTime) * weaponRunTimeStat.GetStat(WeaponStatType.CoolTime);

            int spawnCount = Mathf.RoundToInt(player.runTimeStat.GetStat(StatType.ProjectileCount) * weaponRunTimeStat.GetStat(WeaponStatType.ProjectileCount));

            for(int i = 0; i < spawnCount; i++)
            {
                Bullet bullet = pool.Get();

                bullet.transform.position = transform.position;

                Vector2 dir = GetDirection();
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                bullet.transform.eulerAngles = new Vector3(0, 0, angle);

                damage = player.runTimeStat.GetStat(StatType.Damage) * weaponRunTimeStat.GetStat(WeaponStatType.Damage);

                bullet.Init(pool, damage);
            }

            yield return new WaitForSeconds(spawnTime);
        }
    }

    private Bullet SpawnBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform);

        return bullet;
    }

    private Vector2 GetDirection()
    {
        Vector2 direction = (rangeWeapon.CurrentTarget.transform.position - transform.position).normalized;

        return direction;
    }

    public void OnGet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    public void OnRelease(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    public void DestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }
}
