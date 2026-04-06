using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpSystem : MonoBehaviour
{
    /// <summary>
    /// 레벨업 선택지 버튼 배열.
    /// 각 버튼은 하나의 업그레이드 선택지를 의미한다.
    /// Inspector에서 순서대로 연결해야 한다.
    /// </summary>
    [SerializeField]
    private Button[] buttons;

    /// <summary>
    /// 각 선택지에 표시될 아이콘 이미지 배열.
    /// buttons와 같은 인덱스 순서를 가져야 한다.
    /// </summary>
    [SerializeField]
    private Image[] icons;

    /// <summary>
    /// 각 선택지의 제목 텍스트 배열.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI[] titles;

    /// <summary>
    /// 각 선택지의 상세 설명 텍스트 배열.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI[] descs;

    /// <summary>
    /// 플레이어 스탯 UI 표시용 데이터 목록.
    /// </summary>
    [SerializeField]
    private List<StatUIData> statUIDatas;

    /// <summary>
    /// 레벨업 대상 플레이어 참조.
    /// </summary>
    private Player player;

    /// <summary>
    /// 플레이어 StatType -> UI 데이터 매핑용 Dictionary.
    /// </summary>
    private Dictionary<StatType, StatUIData> statUIMap;

    /// <summary>
    /// 현재 화면에 표시 중인 선택지 목록.
    /// </summary>
    private List<LevelUpOption> currentOptions = new List<LevelUpOption>();

    private void Awake()
    {
        EnsureInitialized();
    }

    public void Init(Player player)
    {
        this.player = player;
    }

    private void EnsureInitialized()
    {
        if (statUIMap != null)
        {
            return;
        }

        statUIMap = new Dictionary<StatType, StatUIData>();

        if (statUIDatas == null)
        {
            Debug.LogWarning("LevelUpSystem : statUIDatas가 비어 있음");
            return;
        }

        foreach (StatUIData data in statUIDatas)
        {
            if (data == null)
            {
                continue;
            }

            if (statUIMap.ContainsKey(data.statType))
            {
                Debug.LogWarning($"LevelUpSystem : {data.statType} UI 데이터가 중복 등록됨");
                continue;
            }

            statUIMap.Add(data.statType, data);
        }
    }

    public void Open()
    {
        EnsureInitialized();

        if (player == null)
        {
            Debug.LogWarning("LevelUpSystem : Player가 Init되지 않았음");
            return;
        }

        if (player.runTimeStat == null)
        {
            Debug.LogWarning("LevelUpSystem : player.runTimeStat이 null임");
            return;
        }

        List<LevelUpOption> options = GetRandomOptions();

        if (options.Count == 0)
        {
            Debug.Log("모든 스탯이 최대 레벨임");
            return;
        }

        currentOptions = options;
        SetButtons(currentOptions);

        SoundManager.Instance.PlaySfx(SfxType.LevelUp);
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private List<LevelUpOption> GetRandomOptions()
    {
        List<LevelUpOption> candidates = new List<LevelUpOption>();

        /// 플레이어 스탯 후보 추가
        List<StatType> playerStats = player.runTimeStat.GetAvailableStats();

        foreach (StatType statType in playerStats)
        {
            if (!statUIMap.ContainsKey(statType))
            {
                Debug.LogWarning($"LevelUpSystem : {statType} UI 데이터가 없음");
                continue;
            }

            StatUIData data = statUIMap[statType];

            LevelUpOption option = new LevelUpOption();
            option.targetType = LevelUpTargetType.Player;
            option.playerStatType = statType;
            option.title = data.title;
            option.icon = data.icon;
            option.currentValue = player.runTimeStat.GetBaseStat(statType);
            option.nextValue = player.runTimeStat.GetNextStat(statType);
            option.deltaValue = player.runTimeStat.GetDelta(statType);

            candidates.Add(option);
        }

        /// 무기 스탯 후보 추가
        foreach (WeaponBase weapon in player.Weapons)
        {
            if (weapon == null || weapon.RunTimeStat == null)
            {
                continue;
            }

            List<WeaponStatType> weaponStats = weapon.RunTimeStat.GetAvailableStats();

            foreach (WeaponStatType statType in weaponStats)
            {
                LevelUpOption option = new LevelUpOption();
                option.targetType = LevelUpTargetType.Weapon;
                option.weaponStatType = statType;
                option.targetWeapon = weapon;
                option.title = $"{weapon.WeaponName} - {statType}";
                option.icon = weapon.WeaponIcon;
                option.currentValue = weapon.RunTimeStat.GetStat(statType);
                option.nextValue = weapon.RunTimeStat.GetNextStat(statType);
                option.deltaValue = weapon.RunTimeStat.GetDeltaStat(statType);

                candidates.Add(option);
            }
        }

        Shuffle(candidates);

        int count = Mathf.Min(buttons.Length, candidates.Count);

        List<LevelUpOption> result = new List<LevelUpOption>();

        for (int i = 0; i < count; i++)
        {
            result.Add(candidates[i]);
        }

        return result;
    }

    private void Shuffle(List<LevelUpOption> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void SetButtons(List<LevelUpOption> options)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();

            if (i < options.Count)
            {
                LevelUpOption option = options[i];

                buttons[i].gameObject.SetActive(true);

                icons[i].sprite = option.icon;
                titles[i].text = option.title;

                if (option.targetType == LevelUpTargetType.Player)
                {
                    descs[i].text = BuildDescriptionText(
                        option.playerStatType,
                        option.currentValue,
                        option.nextValue,
                        option.deltaValue);
                }
                else
                {
                    descs[i].text = BuildDescriptionText(
                        option.weaponStatType,
                        option.currentValue,
                        option.nextValue,
                        option.deltaValue);
                }

                LevelUpOption selectedOption = option;
                buttons[i].onClick.AddListener(() => SelectOption(selectedOption));
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }

    private string BuildDescriptionText(StatType statType, float current, float next, float delta)
    {
        if (statType == StatType.ProjectileCount)
        {
            int currentInt = Mathf.RoundToInt(current);
            int nextInt = Mathf.RoundToInt(next);
            int deltaInt = Mathf.RoundToInt(delta);

            bool isGood = deltaInt > 0;

            string deltaText = isGood
                ? $"<color=#00FF00>+{deltaInt}</color>"
                : $"<color=#FF0000>{deltaInt}</color>";

            return $"{currentInt} → {nextInt} ({deltaText})";
        }

        bool isGoodFloat = delta > 0;

        if (statType == StatType.CoolTime)
        {
            isGoodFloat = delta < 0;
        }

        string sign = delta > 0 ? "+" : "";
        string deltaTextFloat = isGoodFloat
            ? $"<color=#00FF00>{sign}{delta:F1}</color>"
            : $"<color=#FF0000>{delta:F1}</color>";

        return $"{current:F1} → {next:F1} ({deltaTextFloat})";
    }

    private string BuildDescriptionText(WeaponStatType statType, float current, float next, float delta)
    {
        if (statType == WeaponStatType.ProjectileCount)
        {
            int currentInt = Mathf.RoundToInt(current);
            int nextInt = Mathf.RoundToInt(next);
            int deltaInt = Mathf.RoundToInt(delta);

            bool isGood = deltaInt > 0;

            string deltaText = isGood
                ? $"<color=#00FF00>+{deltaInt}</color>"
                : $"<color=#FF0000>{deltaInt}</color>";

            return $"{currentInt} → {nextInt} ({deltaText})";
        }

        bool isGoodFloat = delta > 0;

        if (statType == WeaponStatType.CoolTime)
        {
            isGoodFloat = delta < 0;
        }

        string sign = delta > 0 ? "+" : "";
        string deltaTextFloat = isGoodFloat
            ? $"<color=#00FF00>{sign}{delta:F1}</color>"
            : $"<color=#FF0000>{delta:F1}</color>";

        return $"{current:F1} → {next:F1} ({deltaTextFloat})";
    }

    private void SelectOption(LevelUpOption option)
    {
        if (player == null || player.runTimeStat == null)
        {
            Debug.LogWarning("LevelUpSystem : Player 또는 runTimeStat이 없음");
            return;
        }

        if (option.targetType == LevelUpTargetType.Player)
        {
            player.runTimeStat.LevelUp(option.playerStatType);
            player.RefreshStatsByLevelUp(option.playerStatType);
        }
        else if (option.targetType == LevelUpTargetType.Weapon)
        {
            if (option.targetWeapon == null || option.targetWeapon.RunTimeStat == null)
            {
                Debug.LogWarning("LevelUpSystem : targetWeapon 또는 RunTimeStat이 null임");
                return;
            }

            option.targetWeapon.RunTimeStat.LevelUpStat(option.weaponStatType);
            option.targetWeapon.RefreshStatByLevelUp(option.weaponStatType);
        }

        Close();
    }
}