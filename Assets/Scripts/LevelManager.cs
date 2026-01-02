using UnityEngine;
using UnityEngine.UI; // Slider için
using TMPro;          // TextMeshPro yazıları için (BUNU UNUTMA!)

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Ayarları")]
    public int seviye = 1;
    public float mevcutTecrube = 0f;
    public float seviyeIcinGerekenXP = 100f;

    [Header("UI Elemanları")]
    public Slider xpSlider;
    public TextMeshProUGUI xpText;    // "50 / 100" yazan yer
    public TextMeshProUGUI levelText; // "Level 5" yazan yer
    public TextMeshProUGUI speedText; // "Hız: 1.2x" yazan yer

    [Header("Ödül Ayarları")]
    public float saldiriHiziBonusu = 0.2f;

    private Weapon playerWeapon;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Silahı bul
        playerWeapon = FindAnyObjectByType<Weapon>();
        
        // Oyun başlayınca ekranı güncelle
        UI_Guncelle();
    }

    public void TecrubeKazan(float miktar)
    {
        mevcutTecrube += miktar;
        
        // XP gelince güncelle
        UI_Guncelle();

        if (mevcutTecrube >= seviyeIcinGerekenXP)
        {
            LevelAtla();
        }
    }

    void LevelAtla()
    {
        mevcutTecrube -= seviyeIcinGerekenXP;
        seviye++;
        seviyeIcinGerekenXP *= 1.2f; // Bir sonraki level zorlaşsın

        Debug.Log($"TEBRİKLER! LEVEL {seviye} OLDUN!");

        // Silahı hızlandır
        if (playerWeapon != null)
        {
            playerWeapon.IncreaseAttackSpeed(saldiriHiziBonusu);
        }

        // Level atlayınca her şeyi güncelle
        UI_Guncelle();
    }

    // Tüm UI işlemlerini tek yerde yapıyoruz, kafa karışıklığı olmasın
    void UI_Guncelle()
    {
        // 1. Slider Güncelle
        if (xpSlider != null)
        {
            xpSlider.maxValue = seviyeIcinGerekenXP;
            xpSlider.value = mevcutTecrube;
        }

        // 2. XP Yazısını Güncelle (Örn: "40 / 120")
        // "F0" virgülden sonra sayı gösterme demek (tam sayı)
        if (xpText != null)
        {
            xpText.text = $"{mevcutTecrube:F0} / {seviyeIcinGerekenXP:F0}";
        }

        // 3. Level Yazısını Güncelle
        if (levelText != null)
        {
            levelText.text = $"LEVEL {seviye}";
        }

        // 4. Hız Çarpanını Göster (Silahın üzerinden okuyoruz)
        if (speedText != null && playerWeapon != null)
        {
            // "F1" virgülden sonra tek basamak göster demek (1.2x gibi)
            speedText.text = $"HIZ: {playerWeapon.attackSpeedMultiplier:F1}x";
        }
    }
}