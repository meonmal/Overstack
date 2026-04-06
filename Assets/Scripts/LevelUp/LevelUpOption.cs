using UnityEngine;

public enum LevelUpTargetType
{
    Player,
    Weapon
}

public class LevelUpOption
{
    public LevelUpTargetType targetType;

    public StatType playerStatType;
    public WeaponStatType weaponStatType;

    public WeaponBase targetWeapon;

    public string title;
    public Sprite icon;

    public float currentValue;
    public float nextValue;
    public float deltaValue;
}
