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

    [Header("XP Orb")]
    public GameObject xpOrbPrefab;      // Inspector'dan XpOrb prefabını sürükle
    public int xpOrbCount = 3;          // Kaç orb saçılsın
    public float orbScatterRadius = 0.5f; // Ne kadar dağılsın

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
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
        if (chaseScript) chaseScript.enabled = false;
        if (rb) rb.linearVelocity = Vector2.zero;

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
        // Ölüm efekti
        if (deathVfx != null)
        {
            GameObject fx = Instantiate(deathVfx, transform.position, Quaternion.identity);
            Destroy(fx, effectDuration);
        }

        // XP orb saç
        SpawnXpOrbs();

        Destroy(gameObject);
    }

    void SpawnXpOrbs()
    {
        if (xpOrbPrefab == null) return;

        for (int i = 0; i < xpOrbCount; i++)
        {
            // Rastgele küçük offset — orblar üst üste gelmesin
            Vector2 offset = Random.insideUnitCircle * orbScatterRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0f);
            Instantiate(xpOrbPrefab, spawnPos, Quaternion.identity);
        }
    }
}