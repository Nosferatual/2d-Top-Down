using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    public Transform target;

    [Header("Hareket")]
    public float moveSpeed = 2f;

    [Header("Saldırı Ayarları")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public int damageAmount = 10;
    public float damageDelay = 0.3f;

    [Header("Düşman Ayrışma")]
    [Tooltip("Vampire Survivors gibi yumuşak ayrışma — çok yüksek yapma")]
    public float separationRadius = 0.7f;
    public float separationForce = 1.5f;  // Eskiden 4f — azalttık
    public LayerMask enemyLayer;

    [Header("Sprite")]
    public SpriteRenderer spriteRenderer; // Inspector'dan ata, yoksa otomatik bulur

    Rigidbody2D rb;
    Animator animator;
    PlayerHealth playerHealth;
    bool canAttack = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        if (!spriteRenderer)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        if (!target)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p)
            {
                target = p.transform;
                playerHealth = p.GetComponentInParent<PlayerHealth>();
                if (playerHealth == null) playerHealth = p.GetComponentInChildren<PlayerHealth>();
                if (playerHealth == null) playerHealth = FindAnyObjectByType<PlayerHealth>();
            }
        }
    }

    void FixedUpdate()
    {
        if (!target) return;

        float distance = Vector2.Distance(transform.position, target.position);
        Vector2 separation = GetSeparationVector();

        if (distance > attackRange)
        {
            Vector2 toPlayer = (target.position - transform.position).normalized;
            rb.linearVelocity = toPlayer * moveSpeed + separation;
        }
        else
        {
            // Menzilde — sadece hafif separation, player'a dönük kal
            rb.linearVelocity = separation * 0.3f;
            if (canAttack) StartCoroutine(AttackRoutine());
        }

        // Flip: SADECE player'ın sağda/solda olmasına göre — separation yönüne bakma
        if (spriteRenderer && target)
        {
            float dirX = target.position.x - transform.position.x;
            // Eğer sprite varsayılan olarak sağa bakıyorsa:
            spriteRenderer.flipX = dirX < 0f;
            // Eğer sprite varsayılan olarak sola bakıyorsa üstteki satırı şununla değiştir:
            // spriteRenderer.flipX = dirX > 0f;
        }
    }

    Vector2 GetSeparationVector()
    {
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(
            transform.position, separationRadius, enemyLayer);

        Vector2 sep = Vector2.zero;
        int count = 0;

        foreach (var col in neighbors)
        {
            if (col.gameObject == gameObject) continue;
            Vector2 away = (Vector2)(transform.position - col.transform.position);
            float dist = away.magnitude;
            if (dist < 0.01f) { away = Random.insideUnitCircle.normalized; dist = 0.01f; }
            float strength = 1f - (dist / separationRadius);
            sep += away.normalized * strength;
            count++;
        }

        if (count > 0) sep /= count;
        return sep * separationForce;
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;

        if (animator) animator.SetTrigger("Attack");

        // Ses hemen çal — delay bekleme
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayEnemyAttack();

        yield return new WaitForSeconds(damageDelay);

        if (target && Vector2.Distance(transform.position, target.position) <= attackRange + 0.4f)
        {
            if (playerHealth == null)
            {
                playerHealth = target.GetComponentInParent<PlayerHealth>();
                if (playerHealth == null) playerHealth = FindAnyObjectByType<PlayerHealth>();
            }
            if (playerHealth != null)
                playerHealth.TakeDamage(damageAmount);
        }

        yield return new WaitForSeconds(attackCooldown - damageDelay);
        canAttack = true;
    }
}