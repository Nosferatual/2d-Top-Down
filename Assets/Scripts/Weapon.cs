using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    [Header("Timing")]
    [Tooltip("Animasyon süresi kadar olmalı.")]
    public float fireCooldown = 0.4f;
    [Tooltip("Okun çıkacağı an (animasyon ortası).")]
    public float shootDelay = 0.2f;

    Animator animator;
    PlayerController playerController;
    float cooldown;
    [HideInInspector] public bool isShootingNow = false;

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
        if (Mouse.current == null) return;

        // tek tıklamada ateş; spam'ı cooldown + isShootingNow engeller
        if (Mouse.current.leftButton.wasPressedThisFrame && cooldown <= 0f && !isShootingNow)
            Fire();
    }

    void Fire()
    {
        if (isShootingNow || !bulletPrefab || !firePoint) return;

        isShootingNow = true;
        if (playerController) playerController.canMove = false; // yürüyüşü kilitle
        if (animator) animator.SetBool("isShooting", true);

        Invoke(nameof(SpawnArrow), shootDelay);
        Invoke(nameof(ResetShootAnim), fireCooldown);
        cooldown = fireCooldown;
    }

    void SpawnArrow()
    {
        var go = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        var rb = go.GetComponent<Rigidbody2D>();
        if (!rb) return;

        // pivot yönü (right) nereye bakıyorsa ok oraya gider
        Vector2 dir = (Vector2)firePoint.right;
        rb.linearVelocity = dir.normalized * bulletSpeed;
    }

    public void ResetShootAnim()
    {
        if (animator) animator.SetBool("isShooting", false);
        isShootingNow = false;
        if (playerController) playerController.canMove = true; // yürüyüşü aç
    }
}
