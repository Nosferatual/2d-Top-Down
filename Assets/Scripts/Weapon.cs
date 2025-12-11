using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    [Header("Timing")]
    [Tooltip("Attack klibinin toplam süresi (sn).")]
    public float attackDuration = 0.40f;
    [Tooltip("Okun çıkacağı an (sn).")]
    public float shootDelay    = 0.20f;

    [Header("Animator")]
    [Tooltip("Idle state adı (Animator'daki state adı birebir).")]
    public string idleStateName = "idle";

    private Animator animator;
    private PlayerController playerController;
    private bool isShootingNow;
    private float cooldown;

    private static readonly int AttackTrig = Animator.StringToHash("Attack");

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame && cooldown <= 0f && !isShootingNow)
            StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        if (isShootingNow || !bulletPrefab || !firePoint) yield break;

        isShootingNow = true;
        if (playerController) playerController.canMove = false;

        // 1) Attack animasyonu
        if (animator) animator.SetTrigger(AttackTrig);

        // 2) Ok çıkışı
        if (shootDelay > 0f) yield return new WaitForSeconds(shootDelay);
        SpawnArrow();

        // 3) Klibin kalanı kadar bekle
        float remain = Mathf.Max(0f, attackDuration - shootDelay);
        if (remain > 0f) yield return new WaitForSeconds(remain);

        // 4) ZORLA Idle'a dön ve kilidi aç (Animator ayarı şaşsa bile)
       /* if (animator)
        {
            animator.ResetTrigger(AttackTrig);
            if (!string.IsNullOrEmpty(idleStateName))
                animator.CrossFade(idleStateName, 0f, 0, 0f);
        }*/

        if (playerController) playerController.canMove = true;
        isShootingNow = false;
        cooldown = attackDuration;
    }

    void SpawnArrow()
    {
        var go = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        var rb2d = go.GetComponent<Rigidbody2D>();
        if (!rb2d) return;

        Vector2 dir = (Vector2)firePoint.right;
        rb2d.linearVelocity = dir.normalized * bulletSpeed; // sürümün desteklemiyorsa rb2d.velocity kullan
    }
}
