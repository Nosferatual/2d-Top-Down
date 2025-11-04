using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    public GameObject deathVfx; // opsiyonel

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (deathVfx) Instantiate(deathVfx, transform.position, Quaternion.identity);
            Destroy(other.gameObject);   // mermiyi sil
            Destroy(gameObject);         // düşmanı sil
        }
    }

    // Eğer merminiz Trigger değilse bunu kullanın:
    // void OnCollisionEnter2D(Collision2D col) { if (col.collider.CompareTag("Bullet")) { Destroy(col.collider.gameObject); Destroy(gameObject); } }
}
