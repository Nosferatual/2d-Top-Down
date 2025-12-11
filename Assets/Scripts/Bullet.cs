using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 3f;
    public float damage = 10f; // Hasar değeri

    public Transform gfx;      
    public float angleOffset = 0f;     

    Rigidbody2D rb;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    void Update() 
    {
        life -= Time.deltaTime;
        if (life <= 0f) Destroy(gameObject);
    }

    void LateUpdate()
    {
        if (rb && gfx)
        {
            // Unity sürümüne göre 'velocity' veya 'linearVelocity'
            Vector2 v = rb.linearVelocity; 
            if (v.sqrMagnitude > 0.001f)
            {
                float ang = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg + angleOffset;
                gfx.rotation = Quaternion.Euler(0, 0, ang);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    { 
        // Sadece duvara çarpınca yok ol. Düşmanı EnemyHit halledecek.
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}