using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Fire")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    [Tooltip("Okun çıkacağı an (Attack klibinde normalized time 0..1)")]
    public float fireAtNormalized = 0.35f;   // klibin ortası gibi

    Animator anim;
    PlayerController pc;
    bool busy;

    static readonly int AttackTrig = Animator.StringToHash("Attack");

    void Awake()
    {
        anim = GetComponentInParent<Animator>();
        pc   = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if (Mouse.current == null) return;
        if (Mouse.current.leftButton.wasPressedThisFrame && !busy)
            StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        busy = true;

        // Attack'ı tetikle
        anim.ResetTrigger(AttackTrig);
        anim.SetTrigger(AttackTrig);

        // Attack state'ine girene kadar bekle
        yield return new WaitUntil(() => InAttack());

        if (pc) pc.canMove = false;

        bool shot = false;
        // Attack state'inde kaldığın sürece döngü
        while (InAttack())
        {
            var st = anim.GetCurrentAnimatorStateInfo(0);

            // Belirlenen anda oku bir kez çıkar
            if (!shot && st.normalizedTime >= fireAtNormalized)
            {
                SpawnArrow();
                shot = true;
            }

            yield return null;
        }

        // Attack bitti → Locomotion'a döndü
        if (pc) pc.canMove = true;
        busy = false;
    }

    bool InAttack()
    {
        if (!anim) return false;
        // Tag = "Attack" olan state'te misin? (transition dahil)
        var cur  = anim.GetCurrentAnimatorStateInfo(0);
        if (anim.IsInTransition(0))
        {
            var next = anim.GetNextAnimatorStateInfo(0);
            return cur.IsTag("Attack") || next.IsTag("Attack");
        }
        return cur.IsTag("Attack");
    }

    void SpawnArrow()
    {
        if (!bulletPrefab || !firePoint) return;

        var go = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        var rb = go.GetComponent<Rigidbody2D>();
        if (rb)
        {
            // Projene göre linearVelocity yoksa rb.velocity kullan
            rb.linearVelocity = ((Vector2)firePoint.right).normalized * bulletSpeed;
        }
    }
}
