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
        // MovePosition kullanıyorsan hız ölçümü buradan yapılmalı
        Vector2 now   = rb ? rb.position : (Vector2)transform.position;
        Vector2 delta = now - lastPos;

        float rawSpeed = delta.magnitude / Mathf.Max(Time.fixedDeltaTime, 0.0001f);
        // Üstel alçak geçiren filtre (low-pass)
        float a = 1f - Mathf.Exp(-smoothHz * Time.fixedDeltaTime);
        smoothSpeed = Mathf.Lerp(smoothSpeed, rawSpeed, a);

        // Histerezis: çift eşik ile kararsız bölgeyi yok et
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
        // Animator parametresini damping ile besle (blend tree’yi pürüzsüz sürer)
        anim.SetFloat(SpeedHash, moving ? smoothSpeed : 0f, animDamp, Time.deltaTime);

        // Flip (root scale'e dokunma, sadece sprite)
        if (body)
        {
            if (lastDeltaX >  0.0005f) body.flipX = false;
            if (lastDeltaX < -0.0005f) body.flipX = true;
        }
    }
}
