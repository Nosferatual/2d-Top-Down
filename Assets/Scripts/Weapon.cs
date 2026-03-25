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
    public float fireAtNormalized = 0.35f;

    [Header("Auto-Aim (Otomatik Hedefleme)")]
    public float targetRange = 10f;   // Düşman arama menzili
    public LayerMask enemyLayer;      // Düşmanların olduğu katman

    [Header("Level Sistemi")]
    public float attackSpeedMultiplier = 1.0f; // Level atladıkça bu artacak

    Animator anim;
    PlayerController pc;
    bool busy;

    static readonly int AttackTrig = Animator.StringToHash("Attack");

    void Awake()
    {
        anim = GetComponentInParent<Animator>();
        pc = GetComponentInParent<PlayerController>();
    }

    /*void Update()
    {
        // PC'de denerken Sol Tıkla da ateş edebilmen için (istersen silebilirsin)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            FireAttack();
        }
    }*/

    // --- MOBİLDE EKRANDAKİ BUTONUN ÇAĞIRACAĞI ANA FONKSİYON ---
    public void FireAttack()
    {
        if (!busy)
            StartCoroutine(AttackRoutine());
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

        // 1. ANİMASYON HIZINI ARTIR (Level'a göre)
        if(anim) anim.speed = attackSpeedMultiplier;

        // Attack'ı tetikle
        anim.ResetTrigger(AttackTrig);
        anim.SetTrigger(AttackTrig);

        // Attack state'ine girene kadar bekle
        yield return new WaitUntil(() => InAttack());

        if (pc) pc.canMove = false;

        bool shot = false;
        
        // Attack state'inde kaldığın sürece döngü
        while (InAttack())
        {
            var st = anim.GetCurrentAnimatorStateInfo(0);

            // Belirlenen anda büyüyü bir kez fırlat
            if (!shot && st.normalizedTime >= fireAtNormalized)
            {
                SpawnMagicOrb();
                shot = true;
            }

            yield return null;
        }

        // 2. ANİMASYON HIZINI NORMALE DÖNDÜR
        if(anim) anim.speed = 1f;

        // Attack bitti -> Yürümeye devam
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

    // Oku/Büyüyü Yaratan Yeni Fonksiyon
    void SpawnMagicOrb()
    {
        if (!bulletPrefab || !firePoint) return;

        Vector2 shootDir = Vector2.right; 
        Transform closestEnemy = FindClosestEnemy();

        if (closestEnemy != null)
        {
            // Menzilde düşman varsa ona doğru vektörü al
            shootDir = (closestEnemy.position - firePoint.position).normalized;
            
            // KARAKTERİ ZORLA DÜŞMANA DÖNDÜR (Alt obje aramadan)
            if (shootDir.x < 0)
                transform.parent.localScale = new Vector3(-1, 1, 1); // Sola bak
            else
                transform.parent.localScale = new Vector3(1, 1, 1);  // Sağa bak
        }
        else
        {
            // Menzilde düşman yoksa karakterin şu an baktığı yöne ateş et
            shootDir = transform.parent.localScale.x < 0 ? Vector2.left : Vector2.right;
        }

        var go = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        var rbBullet = go.GetComponent<Rigidbody2D>();
        
        if (rbBullet)
        {
            // linearVelocity sürümüne uymuyorsa velocity yap
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