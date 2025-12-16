using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 3f;
    public float damage = 10f;
    public Transform gfx;
    public float angleOffset = 0f;

    Rigidbody2D rb;
    bool hit; // aynı karede çoklu collider tetiklerine karşı

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0f) Destroy(gameObject);
    }

    void LateUpdate()
    {
        if (!rb || !gfx) return;
        var v = rb.linearVelocity; // sürümün desteklemiyorsa rb.velocity
        if (v.sqrMagnitude > 0.001f)
        {
            float ang = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg + angleOffset;
            gfx.rotation = Quaternion.Euler(0, 0, ang);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
{
    if (hit) return; // varsa mevcut guard

    var enemy = other.GetComponentInParent<EnemyHit>();
    if (enemy)
    {
        hit = true;               // varsa guard değişkenini set et
        enemy.Kill();             // anında öldür
        Destroy(gameObject);      // mermiyi sil
        return;
    }

    if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") || other.CompareTag("Wall"))
    {
        hit = true;
        Destroy(gameObject);
    }
}

}
