using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private Slider playerHpSlider;
    [SerializeField]
    private Slider playerExpSlider;


    private void Update()
    {
        playerHpSlider.value = player.CurrentHp / player.runTimeStat.GetStat(StatType.PlayerHp);
        playerExpSlider.value = player.runTimeStat.GetExpProgress();
    }
}
