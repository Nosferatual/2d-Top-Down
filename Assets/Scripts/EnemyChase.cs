using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    public Transform target;          // Player (Inspector’dan ya da tag ile)
    public float moveSpeed = 2.5f;
    public float stopDistance = 0.2f; // çok yaklaşınca dur

    Rigidbody2D rb;
    SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        if (!target)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
        }
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void FixedUpdate()
    {
        if (!target) return;
        Vector2 to = (target.position - transform.position);
        float d = to.magnitude;

        if (d > stopDistance)
        {
            Vector2 step = to.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + step);
        }

        // basit yüz çevirme (opsiyonel)
        if (sr) sr.flipX = to.x < 0f;
    }
}
