using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    Transform target;              // Inspector'a açmıyoruz → mismatch biter
    public float speed = 2.5f;
    public float stopDistance = 0.2f;

    Rigidbody2D rb;
    SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        sr = GetComponentInChildren<SpriteRenderer>();

        // Player'ı otomatik bul
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) target = p.transform;
        else Debug.LogError("[EnemyChase] 'Player' tag'li obje bulunamadı.");
    }

    void FixedUpdate()
    {
        if (!target) return;

        Vector2 pos = rb.position;
        Vector2 to = (Vector2)target.position - pos;
        float d = to.magnitude;

        if (d <= stopDistance) { rb.linearVelocity = Vector2.zero; return; }

        Vector2 dir = to / d;
        rb.MovePosition(pos + dir * speed * Time.fixedDeltaTime);

        if (sr)
        {
            if (dir.x > 0.01f) sr.flipX = false;
            else if (dir.x < -0.01f) sr.flipX = true;
        }
    }
}
