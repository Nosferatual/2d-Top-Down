using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Hasar Sonrası Koruma")]
    public float invincibilityDuration = 0.5f;  // Kaç saniye ölümsüz kalsın
    public LayerMask enemyLayer;                 // Inspector'dan Enemy layer'ını seç

    [Header("Flash Efekti")]
    public Color hitColor = Color.white;         // Beyaz flash
    public int flashCount = 3;                   // Kaç kez yanıp sönsün
    public float flashInterval = 0.08f;

    [Header("UI")]
    public Slider healthSlider;

    Animator anim;
    PlayerMovement playerMovement;
    Weapon weaponScript;
    SpriteRenderer[] renderers;
    Color[] originalColors;
    bool isDead = false;
    bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        weaponScript = GetComponentInChildren<Weapon>();

        // Tüm sprite renderer'ları bul
        renderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].color;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        // Ghost mode: düşman collider'larıyla çarpışmayı kapat
        SetCollisionWithEnemies(false);

        // Beyaz flash — yanıp sön
        for (int i = 0; i < flashCount; i++)
        {
            SetColor(hitColor);
            yield return new WaitForSeconds(flashInterval);
            ResetColor();
            yield return new WaitForSeconds(flashInterval);
        }

        // Kalan süreyi bekle
        float elapsed = flashCount * flashInterval * 2f;
        float remaining = invincibilityDuration - elapsed;
        if (remaining > 0f)
        {
            // Yarı saydam göster — hâlâ invincible olduğunu belli et
            SetAlpha(0.5f);
            yield return new WaitForSeconds(remaining);
            SetAlpha(1f);
        }

        // Ghost mode kapat
        SetCollisionWithEnemies(true);
        isInvincible = false;
    }

    // Physics2D layer collision — düşmanlardan geç
    void SetCollisionWithEnemies(bool collide)
    {
        int playerLayer = gameObject.layer;
        // enemyLayer maskesindeki her layer ile çarpışmayı aç/kapat
        for (int i = 0; i < 32; i++)
        {
            if ((enemyLayer.value & (1 << i)) != 0)
                Physics2D.IgnoreLayerCollision(playerLayer, i, !collide);
        }
    }

    void SetColor(Color c)
    {
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i]) renderers[i].color = c;
    }

    void ResetColor()
    {
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i]) renderers[i].color = originalColors[i];
    }

    void SetAlpha(float a)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (!renderers[i]) continue;
            Color c = originalColors[i];
            c.a = a;
            renderers[i].color = c;
        }
    }

    void Die()
    {
        isDead = true;

        if (playerMovement != null) playerMovement.enabled = false;
        if (weaponScript != null) weaponScript.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        GetComponent<Collider2D>().enabled = false;

        if (anim != null) anim.SetTrigger("Die");

        Invoke(nameof(RestartGame), 2f);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}