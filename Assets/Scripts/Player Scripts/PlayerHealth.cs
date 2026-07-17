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
    public float invincibilityDuration = 0.5f;
    public int enemyLayer = 7;

    [Header("Flash Efekti")]
    public int flashCount = 3;
    public float flashInterval = 0.08f;

    [Header("Ekran Sarsıntısı")]
    public float shakeMagnitude = 0.15f;
    public float shakeDuration  = 0.3f;

    [Header("UI")]
    public Slider healthSlider;

    PlayerAnimation playerAnim;   // Animator yerine PlayerAnimation kullanıyoruz
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

        // PlayerAnimation root objesinde veya child'da
        playerAnim = GetComponent<PlayerAnimation>();
        if (playerAnim == null) playerAnim = GetComponentInChildren<PlayerAnimation>();

        playerMovement = GetComponent<PlayerMovement>();
        weaponScript = GetComponentInChildren<Weapon>();

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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPlayerHurt();

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(shakeMagnitude, shakeDuration);

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(InvincibilityRoutine());
    }

    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayer, true);

        for (int i = 0; i < flashCount; i++)
        {
            SetColor(Color.white);
            yield return new WaitForSeconds(flashInterval);
            ResetColor();
            yield return new WaitForSeconds(flashInterval);
        }

        float elapsed = flashCount * flashInterval * 2f;
        float remaining = invincibilityDuration - elapsed;
        if (remaining > 0f)
        {
            SetAlpha(0.5f);
            yield return new WaitForSeconds(remaining);
            SetAlpha(1f);
        }

        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayer, false);
        isInvincible = false;
    }

    void SetColor(Color c)
    {
        foreach (var r in renderers)
            if (r) r.color = c;
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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameOver();

        if (playerMovement != null) playerMovement.enabled = false;
        if (weaponScript != null) weaponScript.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        GetComponent<Collider2D>().enabled = false;

        // PlayerAnimation üzerinden death trigger'ı tetikle
        if (playerAnim != null)
            playerAnim.TriggerDeath();

        Invoke(nameof(RestartGame), 2f);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}