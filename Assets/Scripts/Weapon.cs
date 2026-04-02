using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Fire Ayarları")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    [Tooltip("Büyünün çıkacağı an (Attack klibinde normalized time 0..1)")]
    public float fireAtNormalized = 0.01f;

    [Header("Auto-Aim (Otomatik Hedefleme)")]
    public float targetRange = 10f;   // Düşman arama menzili
    public LayerMask enemyLayer;      // Düşmanların olduğu katman

    [Header("Level Sistemi & Cooldown")]
    public float attackSpeedMultiplier = 1.0f; // Level atladıkça bu artacak
    public float baseAttackCooldown = 0.8f;    // Başlangıçta kaç saniyede bir vurabilsin?
    private float nextFireTime = 0f;           // Bir sonraki atış zamanını tutar

    Animator anim;
    PlayerController pc;
    bool busy;

    static readonly int AttackTrig = Animator.StringToHash("Attack");

    void Awake()
    {
        // PlayerController ana objede (PlayerSUMP)
        pc = GetComponentInParent<PlayerController>();

        // Animator artık SPUM objesinin içinde
        anim = transform.root.GetComponentInChildren<Animator>();
    }

    // --- MOBİLDE EKRANDAKİ BUTONUN ÇAĞIRACAĞI ANA FONKSİYON ---
    public void FireAttack()
    {
        // Level atladıkça bekleme süresini (cooldown) kısalt
        float currentCooldown = baseAttackCooldown / attackSpeedMultiplier;

        // Eğer karakter meşgul değilse VE bekleme süresi dolmuşsa ateş et
        if (!busy && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + currentCooldown;
            StartCoroutine(AttackRoutine());
        }
    }

    // --- LEVEL MANAGER'IN ÇAĞIRACAĞI FONKSİYON ---
    public void IncreaseAttackSpeed(float amount)
    {
        attackSpeedMultiplier += amount;
        Debug.Log("Saldırı Hızı Arttı! Yeni Çarpan: " + attackSpeedMultiplier);
    }

    IEnumerator AttackRoutine()
    {
        busy = true;
        if(anim) anim.speed = attackSpeedMultiplier;

        anim.ResetTrigger(AttackTrig);
        anim.SetTrigger(AttackTrig);

        yield return null; // Başlaması için 1 frame bekle

        bool shot = false;
        float failSafe = 2f; 
        
        while (InAttack() && failSafe > 0f)
        {
            failSafe -= Time.deltaTime;
            
            // GEÇİŞ (Transition) anındaysa eski animasyonun zamanını almasın diye bekle
            if (anim.IsInTransition(0)) { yield return null; continue; }

            var st = anim.GetCurrentAnimatorStateInfo(0);

            // Zamanı gelince mermiyi ateşle
            if (!shot && st.normalizedTime >= fireAtNormalized)
            {
                SpawnMagicOrb();
                shot = true;
            }
            yield return null;
        }

        // --- SİGORTA KODU ---
        // Eğer SPUM animasyonu çok hızlı bittiyse ve süre yetmediği için mermi atamadıysa, ZORLA AT!
        if (!shot)
        {
            Debug.LogWarning("Uyarı: Animasyon süreyi kaçırdı, mermi zorla ateşlendi!");
            SpawnMagicOrb();
        }

        // Sistemi sıfırla
        if(anim) anim.speed = 1f;
        if (pc) pc.canMove = true;
        busy = false;
    }

    bool InAttack()
    {
        if (!anim) return false;
        
        var cur = anim.GetCurrentAnimatorStateInfo(0);
        if (anim.IsInTransition(0))
        {
            var next = anim.GetNextAnimatorStateInfo(0);
            return cur.IsTag("Attack") || next.IsTag("Attack");
        }
        return cur.IsTag("Attack");
    }

    void SpawnMagicOrb()
    {
        // KONTROL: Inspector'da kutular boş mu kalmış?
        if (bulletPrefab == null) { Debug.LogError("KRİTİK HATA: Mermi Prefab'ı BOŞ!"); return; }
        if (firePoint == null) { Debug.LogError("KRİTİK HATA: Fire Point BOŞ!"); return; }

        Vector2 shootDir = Vector2.right; 
        Transform closestEnemy = FindClosestEnemy();

        if (closestEnemy != null)
        {
            // Düşman varsa mermiyi ona doğru gönder
            shootDir = (closestEnemy.position - firePoint.position).normalized;
            
            // Karakteri zorla düşmana döndür
            if (shootDir.x < 0)
                transform.parent.localScale = new Vector3(-1, 1, 1); 
            else
                transform.parent.localScale = new Vector3(1, 1, 1);  
        }
        else
        {
            // Düşman yoksa karakterin baktığı yöne at
            shootDir = transform.parent.localScale.x < 0 ? Vector2.left : Vector2.right;
        }

        var go = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        var rbBullet = go.GetComponent<Rigidbody2D>();
        
        if (rbBullet)
        {
            rbBullet.linearVelocity = shootDir * bulletSpeed; 
        }
    }

    // Matematiksel Arama Algoritması
    Transform FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, targetRange, enemyLayer);
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy.transform;
            }
        }
        return closest;
    }

    // Editörde menzilini mor bir çemberle gösterir
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, targetRange);
    }
}