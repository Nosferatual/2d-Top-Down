using UnityEngine;

[DisallowMultipleComponent]
public class EnemyHit : MonoBehaviour
{
    public float xpMiktari = 10f;
    public GameObject deathVfx;

    bool dead;

    // Dışarıdan çağır: vurulduğu anda öldür
    public void Kill()
    {
        if (dead) return;
        dead = true;

        if (LevelManager.Instance)
            LevelManager.Instance.TecrubeKazan(xpMiktari);

        if (deathVfx)
        {
            var fx = Instantiate(deathVfx, transform.position, Quaternion.identity);
            Destroy(fx, 2f); // asset değil instance yok ediliyor
        }

        Destroy(gameObject); // sahnedeki düşman instance'ı
    }
}
