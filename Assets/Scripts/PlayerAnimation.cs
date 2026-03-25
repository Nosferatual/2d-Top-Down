using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] SpriteRenderer body;      // boşsa otomatik bulunur
    [SerializeField] float enterWalk = 0.20f;  // yürüme için giriş eşiği
    [SerializeField] float exitWalk  = 0.10f;  // yürümeden çıkış eşiği
    [SerializeField] float animDamp  = 0.08f;  // Animator.SetFloat damping (s)
    [SerializeField] float smoothHz  = 10f;    // hız yumuşatma katsayısı

    static readonly int SpeedHash = Animator.StringToHash("Speed");

    Animator    anim;
    Rigidbody2D rb;

    Vector2 lastPos;
    float   smoothSpeed;   // yumuşatılmış hız
    bool    moving;
    float   lastDeltaX;    // flip için son frame yatay hareketi

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb   = GetComponent<Rigidbody2D>();
        if (!body) body = GetComponentInChildren<SpriteRenderer>();
        lastPos = rb ? rb.position : (Vector2)transform.position;
    }

    void FixedUpdate()
    {
        Vector2 now   = rb ? rb.position : (Vector2)transform.position;
        Vector2 delta = now - lastPos;

        float rawSpeed = delta.magnitude / Mathf.Max(Time.fixedDeltaTime, 0.0001f);
        float a = 1f - Mathf.Exp(-smoothHz * Time.fixedDeltaTime);
        smoothSpeed = Mathf.Lerp(smoothSpeed, rawSpeed, a);

        if (moving)
        {
            if (smoothSpeed < exitWalk) moving = false;
        }
        else
        {
            if (smoothSpeed > enterWalk) moving = true;
        }

        lastDeltaX = delta.x;
        lastPos    = now;
    }

    void Update()
    {
        anim.SetFloat(SpeedHash, moving ? smoothSpeed : 0f, animDamp, Time.deltaTime);

        // --- YENİ EKLENEN KISIM: EĞER SALDIRIYORSAK (ATTACK) DÖNMEYE KARIŞMA ---
        if (InAttack()) return;

        // Flip (Sadece yürürken çalışır)
        if (body)
        {
            if (lastDeltaX >  0.0005f) body.flipX = false;
            if (lastDeltaX < -0.0005f) body.flipX = true;
        }
    }

    // Saldırı (Attack) durumunda olup olmadığımızı kontrol eder
    bool InAttack()
    {
        if (!anim) return false;
        
        var cur = anim.GetCurrentAnimatorStateInfo(0);
        if (anim.IsInTransition(0))
        {
            var next = anim.GetNextAnimatorStateInfo(0);
            return cur.IsTag("Attack") || next.IsTag("Attack");
        }
        return cur.IsTag("Attack");
    }
}