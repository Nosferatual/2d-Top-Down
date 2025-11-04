using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 1.2f;

    // 🔽 EK: sprite'ı ayrı child olarak bağla
    public Transform gfx;              
    // 🔽 EK: sprite ileri eksenine göre ayar (ok sağa bakıyorsa 0, yukarıysa +90, sola bakıyorsa 180)
    public float angleOffset = 0f;     

    Rigidbody2D rb;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    void Update() {
        life -= Time.deltaTime;
        if (life <= 0f) Destroy(gameObject);
    }

    void LateUpdate()
    {
        if (rb && gfx)
        {
            Vector2 v = rb.linearVelocity;
            if (v.sqrMagnitude > 0.0001f)
            {
                float ang = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg + angleOffset;
                gfx.rotation = Quaternion.Euler(0, 0, ang);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) { Destroy(gameObject); 
    
        if (other.CompareTag("Enemy")) // düşmana değdi
    {
        Destroy(other.gameObject); // düşman
        Destroy(gameObject);       // mermi
    }
    }
    // Eğer trigger değilse alttakini kullan:
    // void OnCollisionEnter2D(Collision2D other) { Destroy(gameObject); }
}
