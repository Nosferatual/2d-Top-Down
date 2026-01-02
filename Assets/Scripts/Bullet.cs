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

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Eğer vurduğumuz şey Düşman ise
        EnemyHit enemy = hitInfo.GetComponent<EnemyHit>();
        if (enemy != null)
        {
            // Merminin gidiş yönünü hesapla (Geri tepme için lazım)
            Vector2 pushDirection = (enemy.transform.position - transform.position).normalized;
            
            // Düşmana hasar ver (Örn: 10 hasar) ve itme yönünü gönder
            enemy.TakeDamage(10, pushDirection); 
            
            // Mermiyi yok et
            Destroy(gameObject);
        }
        
        // Duvara çarparsa da yok olsun (Tag kontrolü yapabilirsin)
        if (hitInfo.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

}
