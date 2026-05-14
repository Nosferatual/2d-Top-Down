using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 3f;
    public float damage = 10f;
    public Transform gfx;
    public float angleOffset = 0f;

    [Header("Çarpma Efekti")]
    [Tooltip("Okçu için: ok yönünde ilerleyen toz/duman partikülleri")]
    //[Tooltip("Büyücü için: ateş patlama partikülleri")]
    public GameObject hitVfxPrefab;

    [Tooltip("Efekt düşmanın üzerinde mi (true) yoksa çarpma noktasında mı (false) doğsun")]
    public bool spawnVfxOnEnemy = true;

    [Tooltip("Büyücü ise true — efekt yön almaz, her yana patlar")]
    public bool isMageProjectile = false;

    Rigidbody2D rb;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0f) Destroy(gameObject);
    }

    void LateUpdate()
    {
        if (!rb || !gfx) return;
        var v = rb.linearVelocity;
        if (v.sqrMagnitude > 0.001f)
        {
            float ang = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg + angleOffset;
            gfx.rotation = Quaternion.Euler(0, 0, ang);
        }
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        EnemyHit enemy = hitInfo.GetComponent<EnemyHit>();
        if (enemy != null)
        {
            Vector2 pushDirection = (enemy.transform.position - transform.position).normalized;
            enemy.TakeDamage((int)damage, pushDirection);

            SpawnHitVfx(enemy.transform.position, pushDirection);

            Destroy(gameObject);
            return;
        }

        if (hitInfo.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    void SpawnHitVfx(Vector3 enemyPos, Vector2 bulletDir)
    {
        if (hitVfxPrefab == null) return;

        Vector3 spawnPos = spawnVfxOnEnemy ? enemyPos : transform.position;

        Quaternion rot;
        if (isMageProjectile)
        {
            // Büyücü: her yana patlasın — rotasyon fark etmez
            rot = Quaternion.identity;
        }
        else
        {
            // Okçu: partiküller merminin geldiği yönde gitsin (momentum yönü)
            float angle = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg;
            rot = Quaternion.Euler(0f, 0f, angle);
        }

        GameObject vfx = Instantiate(hitVfxPrefab, spawnPos, rot);
        Destroy(vfx, 0.5f); // 0.5 saniye sonra yok ol — sahne çöp dolmasın
    }
}