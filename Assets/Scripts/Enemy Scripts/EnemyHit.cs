using UnityEngine;
using System.Collections;

public class EnemyHit : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int maxHealth = 30;
    public int currentHealth;

    [Header("Ölüm Efekti")]
    public GameObject deathVfx;
    public float effectDuration = 1f;

    [Header("Vurulma Fizik")]
    public float knockbackForce = 3f;
    public float stunTime = 0.12f;

    [Header("Vurulma Flash")]
    public float flashDuration = 0.1f;
    public Color hitColor = Color.red;

    Rigidbody2D rb;
    EnemyChase chaseScript;
    SpriteRenderer[] renderers;
    Color[] originalColors;
    bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        chaseScript = GetComponent<EnemyChase>();

        renderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].color;
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (gameObject.activeInHierarchy)
            StartCoroutine(KnockbackRoutine(hitDirection));

        if (currentHealth <= 0)
        {
            isDead = true;
            // Flash ÖNCE çalışsın, SONRA öl — son canda da kırmızı görsün
            StartCoroutine(FlashThenDie());
        }
        else
        {
            StartCoroutine(HitFlash());
        }
    }

    IEnumerator KnockbackRoutine(Vector2 dir)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(stunTime);
        if (!isDead) rb.linearVelocity = Vector2.zero;
    }

    IEnumerator HitFlash()
    {
        SetColor(hitColor);
        yield return new WaitForSeconds(flashDuration);
        ResetColor();
    }

    IEnumerator FlashThenDie()
    {
        // Collider kapat — çift hasar/ölüm olmasın
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        if (chaseScript) chaseScript.enabled = false;
        if (rb) rb.linearVelocity = Vector2.zero;

        // Flash — son canda da görünsün
        SetColor(hitColor);
        yield return new WaitForSeconds(flashDuration * 2f);
        ResetColor();

        Die();
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

    void Die()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.TecrubeKazan(20);

        if (deathVfx != null)
        {
            GameObject fx = Instantiate(deathVfx, transform.position, Quaternion.identity);
            Destroy(fx, effectDuration);
        }

        Destroy(gameObject);
    }
}