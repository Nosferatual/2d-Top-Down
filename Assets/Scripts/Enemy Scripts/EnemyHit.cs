using UnityEngine;
using System.Collections;

public class EnemyHit : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int maxHealth = 30;
    public int currentHealth;

    [Header("Efektler")]
    public GameObject deathVfx; // İŞTE SENİN İSTEDİĞİN KISIM BURASI
    public float effectDuration = 1f; // Efekt kaç saniye sahnede kalsın?

    [Header("Vurulma Fizik")]
    public float knockbackForce = 10f; 
    public float stunTime = 0.3f;      
    
    private Rigidbody2D rb;
    private EnemyChase enemyChaseScript;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        enemyChaseScript = GetComponent<EnemyChase>();
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        currentHealth -= damage;

        // Vurulunca Geri Tepme (Knockback)
        if(this.gameObject.activeInHierarchy) 
        {
            StartCoroutine(KnockbackRoutine(hitDirection));
        }

        // Ölüm Kontrolü
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator KnockbackRoutine(Vector2 dir)
    {
        if (enemyChaseScript) enemyChaseScript.enabled = false;

        rb.linearVelocity = Vector2.zero; // Unity 2023 öncesi ise 'velocity' yaz
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(stunTime);

        rb.linearVelocity = Vector2.zero;
        if (enemyChaseScript) enemyChaseScript.enabled = true;
    }

    void Die()
    {
        // 1. Level Sistemine XP Gönder
        if (LevelManager.Instance != null) 
        {
            LevelManager.Instance.TecrubeKazan(20); 
        }

        // 2. DEAD FX (ÖLÜM EFEKTİ) OLUŞTURMA
        if (deathVfx != null)
        {
            // Düşmanın olduğu yerde efekti oluştur
            GameObject effect = Instantiate(deathVfx, transform.position, Quaternion.identity);
            
            // Efekti belirli bir süre sonra yok et (sahne çöp dolmasın)
            Destroy(effect, effectDuration);
        }

        // 3. Düşmanı Yok Et
        Destroy(gameObject); 
    }
}