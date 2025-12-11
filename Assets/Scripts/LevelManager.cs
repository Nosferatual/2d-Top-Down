using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Diğer scriptlerden ulaşmak için

    [Header("Level Ayarları")]
    public int seviye = 1;
    public float mevcutTecrube = 0f;
    public float seviyeIcinGerekenXP = 100f;

    void Awake()
    {
        // Singleton yapısı: Her yerden LevelManager.Instance diyerek ulaşabilirsin
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TecrubeKazan(float miktar)
    {
        mevcutTecrube += miktar;
        // Debug.Log ile test et
        Debug.Log($"XP Kazanıldı! Toplam: {mevcutTecrube}/{seviyeIcinGerekenXP}");

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
        
        // Burada ilerde "Ödül Paneli Açma" fonksiyonunu çağıracağız
        // Ornek: UIManager.Instance.OdulEkraniniAc();
    }
}