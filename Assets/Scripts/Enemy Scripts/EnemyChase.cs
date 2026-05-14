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
    [Tooltip("Bu değeri düşmanın collider yarıçapının 2 katı yap. Örn collider radius 0.4 ise bunu 0.8 yap")]
    public float separationRadius = 0.8f;
    [Tooltip("3-5 arası dene")]
    public float separationForce = 4f;
    public LayerMask enemyLayer;

    Rigidbody2D rb;
    Animator animator;
    PlayerHealth playerHealth;
    bool canAttack = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
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

        // Separation her durumda çalışsın — saldırı sırasında da
        Vector2 separation = GetSeparationVector();

        if (distance > attackRange)
        {
            Vector2 toPlayer = (target.position - transform.position).normalized;
            // Separation'ı direkt velocity olarak uygula — MovePosition değil
            Vector2 moveDir = toPlayer * moveSpeed + separation;
            rb.linearVelocity = moveDir;
        }
        else
        {
            // Menzilde — sadece separation uygula, player'a doğru gitme
            rb.linearVelocity = separation * 0.5f;
            if (canAttack)
                StartCoroutine(AttackRoutine());
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

            if (dist < 0.01f)
            {
                // Tam üst üste gelmiş — rastgele yönde it
                away = Random.insideUnitCircle.normalized;
                dist = 0.01f;
            }

            // Ne kadar yakınsa o kadar kuvvetli it
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