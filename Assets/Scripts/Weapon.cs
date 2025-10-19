using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireCooldown = 0.12f;
    public bool useUpAxis = true;

    Camera cam;
    float cd;

    void Awake() => cam = Camera.main;

    void Update()
    {
        cd -= Time.deltaTime;

        if (cam == null) cam = Camera.main;
        if (cam == null || Mouse.current == null) return;

        // 🔹 Silahın mouse yönüne bakması
        Vector3 mp = Mouse.current.position.ReadValue();
        mp.z = -cam.transform.position.z;
        Vector3 mWorld = cam.ScreenToWorldPoint(mp);
/*
        Vector2 dir = (mWorld - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
*/
        // 🔹 Ateş kontrolü
        if (Mouse.current.leftButton.isPressed && cd <= 0f)
            Fire();
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            Fire();
    }

    void Fire()
    {
        if (!bulletPrefab) { Debug.LogError("[Weapon] bulletPrefab atanmadı."); return; }
        if (!firePoint)    { Debug.LogError("[Weapon] firePoint atanmadı.");   return; }

        var go = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        var rb = go.GetComponent<Rigidbody2D>();

        if (!rb)
        {
            Debug.LogError("[Weapon] Bullet prefabında Rigidbody2D yok.");
            Destroy(go);
            return;
        }

        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        Vector2 dir = useUpAxis ? (Vector2)firePoint.up : (Vector2)firePoint.right;
        rb.linearVelocity = dir * bulletSpeed;

        cd = fireCooldown;
    }
}
