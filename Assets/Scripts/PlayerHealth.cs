using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; // Oyunu yeniden başlatmak için gerekli

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Slider healthSlider; 

    private Animator anim;
    private PlayerMovement playerMovement; // Hareket koduna erişmek için (varsa)
    private Weapon weaponScript; // Silahı kapatmak için

    // Ölüm kontrolü (üst üste ölmesin diye)
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        
        if(healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        anim = GetComponent<Animator>();
        // Eğer hareket scriptinin adı farklıysa (örn: PlayerController) burayı düzelt
        playerMovement = GetComponent<PlayerMovement>(); 
        weaponScript = GetComponentInChildren<Weapon>();
    }

    public void TakeDamage(int damage)
    {
        // Eğer zaten ölüysek hasar almayalım
        if (isDead) return;

        currentHealth -= damage;

        // UI Güncelle
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // --- ÖLÜM KONTROLÜ ---
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("OYUNCU ÖLDÜ!");

        // 1. Hareket etmeyi engelle
        if (playerMovement != null) playerMovement.enabled = false;
        
        // 2. Silahı kapat (Artık ateş edemesin)
        if (weaponScript != null) weaponScript.enabled = false;

        // 3. Fiziği durdur (Kaymasın)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero; // Unity sürümüne göre 'velocity' olabilir

        // 4. Çarpışmayı kapat (Düşmanlar cesede vurmasın)
        GetComponent<Collider2D>().enabled = false;

        // 5. Ölüm Animasyonunu Oynat
        if (anim != null)
        {
            anim.SetTrigger("Die"); // Animator'da bu Trigger'ı oluşturman lazım!
        }

        // 6. Oyun Bitti Ekranı veya Restart (Şimdilik 2 saniye sonra restart atar)
        Invoke(nameof(RestartGame), 2f);
    }

    void RestartGame()
    {
        // Şu anki sahneyi baştan yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}