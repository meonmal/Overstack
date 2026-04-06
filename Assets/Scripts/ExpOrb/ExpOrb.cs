using UnityEngine;
using UnityEngine.Pool;

public class ExpOrb : MonoBehaviour
{
    private float speed = 8f;
    private float expAmount;
    private float absorbDistance = 0.2f;

    private Transform target;

    private IObjectPool<ExpOrb> pool;

    public float ExpAmount => expAmount;

    public void SetPool(IObjectPool<ExpOrb> _pool)
    {
        pool = _pool;
    }

    public void Init(float amount)
    {
        expAmount = amount;
    }

    public void StartAbsorb(Transform target)
    {
        this.target = target;
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        float distance = (target.position - transform.position).sqrMagnitude;

        if (distance < absorbDistance * absorbDistance)
        {
            Collect();
        }
    }

    private void Collect()
    {
        Player player = target.GetComponent<Player>();

        if (player != null)
        {
            player.AddExp(expAmount);
        }

        target = null;
        pool.Release(this);
    }
}
