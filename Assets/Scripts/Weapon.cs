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

    [Header("Cast Efekti")]
    public GameObject castVfxPrefab; // Inspector'dan MageCastVFX prefabını sürükle

    [Header("Auto-Aim (Otomatik Hedefleme)")]
    public float targetRange = 10f;
    public LayerMask enemyLayer;

    [Header("Level Sistemi & Cooldown")]
    public float attackSpeedMultiplier = 1.0f;
    public float baseAttackCooldown = 0.8f;
    private float nextFireTime = 0f;

    Animator anim;
    PlayerController pc;
    bool busy;

    static readonly int AttackTrig = Animator.StringToHash("Attack");

    void Awake()
    {
        pc = GetComponentInParent<PlayerController>();
        anim = transform.root.GetComponentInChildren<Animator>();
    }

    public void FireAttack()
    {
        float currentCooldown = baseAttackCooldown / attackSpeedMultiplier;
        if (!busy && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + currentCooldown;
            StartCoroutine(AttackRoutine());
        }
    }

    public void IncreaseAttackSpeed(float amount)
    {
        attackSpeedMultiplier += amount;
        Debug.Log("Saldırı Hızı Arttı! Yeni Çarpan: " + attackSpeedMultiplier);
    }

    IEnumerator AttackRoutine()
    {
        busy = true;
        if (anim) anim.speed = attackSpeedMultiplier;

        anim.ResetTrigger(AttackTrig);
        anim.SetTrigger(AttackTrig);

        yield return null;

        bool shot = false;
        float failSafe = 2f;

        while (InAttack() && failSafe > 0f)
        {
            failSafe -= Time.deltaTime;

            if (anim.IsInTransition(0)) { yield return null; continue; }

            var st = anim.GetCurrentAnimatorStateInfo(0);

            if (!shot && st.normalizedTime >= fireAtNormalized)
            {
                SpawnMagicOrb();
                shot = true;
            }
            yield return null;
        }

        if (!shot)
        {
            Debug.LogWarning("Uyarı: Animasyon süreyi kaçırdı, mermi zorla ateşlendi!");
            SpawnMagicOrb();
        }

        if (anim) anim.speed = 1f;
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
        if (bulletPrefab == null) { Debug.LogError("KRİTİK HATA: Mermi Prefab'ı BOŞ!"); return; }
        if (firePoint == null)    { Debug.LogError("KRİTİK HATA: Fire Point BOŞ!"); return; }

        Vector2 shootDir = Vector2.right;
        Transform closestEnemy = FindClosestEnemy();

        if (closestEnemy != null)
        {
            shootDir = (closestEnemy.position - firePoint.position).normalized;

            if (shootDir.x < 0)
                transform.parent.localScale = new Vector3(-1, 1, 1);
            else
                transform.parent.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            shootDir = transform.parent.localScale.x < 0 ? Vector2.left : Vector2.right;
        }

        // Cast efekti — firePoint'te mavi parlama
        if (castVfxPrefab != null)
        {
            GameObject vfx = Instantiate(castVfxPrefab, firePoint.position, Quaternion.identity);
            Destroy(vfx, 0.4f);
        }

        var go = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        var rbBullet = go.GetComponent<Rigidbody2D>();
        if (rbBullet)
            rbBullet.linearVelocity = shootDir * bulletSpeed;
    }

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, targetRange);
    }
}