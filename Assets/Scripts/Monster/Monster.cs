using UnityEngine;
using UnityEngine.Pool;

public class Monster : MonoBehaviour, IDamageable
{
    public MonsterStats MonsterStat { get; private set; }

    [SerializeField]
    private Transform visualRoot;
    [SerializeField]
    private Animator animator;

    /// <summary>
    /// 몬스터를 관리할 오브젝트 풀.
    /// </summary>
    private IObjectPool<Monster> _pool;

    private SpriteRenderer spriteRenderer;
    private MonsterMovement monsterMovement;
    private ExpOrbSpawner expOrbSpawner;
    private CircleCollider2D monsterCollider;
    private float currentHp;
    private bool isDead;
    private float expAmount;
    private float damage;

    private void Awake()
    {
        spriteRenderer = visualRoot.GetComponent<SpriteRenderer>();
        monsterMovement = GetComponent<MonsterMovement>();
        monsterCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        damage = MonsterStat.Damage;
    }

    public void SetPool(IObjectPool<Monster> pool)
    {
        _pool = pool;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        SoundManager.Instance.PlaySfx(SfxType.EnemyHit);
        currentHp -= damage;

        if(currentHp <= 0)
        {
            isDead = true;

            SoundManager.Instance.PlaySfx(SfxType.EnemyDead);

            expAmount = MonsterStat.ExpAmount;

            expOrbSpawner.Spawn(transform.position, expAmount);

            if(_pool == null)
            {
                Destroy(gameObject);
            }

            ReturnToPool();
        }
    }

    public void Setup(MonsterStats data, Rigidbody2D target, ExpOrbSpawner expOrbSpawner)
    {
        monsterMovement.SetTarget(target);

        MonsterStat = data;

        this.expOrbSpawner = expOrbSpawner;

        isDead = false;
        currentHp = data.MaxHp;

        if(spriteRenderer != null)
        {
            spriteRenderer.sprite = data.sprite;
        }

        if(visualRoot != null)
        {
            visualRoot.localScale = data.visualScale;
        }

        if(monsterCollider != null)
        {
            monsterCollider.radius = data.colliderRadius;
            monsterCollider.enabled = true;
        }

        if (animator != null && data.animatorController != null)
        {
            animator.runtimeAnimatorController = data.animatorController;
        }
    }

    public void ReturnToPool()
    {
        if(_pool == null)
        {
            Destroy(gameObject);
            return;
        }

        _pool.Release(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isDead == true)
        {
            return;
        }

        IDamageable iDamageable = collision.GetComponent<IDamageable>();

        if(iDamageable != null && collision.CompareTag("Player"))
        {
            iDamageable.TakeDamage(damage);
        }
    }
}
