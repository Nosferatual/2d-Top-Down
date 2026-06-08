using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class XpOrb : MonoBehaviour
{
    [Header("XP")]
    public float xpAmount = 20f;

    [Header("Hareket")]
    public float moveSpeed = 6f;
    public float attractRadius = 3f;    // Bu mesafede player'a doğru çekil
    public float pickupRadius = 0.6f;   // Bu mesafede topla
    public float lifetime = 8f;
    public float scatterForce = 2f;     // Spawn anında dağılma kuvveti

    [Header("Renk Geçişi")]
    public Color colorA = new Color(1f, 0.9f, 0f);   // Sarı
    public Color colorB = new Color(0f, 1f, 0.4f);   // Yeşil
    public float pulseSpeed = 3f;                     // Geçiş hızı

    Rigidbody2D rb;
    SpriteRenderer sr;
    Transform player;
    bool isAttracting = false;
    float timer;
    float colorTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // Fizik ayarları
        rb.gravityScale = 0f;
        rb.linearDamping = 3f;          // Sürüklenme — yavaşça dursun
        rb.freezeRotation = true;

        // Collider — orb'lar birbirini itsin ama trigger değil
        var col = GetComponent<CircleCollider2D>();
        col.radius = 0.2f;
        col.isTrigger = false;          // Fiziksel çarpışma olsun
    }

    void Start()
    {
        timer = lifetime;

        // Player'ı bul
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        // Spawn anında rastgele yönde dağıl
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        if (randomDir == Vector2.zero) randomDir = Vector2.up;
        rb.AddForce(randomDir * scatterForce, ForceMode2D.Impulse);
    }

    void Update()
    {
        // Lifetime
        timer -= Time.deltaTime;
        if (timer <= 0f) { Destroy(gameObject); return; }

        // Renk pulse — A ile B arasında gidip gel
        colorTimer += Time.deltaTime * pulseSpeed;
        float t = (Mathf.Sin(colorTimer) + 1f) / 2f; // 0-1 arası
        if (sr) sr.color = Color.Lerp(colorA, colorB, t);
    }

    void FixedUpdate()
    {
        if (!player) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attractRadius) isAttracting = true;

        if (isAttracting)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            float speed = moveSpeed * (attractRadius / Mathf.Max(dist, 0.1f));
            rb.linearVelocity = dir * speed;

            if (dist <= pickupRadius) Collect();
        }
    }

    void Collect()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.TecrubeKazan(xpAmount);
        Destroy(gameObject);
    }
}