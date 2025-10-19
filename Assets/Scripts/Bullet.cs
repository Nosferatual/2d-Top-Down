using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 1.2f;

    void Update() {
        life -= Time.deltaTime;
        if (life <= 0f) Destroy(gameObject);
    }

    // Trigger kullanıyorsan:
    void OnTriggerEnter2D(Collider2D other) { Destroy(gameObject); }

    // Trigger kullanmıyorsan üsttekini sil, bunu aç:
    // void OnCollisionEnter2D(Collision2D other) { Destroy(gameObject); }
}
