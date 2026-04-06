using UnityEngine;

[CreateAssetMenu(fileName = "MonsterStats", menuName = "Scriptable Objects/MonsterStats")]
public class MonsterStats : ScriptableObject
{
    public Sprite sprite;
    public AnimatorOverrideController animatorController;

    [SerializeField]
    private float maxHp;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float expAmount;

    /// <summary>
    /// 몬스터의 최대 체력.
    /// </summary>
    public float MaxHp => maxHp;
    /// <summary>
    /// 몬스터의 데미지.
    /// </summary>
    public float Damage => damage;
    /// <summary>
    /// 몬스터의 이동속도.
    /// </summary>
    public float MoveSpeed => moveSpeed;

    public float ExpAmount => expAmount;

    public Vector3 visualScale = Vector3.one;

    public float colliderRadius = 0.5f;
}
