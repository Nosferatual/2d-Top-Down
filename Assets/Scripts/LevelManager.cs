using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Ayarları")]
    public int seviye = 1;
    public float mevcutTecrube = 0f;
    public float seviyeIcinGerekenXP = 100f;

    [Header("Ödül Ayarları")]
    public float saldiriHiziBonusu = 0.2f; // Her levelde %20 hızlansın

    // Silah scriptine ulaşmak için referans
    private Weapon playerWeapon;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Sahnedeki oyuncunun silahını (Weapon scriptini) bul
        playerWeapon = FindAnyObjectByType<Weapon>();
        
        if(playerWeapon == null)
            Debug.LogWarning("LevelManager: Weapon scripti sahnede bulunamadı!");
    }

    public void TecrubeKazan(float miktar)
    {
        mevcutTecrube += miktar;
        // Debug.Log($"XP Kazanıldı! Toplam: {mevcutTecrube}/{seviyeIcinGerekenXP}");

        if (mevcutTecrube >= seviyeIcinGerekenXP)
        {
            LevelAtla();
        }
    }

    void LevelAtla()
    {
        mevcutTecrube -= seviyeIcinGerekenXP;
        seviye++;
        seviyeIcinGerekenXP *= 1.2f; // Her levelda zorlaşsın

        Debug.Log($"<color=green>TEBRİKLER! LEVEL {seviye} OLDUN!</color>");

        // --- SİLAHI HIZLANDIRMA KODU ---
        if (playerWeapon != null)
        {
            playerWeapon.IncreaseAttackSpeed(saldiriHiziBonusu);
        }
    }
}