using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Slider")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField, Range(0.01f, 1f)]
    private float minimumUnmuteVolume = 0.1f;

    [Header("Mute All")]
    [SerializeField] private Toggle muteAllToggle;

    [Header("Ikon Musik")]
    [SerializeField] private Image musicSpeakerImage;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;

    [Header("Ikon SFX")]
    [SerializeField] private Image sfxSpeakerImage;
    [SerializeField] private Sprite sfxOnSprite;
    [SerializeField] private Sprite sfxOffSprite;
    
    [Header("Pause Panel")]
    [SerializeField] private Slider pauseMusicSlider;

    private void OnEnable()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError(
                "[AudioSettingsUI] AudioManager tidak ditemukan."
            );

            return;
        }

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(
                AudioManager.Instance.GetMusicVolume()
            );
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(
                AudioManager.Instance.GetSFXVolume()
            );
        }

        if (muteAllToggle != null)
        {
            RefreshMuteAllToggle();
        }

        UpdateIcons();
    }

    public void OnMusicSliderChanged(float value)
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.SetMusicVolume(value);

        UpdateIcons();
        RefreshMuteAllToggle();
        SyncPauseMusicSlider();
    }

    public void OnSFXSliderChanged(float value)
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.SetSFXVolume(value);

        UpdateIcons();
        RefreshMuteAllToggle();
    }

    public void SyncPauseMusicSlider()
    {
        if (AudioManager.Instance == null ||
            pauseMusicSlider == null)
        {
            return;
        }

        pauseMusicSlider.SetValueWithoutNotify(
            AudioManager.Instance.GetMusicVolume()
        );
    }

    public void ToggleMusicMute()
    {
        if (AudioManager.Instance == null)
            return;

        float newVolume;

        if (AudioManager.Instance.IsMuted())
        {
            newVolume =
                AudioManager.Instance.GetLastMusicVolume();

            if (newVolume <= 0f)
            {
                newVolume = 1f;
            }
        }
        else
        {
            newVolume = 0f;
        }

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(newVolume);
        }

        AudioManager.Instance.SetMusicVolume(newVolume);

        UpdateIcons();
        RefreshMuteAllToggle();
        SyncPauseMusicSlider();
    }

    public void ToggleSFXMute()
    {
        if (AudioManager.Instance == null)
            return;

        float newVolume;

        if (AudioManager.Instance.IsSFXMuted())
        {
            newVolume =
                AudioManager.Instance.GetLastSFXVolume();

            if (newVolume <= 0f)
            {
                newVolume = 1f;
            }
        }
        else
        {
            newVolume = 0f;
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(newVolume);
        }

        AudioManager.Instance.SetSFXVolume(newVolume);

        UpdateIcons();
        RefreshMuteAllToggle();
    }

    public void OnMuteAllChanged(bool isMuted)
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.SetMuteAll(isMuted);

        if (!isMuted)
        {
            // Naikkan slider musik sedikit jika masih nol.
            if (musicSlider != null &&
                AudioManager.Instance.GetMusicVolume() <= 0f)
            {
                musicSlider.SetValueWithoutNotify(
                    minimumUnmuteVolume
                );

                AudioManager.Instance.SetMusicVolume(
                    minimumUnmuteVolume
                );
            }

            // Naikkan slider SFX sedikit jika masih nol.
           if (sfxSlider != null && 
                AudioManager.Instance.GetSFXVolume() <= 0f)
            {
                sfxSlider.SetValueWithoutNotify(
                    minimumUnmuteVolume
                );

                AudioManager.Instance.SetSFXVolume(
                    minimumUnmuteVolume
                );
            }
        }

        UpdateIcons();
        RefreshMuteAllToggle();
        SyncPauseMusicSlider();
    }

    private void UpdateIcons()
    {
        if (AudioManager.Instance == null)
            return;

        bool allMuted =
            AudioManager.Instance.IsAllMuted();

        bool musicIsActive =
            !allMuted &&
            AudioManager.Instance.GetMusicVolume() > 0f;

        bool sfxIsActive =
            !allMuted &&
            AudioManager.Instance.GetSFXVolume() > 0f;

        if (musicSpeakerImage != null)
        {
            musicSpeakerImage.sprite =
                musicIsActive
                    ? musicOnSprite
                    : musicOffSprite;
        }

        if (sfxSpeakerImage != null)
        {
            sfxSpeakerImage.sprite =
                sfxIsActive
                    ? sfxOnSprite
                    : sfxOffSprite;
        }
    }

    private void RefreshMuteAllToggle()
    {
        if (AudioManager.Instance == null || muteAllToggle == null)
            return;

        bool musicMuted =
            AudioManager.Instance.GetMusicVolume() <= 0f;

        bool sfxMuted =
            AudioManager.Instance.GetSFXVolume() <= 0f;

        bool shouldBeChecked =
            AudioManager.Instance.IsAllMuted()
            || (musicMuted && sfxMuted);

        muteAllToggle.SetIsOnWithoutNotify(shouldBeChecked);
    }
}