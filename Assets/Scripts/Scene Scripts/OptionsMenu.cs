using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject optionsPanel;    // Options paneli
    public GameObject mainMenuPanel;   // Ana menü paneli (Başla/Ayarlar/Çıkış)

    [Header("Sliderlar")]
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Kaydedilmiş değerleri yükle — yoksa varsayılan 0.5
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSfx   = PlayerPrefs.GetFloat("SfxVolume", 1f);

        if (musicSlider) musicSlider.value = savedMusic;
        if (sfxSlider)   sfxSlider.value   = savedSfx;

        // AudioManager'a uygula
        ApplyVolumes(savedMusic, savedSfx);

        // Başta options kapalı
        if (optionsPanel) optionsPanel.SetActive(false);
    }

    // Ayarlar butonuna basınca
    public void OnOptionsClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (optionsPanel)  optionsPanel.SetActive(true);
    }

    // Geri butonuna basınca
    public void OnBackClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        // Kaydet
        PlayerPrefs.SetFloat("MusicVolume", musicSlider ? musicSlider.value : 0.5f);
        PlayerPrefs.SetFloat("SfxVolume",   sfxSlider   ? sfxSlider.value   : 1f);
        PlayerPrefs.Save();

        if (optionsPanel)  optionsPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    // Müzik slider değişince
    public void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.musicVolume = value;
            AudioManager.Instance.musicSource.volume = value;
        }
    }

    // SFX slider değişince
    public void OnSfxVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.sfxVolume = value;
    }

    void ApplyVolumes(float music, float sfx)
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.musicVolume = music;
        AudioManager.Instance.sfxVolume   = sfx;
        AudioManager.Instance.musicSource.volume = music;
    }
}