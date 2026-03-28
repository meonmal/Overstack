using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// 플레이어 스탯의 원본 SO
    /// </summary>
    [SerializeField]
    private PlayerStats playerStats;

    /// <summary>
    /// 플레이어 이동 스크립트.
    /// </summary>
    private PlayerMovement playerMovement;

    /// <summary>
    /// 플레이어 런타임 스탯.
    /// </summary>
    private PlayerRunTimeStat runTimeStat;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        runTimeStat = new PlayerRunTimeStat(playerStats);
        playerMovement.Init(runTimeStat);
    }
}
