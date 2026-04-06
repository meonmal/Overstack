using UnityEngine;
using UnityEngine.Pool;

public class ExpOrbSpawner : MonoBehaviour
{
    [SerializeField]
    private ExpOrb expOrbPrefab;

    private IObjectPool<ExpOrb> pool;

    private void Awake()
    {
        pool = new ObjectPool<ExpOrb>(
            CreateOrb,
            OnGetOrb,
            OnReleaseOrb,
            OnDestroyOrb,
            true,
            30,
            200);
    }

    private ExpOrb CreateOrb()
    {
        ExpOrb orb = Instantiate(expOrbPrefab, transform);
        orb.SetPool(pool);
        return orb;
    }

    private void OnGetOrb(ExpOrb orb)
    {
        orb.gameObject.SetActive(true);
    }

    private void OnReleaseOrb(ExpOrb orb)
    {
        orb.gameObject.SetActive(false);
    }

    private void OnDestroyOrb(ExpOrb orb)
    {
        Destroy(orb.gameObject);
    }

    public void Spawn(Vector3 position, float expAmount)
    {
        ExpOrb orb = pool.Get();
        orb.transform.position = position;
        orb.Init(expAmount);
    }
}
