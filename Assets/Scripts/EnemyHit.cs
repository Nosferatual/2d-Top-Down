using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    [Header("Ayarlar")]
    public float xpMiktari = 10f;
    public float saglik = 10f; // DİKKAT: Tek vuruşta ölmesi için 10 yaptık
    public GameObject deathVfx; 

    void OnTriggerEnter2D(Collider2D other)
    {
        // Mermi çarptı mı?
        if (other.CompareTag("Bullet"))
        {
            // Merminin hasarını al (Bullet scripti yoksa 10 varsay)
            Bullet mermi = other.GetComponent<Bullet>();
            float gelenHasar = (mermi != null) ? mermi.damage : 10f;

            HasarAl(gelenHasar);
            
            // Çarpan mermiyi yok et
            Destroy(other.gameObject);
        }
    }

    void HasarAl(float miktar)
    {
        saglik -= miktar;
        
        // Sağlık bittiyse ölüm fonksiyonunu çağır
        if (saglik <= 0)
        {
            Olum();
        }
    }

    void Olum()
    {
        // 1. XP Ver
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.TecrubeKazan(xpMiktari);
        }

        // 2. Efekt (Varsa)
        if (deathVfx) Instantiate(deathVfx, transform.position, Quaternion.identity);

        // 3. Düşmanı Sil
        Destroy(gameObject);
    }
}