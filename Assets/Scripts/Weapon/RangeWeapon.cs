using UnityEngine;

public class RangeWeapon : MonoBehaviour
{
    [SerializeField]
    private float detectionRange;
    [SerializeField]
    private LayerMask monsterLayer;

    private Collider2D[] colliders;

    private Collider2D currentTarget;

    public Collider2D CurrentTarget => currentTarget;

    public bool HasValidTarget
    {
        get
        {
            if(currentTarget == null)
            {
                return false;
            }

            float distance = (currentTarget.transform.position - transform.position).magnitude;
            return distance <= detectionRange;
        }
    }

    private void Update()
    {
        FindTarget();
    }

    private void FindTarget()
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange, monsterLayer);

        if(colliders.Length == 0)
        {
            currentTarget = null;
            return;
        }

        if (colliders.Length > 0)
        {
            currentTarget = colliders[0];
            float minDistance = Vector2.Distance(transform.position, currentTarget.transform.position);

            foreach (Collider2D col in colliders)
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    currentTarget = col;
                }
            }
        }
    }
}
