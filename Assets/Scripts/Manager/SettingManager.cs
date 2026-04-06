using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class SettingManager : MonoBehaviour
{
    [SerializeField]
    private Slider bgmSlider;

    [SerializeField]
    private Slider sfxSlider;

    private void Start()
    {
        InitSoundUI();
        BindUIEvents();
    }

    private void InitSoundUI()
    {
        if (SoundManager.Instance == null)
            return;

        if (bgmSlider != null)
            bgmSlider.value = SoundManager.Instance.GetBgmVolume();

        if (sfxSlider != null)
            sfxSlider.value = SoundManager.Instance.GetSfxVolume();
    }

    private void BindUIEvents()
    {
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    public void OnBgmVolumeChanged(float volume)
    {
        if (SoundManager.Instance == null)
            return;

        SoundManager.Instance.SetBgmVolume(volume);
    }

    public void OnSfxVolumeChanged(float volume)
    {
        if (SoundManager.Instance == null)
            return;

        SoundManager.Instance.SetSfxVolume(volume);
    }

    public void SettingOn()
    {
        Time.timeScale = 0f;
        gameObject.SetActive(true);
    }

    public void SettingClose()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}