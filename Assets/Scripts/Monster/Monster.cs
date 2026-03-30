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
    private MonsterObjectPool ownerPool;
    private float currentHp;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        monsterMovement = GetComponent<MonsterMovement>();
    }

    private void Start()
    {
        spriteRenderer.sprite = MonsterStat.sprite;
        currentHp = MonsterStat.MaxHp;
    }

    public void SetPool(IObjectPool<Monster> pool)
    {
        _pool = pool;
    }

    public void SetOwnerPool(MonsterObjectPool ownerPool)
    {
        this.ownerPool = ownerPool;
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        if(currentHp <= 0)
        {
            ReturnToPool();

            if(_pool == null)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Setup(MonsterStats data, Rigidbody2D target)
    {
        monsterMovement.SetTarget(target);

        MonsterStat = data;

        if (animator != null && data.animatorController != null)
        {
            animator.runtimeAnimatorController = data.animatorController;
        }
    }

    public void ReturnToPool()
    {
        _pool.Release(this);
    }
}
