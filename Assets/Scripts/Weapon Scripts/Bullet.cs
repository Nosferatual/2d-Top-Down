using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 5f;
    public float damage = 10f;
    public Transform gfx;
    public float angleOffset = 0f;

    [Header("Çarpma Efekti")]
    public GameObject hitVfxPrefab;
    public bool spawnVfxOnEnemy = true;
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
            Vector2 pushDir = (enemy.transform.position - transform.position).normalized;
            enemy.TakeDamage((int)damage, pushDir);

            SpawnHitVfx(enemy.transform.position, pushDir);

            // Ses
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayFireballHit();

            Destroy(gameObject);
            return;
        }

        if (hitInfo.CompareTag("Wall"))
            Destroy(gameObject);
    }

    void SpawnHitVfx(Vector3 enemyPos, Vector2 bulletDir)
    {
        if (hitVfxPrefab == null) return;

        Vector3 spawnPos = spawnVfxOnEnemy ? enemyPos : transform.position;

        Quaternion rot = isMageProjectile
            ? Quaternion.identity
            : Quaternion.Euler(0f, 0f, Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg);

        GameObject vfx = Instantiate(hitVfxPrefab, spawnPos, rot);
        Destroy(vfx, 0.5f);
    }
}