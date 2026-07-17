using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("SPUM Karakteri")]
    [Tooltip("Hiyerarşideki Unit1 (veya SPUM) objesini buraya sürükle")]
    public Transform visualBody;

    [Header("Yön Ayarı")]
    public bool isSpumFacingLeft = true;

    Animator    anim;
    Rigidbody2D rb;

    Vector2 lastPos;
    bool    moving;
    float   lastDeltaX;
    Vector3 originalScale;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb   = GetComponent<Rigidbody2D>();

        if (visualBody != null) originalScale = visualBody.localScale;
        else originalScale = Vector3.one;

        lastPos = rb ? rb.position : (Vector2)transform.position;
    }

    void FixedUpdate()
    {
        Vector2 now   = rb ? rb.position : (Vector2)transform.position;
        Vector2 delta = now - lastPos;

        float rawSpeed = delta.magnitude / Mathf.Max(Time.fixedDeltaTime, 0.0001f);
        moving = rawSpeed > 0.1f;

        if (Mathf.Abs(delta.x) > 0.001f)
            lastDeltaX = delta.x;

        lastPos = now;
    }

    void Update()
    {
        if (anim != null)
        {
            anim.SetBool("Run", moving);
            anim.SetFloat("RunState", moving ? 1f : 0f);
        }

        if (InAttack()) return;

        if (visualBody != null)
        {
            float absX = Mathf.Abs(originalScale.x);

            if (lastDeltaX > 0.001f)
                visualBody.localScale = new Vector3(isSpumFacingLeft ? -absX : absX, originalScale.y, originalScale.z);
            else if (lastDeltaX < -0.001f)
                visualBody.localScale = new Vector3(isSpumFacingLeft ? absX : -absX, originalScale.y, originalScale.z);
        }
    }

    // PlayerHealth tarafından çağrılır
    public void TriggerDeath()
    {
        if (anim != null)
        {
            anim.SetTrigger("Die");
            anim.SetBool("Run", false);
            anim.SetFloat("RunState", 0f);
        }
    }

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