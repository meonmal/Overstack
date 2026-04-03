using UnityEngine;
using UnityEngine.Pool;

public class Monster : MonoBehaviour, IDamageable
{
    public MonsterStats MonsterStat { get; private set; }

    [SerializeField]
    private Animator animator;

    /// <summary>
    /// 몬스터를 관리할 오브젝트 풀.
    /// </summary>
    private IObjectPool<Monster> _pool;

    private SpriteRenderer spriteRenderer;
    private MonsterMovement monsterMovement;
    private Collider2D monsterCollider;
    private float currentHp;
    private bool isDead;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        monsterMovement = GetComponent<MonsterMovement>();
        monsterCollider = GetComponent<Collider2D>();
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

        currentHp -= damage;

        if(currentHp <= 0)
        {
            isDead = true;

            if(_pool == null)
            {
                Destroy(gameObject);
            }

            ReturnToPool();
        }
    }

    public void Setup(MonsterStats data, Rigidbody2D target)
    {
        monsterMovement.SetTarget(target);

        MonsterStat = data;

        isDead = false;
        currentHp = data.MaxHp;

        if(spriteRenderer != null)
        {
            spriteRenderer.sprite = data.sprite;
        }

        if(monsterCollider != null)
        {
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
}
