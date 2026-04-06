using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField]
    protected string weaponName;

    [SerializeField]
    protected Sprite weaponIcon;

    public string WeaponName => weaponName;

    public Sprite WeaponIcon => weaponIcon;

    public abstract WeaponRunTimeStat RunTimeStat { get; }

    public virtual void RefreshStatByLevelUp(WeaponStatType statType)
    {

    }
}
