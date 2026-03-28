using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerStats playerStats;

    private PlayerMovement playerMovement;

    private PlayerRunTimeStat runTimeStat;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        runTimeStat = new PlayerRunTimeStat(playerStats);
        playerMovement.Init(runTimeStat);
    }
}
