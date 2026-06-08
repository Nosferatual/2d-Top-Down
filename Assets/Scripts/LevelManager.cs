using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Ayarları")]
    public int seviye = 1;
    public float mevcutTecrube = 0f;
    public float seviyeIcinGerekenXP = 100f;

    [Header("UI Elemanları")]
    public Slider xpSlider;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI speedText;

    [Header("Level Up Ekranı")]
    public GameObject levelUpPanel;             // Kararan arka plan + butonlar
    public CanvasGroup levelUpCanvasGroup;      // Fade için
    public TextMeshProUGUI levelUpTitleText;    // "LEVEL 2!" yazısı
    public float fadeDuration = 0.25f;

    [Header("Level Up Seçenekleri - Butonlar")]
    public Button moveSpeedButton;     // Yürüme hızı butonu
    public Button attackSpeedButton;   // Atış hızı butonu

    [Header("Buton Yazıları")]
    public TextMeshProUGUI moveSpeedButtonText;
    public TextMeshProUGUI attackSpeedButtonText;

    [Header("Ödül Miktarları")]
    public float hareketHiziBonusu = 0.5f;   // Her seçimde +0.5 hız
    public float saldiriHiziBonusu = 0.2f;   // Her seçimde +0.2 atış hızı

    private Weapon playerWeapon;
    private PlayerController playerController;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        playerWeapon = FindAnyObjectByType<Weapon>();
        playerController = FindAnyObjectByType<PlayerController>();

        // Level up paneli başta kapalı
        if (levelUpPanel) levelUpPanel.SetActive(false);
        if (levelUpCanvasGroup) levelUpCanvasGroup.alpha = 0f;

        UI_Guncelle();
        GuncelleButonYazilari();
    }

    public void TecrubeKazan(float miktar)
    {
        mevcutTecrube += miktar;
        UI_Guncelle();

        if (mevcutTecrube >= seviyeIcinGerekenXP)
            LevelAtla();
    }

    void LevelAtla()
    {
        mevcutTecrube -= seviyeIcinGerekenXP;
        seviye++;
        seviyeIcinGerekenXP *= 1.2f;

        // Level up sesi
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayLevelUp();

        UI_Guncelle();
        StartCoroutine(AcLevelUpEkrani());
    }

    IEnumerator AcLevelUpEkrani()
    {
        // Oyunu durdur
        Time.timeScale = 0f;

        if (levelUpTitleText) levelUpTitleText.text = $"LEVEL {seviye}!";

        levelUpPanel.SetActive(true);
        GuncelleButonYazilari();

        // Fade in — unscaledDeltaTime çünkü timeScale = 0
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            if (levelUpCanvasGroup)
                levelUpCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        if (levelUpCanvasGroup) levelUpCanvasGroup.alpha = 1f;
    }

    void KapatLevelUpEkrani()
    {
        StartCoroutine(KapatRoutine());
    }

    IEnumerator KapatRoutine()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            if (levelUpCanvasGroup)
                levelUpCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // Oyunu devam ettir
    }

    // Yürüme hızı seçildi
    public void OnMoveSpeedSelected()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        if (playerController != null)
            playerController.moveSpeed += hareketHiziBonusu;

        UI_Guncelle();
        KapatLevelUpEkrani();
    }

    // Atış hızı seçildi
    public void OnAttackSpeedSelected()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        if (playerWeapon != null)
            playerWeapon.IncreaseAttackSpeed(saldiriHiziBonusu);

        UI_Guncelle();
        KapatLevelUpEkrani();
    }

        void GuncelleButonYazilari()
    {
        float mevcutHiz = playerController != null ? playerController.moveSpeed : 0f;
        float mevcutAtis = playerWeapon != null ? playerWeapon.attackSpeedMultiplier : 1f;

        if (moveSpeedButtonText)
            moveSpeedButtonText.text = $"Move Speed\n{mevcutHiz:F1} -> {mevcutHiz + hareketHiziBonusu:F1}";

        if (attackSpeedButtonText)
            attackSpeedButtonText.text = $"Attack Speed\n{mevcutAtis:F1}x -> {mevcutAtis + saldiriHiziBonusu:F1}x";
    }

    void UI_Guncelle()
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = seviyeIcinGerekenXP;
            xpSlider.value = mevcutTecrube;
        }

        if (xpText != null)
            xpText.text = $"{mevcutTecrube:F0} / {seviyeIcinGerekenXP:F0}";

        if (levelText != null)
            levelText.text = $"LEVEL {seviye}";

        if (speedText != null && playerWeapon != null)
            speedText.text = $"HIZ: {playerWeapon.attackSpeedMultiplier:F1}x";
    }
}