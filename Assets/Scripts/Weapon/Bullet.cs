using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private WeaponStat weaponStat;

    private Player player;
    private WeaponRunTimeStat weaponRunTimeStat;
    private BulletSpawner bulletSpawner;
    private Rigidbody2D rigid;

    private float moveSpeed;
    private float damage;
    private bool isHit = false;

    private IObjectPool<Bullet> pool;

    public void Init(IObjectPool<Bullet> _pool, float _damage)
    {
        pool = _pool;

        damage = _damage;

        isHit = false;
    }

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        bulletSpawner = GetComponentInParent<BulletSpawner>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        weaponRunTimeStat = new WeaponRunTimeStat(weaponStat);
    }


    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        moveSpeed = player.runTimeStat.GetStat(StatType.ProjectileSpeed) * weaponRunTimeStat.GetStat(WeaponStatType.ProjectileSpeed);

        rigid.linearVelocity = transform.right * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHit)
        {
            return;
        }

        IDamageable iDamageable = collision.GetComponent<IDamageable>();

        if(iDamageable != null && collision.CompareTag("Monster"))
        {
            iDamageable.TakeDamage(damage);

            isHit = true;

            if (pool != null)
            {
                pool.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
