using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    public Transform target;          
    public float moveSpeed = 2.5f;
    
    [Header("Saldırı Ayarları")]
    public float attackRange = 1.5f;   // Saldırı mesafesi
    public float attackCooldown = 1.5f; // Kaç saniyede bir vursun
    public int damageAmount = 10;
    
    [Header("Geri Tepme Ayarları")]
    public float knockbackForce = 5f;
    public float stunDuration = 0.2f;
    
    [Header("Saldırı Zamanlaması")]
    public float damageDelay = 0.4f; // Animasyonun tam vurduğu anı (saniye) buraya yazacağız

    Rigidbody2D rb;
    Animator animator; 
    bool isKnockedBack = false;
    bool canAttack = true; // Saldırabilir mi?

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
            if (p) target = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (!target || isKnockedBack) return;

        // Mesafeyi ölç
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            // Uzaksak kovala
            Vector2 direction = (target.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Yakınsak ve bekleme süresi bittiyse SALDIR
            if (canAttack)
            {
                StartCoroutine(AttackRoutine());
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        
        // 1. Düşmanı durdur (Saldırırken kaymasın)
        rb.linearVelocity = Vector2.zero; 

        // 2. Animasyonu başlat
        if (animator) animator.SetTrigger("Attack");

        // 3. KRİTİK NOKTA: Bekle! (Animasyonun vurma karesine gelmesini bekle)
        yield return new WaitForSeconds(damageDelay);

        // 4. Hala menzilde miyiz? (Oyuncu kaçmış olabilir)
        float distanceInfo = Vector2.Distance(transform.position, target.position);
        if (distanceInfo <= attackRange + 0.5f) // Biraz tolerans tanıdık (+0.5f)
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }

        // 5. Saldırı bekleme süresinin geri kalanı
        // (Toplam cooldown - harcanan delay kadar bekle)
        yield return new WaitForSeconds(attackCooldown - damageDelay);
        
        canAttack = true;
    }

    // --- Burası senin eski Knockback kodun, aynen kalıyor ---
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
             // Çarpınca da ufak itişme olsun istersen burası kalabilir
             // Ama saldırı animasyonu eklediğimiz için genelde çarpışma hasarını kapatırız.
             // Şimdilik sadece Knockback kalsın, hasarı yukarıdaki AttackRoutine veriyor.
             
             Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
             StartCoroutine(KnockbackRoutine(knockbackDir));
        }
    }

    IEnumerator KnockbackRoutine(Vector2 direction)
    {
        isKnockedBack = true;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(stunDuration);
        rb.linearVelocity = Vector2.zero; 
        isKnockedBack = false;
    }
}