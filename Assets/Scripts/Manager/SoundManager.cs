using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배경음(BGM)의 종류를 구분하기 위한 Enum.
/// 상황(타이틀, 게임, 보스전 등)에 따라 다른 BGM을 재생하기 위해 사용한다.
/// </summary>
public enum BgmType
{
    Game,
}

/// <summary>
/// 효과음(SFX)의 종류를 구분하기 위한 Enum.
/// 버튼 클릭, 공격, 피격 등 다양한 이벤트에 따라 사운드를 재생하기 위해 사용한다.
/// </summary>
public enum SfxType
{
    PlayerShoot,
    EnemyHit,
    EnemyDead,
    LevelUp,
    PlayerHit,
    PlayerDie
}

public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// BGM 볼륨을 PlayerPrefs에 저장할 때 사용하는 키.
    /// </summary>
    private const string BGM_VOLUME_KEY = "BGM_VOLUME";
    /// <summary>
    /// SFX 볼륨을 PlayerPrefs에 저장할 때 사용하는 키.
    /// </summary>
    private const string SFX_VOLUME_KEY = "SFX_VOLUME";

    /// <summary>
    /// 싱글톤 인스턴스.
    /// 다른 스크립트에서 SoundManager.Instance 로 접근한다.
    /// </summary>
    public static SoundManager Instance { get; private set; }

    /// <summary>
    /// BGM 데이터 (Enum ↔ AudioClip 연결용).
    /// Inspector에서 설정한다.
    /// </summary>
    [System.Serializable]
    public class BgmData
    {
        public BgmType type;
        public AudioClip clip;
    }

    /// <summary>
    /// SFX 데이터 (Enum ↔ AudioClip 연결용).
    /// Inspector에서 설정한다.
    /// </summary>
    [System.Serializable]
    public class SfxData
    {
        public SfxType type;
        public AudioClip clip;
    }

    /// <summary>
    /// BGM 재생용 AudioSource.
    /// 루프 재생을 기본으로 사용한다.
    /// </summary>
    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource bgmSource;

    /// <summary>
    /// 효과음(SFX) 재생용 AudioSource.
    /// PlayOneShot 방식으로 사용한다.
    /// </summary>
    [SerializeField]
    private AudioSource sfxSource;

    /// <summary>
    /// BGM 데이터 리스트.
    /// Enum과 AudioClip을 매핑하기 위해 사용한다.
    /// </summary>
    [Header("Audio Clip Data")]
    [SerializeField]
    private List<BgmData> bgmList = new();

    /// <summary>
    /// SFX 데이터 리스트.
    /// Enum과 AudioClip을 매핑하기 위해 사용한다.
    /// </summary>
    [SerializeField]
    private List<SfxData> sfxList = new();

    /// <summary>
    /// 현재 BGM 볼륨 값 (0 ~ 1).
    /// Inspector 기본값은 1로 설정되어 있다.
    /// </summary>
    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField]
    private float bgmVolume = 1f;

    /// <summary>
    /// 현재 SFX 볼륨 값 (0 ~ 1).
    /// </summary>
    [Range(0f, 1f)]
    [SerializeField]
    private float sfxVolume = 1f;

    /// <summary>
    /// BGM Enum → AudioClip 매핑 딕셔너리.
    /// 런타임에서 빠르게 찾기 위해 List를 Dictionary로 변환한다.
    /// </summary>
    private Dictionary<BgmType, AudioClip> bgmDict;

    /// <summary>
    /// SFX Enum → AudioClip 매핑 딕셔너리.
    /// </summary>
    private Dictionary<SfxType, AudioClip> sfxDict;

    /// <summary>
    /// 현재 재생 중인 BGM 타입.
    /// 동일한 BGM 중복 재생을 막기 위해 사용한다.
    /// </summary>
    private BgmType? currentBgmType = null;

    /// <summary>
    /// 싱글톤 초기화.
    /// 이미 존재하는 인스턴스가 있으면 중복 생성을 막고 자신을 파괴한다.
    /// 이후 DontDestroyOnLoad로 씬 전환 시에도 유지한다.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Init();
    }

    /// <summary>
    /// 사운드 매니저 초기화 함수.
    /// Dictionary 생성, AudioSource 설정, 저장된 볼륨 로드를 담당한다.
    /// </summary>
    private void Init()
    {
        // AudioSource가 없으면 사운드를 재생할 수 없으므로 종료
        if (bgmSource == null || sfxSource == null)
        {
            return;
        }

        bgmDict = new Dictionary<BgmType, AudioClip>();
        sfxDict = new Dictionary<SfxType, AudioClip>();

        // BGM 리스트를 Dictionary로 변환
        foreach (var data in bgmList)
        {
            if (data.clip == null)
            {
                continue;
            }

            // 중복 타입 방지
            if (bgmDict.ContainsKey(data.type))
            {
                continue;
            }

            bgmDict.Add(data.type, data.clip);
        }

        // SFX 리스트를 Dictionary로 변환
        foreach (var data in sfxList)
        {
            if (data.clip == null)
            {
                continue;
            }

            if (sfxDict.ContainsKey(data.type))
            {
                continue;
            }

            sfxDict.Add(data.type, data.clip);
        }

        // AudioSource 기본 설정
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        // 초기 볼륨 적용 (Inspector 값 기준)
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;

        // 저장된 볼륨 값이 있으면 덮어씌운다.
        LoadVolume();
    }

    /// <summary>
    /// 현재 볼륨 값을 PlayerPrefs에 저장하는 함수.
    /// 게임 종료 후에도 설정을 유지하기 위해 사용한다.
    /// </summary>
    private void SaveVolume()
    {
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, bgmVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// PlayerPrefs에 저장된 볼륨 값을 불러오는 함수.
    /// 저장된 값이 없으면 기본값(1f)을 사용한다.
    /// </summary>
    private void LoadVolume()
    {
        bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }

    /// <summary>
    /// BGM을 재생하는 함수.
    /// 이미 같은 BGM이 재생 중이면 중복 재생을 막는다.
    /// </summary>
    public void PlayBgm(BgmType type)
    {
        if (currentBgmType.HasValue && currentBgmType.Value == type && bgmSource.isPlaying)
            return;

        if (bgmDict.TryGetValue(type, out AudioClip clip) == false)
        {
            return;
        }

        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.loop = true;
        bgmSource.Play();

        currentBgmType = type;
    }

    /// <summary>
    /// 현재 재생 중인 BGM을 정지하고 상태를 초기화하는 함수.
    /// </summary>
    public void StopBgm()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
        currentBgmType = null;
    }

    /// <summary>
    /// 효과음을 재생하는 함수.
    /// PlayOneShot을 사용하여 기존 사운드를 끊지 않고 동시에 재생한다.
    /// </summary>
    public void PlaySfx(SfxType type)
    {
        if (sfxDict.TryGetValue(type, out AudioClip clip) == false)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    /// <summary>
    /// BGM 볼륨을 설정하는 함수.
    /// 값을 0~1 범위로 제한하고, AudioSource에 즉시 반영 후 저장한다.
    /// </summary>
    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;

        SaveVolume();
    }

    /// <summary>
    /// SFX 볼륨을 설정하는 함수.
    /// </summary>
    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;

        SaveVolume();
    }

    /// <summary>
    /// 현재 BGM 볼륨 값을 반환한다.
    /// UI 초기화 시 사용된다.
    /// </summary>
    public float GetBgmVolume()
    {
        return bgmVolume;
    }

    /// <summary>
    /// 현재 SFX 볼륨 값을 반환한다.
    /// </summary>
    public float GetSfxVolume()
    {
        return sfxVolume;
    }
}
