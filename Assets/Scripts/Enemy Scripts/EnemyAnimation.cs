using UnityEngine;

[DisallowMultipleComponent]
public class EnemyAnimation : MonoBehaviour
{
    [Header("Bağlantılar")]
    [SerializeField] SpriteRenderer body;   // boşsa otomatik bulunur
    [SerializeField] Animator anim;         // boşsa otomatik bulunur
    [SerializeField] Rigidbody2D rb;        // boşsa parent/root'ta aranır

    [Header("Animator Param Adı")]
    [SerializeField] string speedParam = "Speed"; // Animator'daki Float parametre adı

    [Header("Ayarlama")]
    [SerializeField] float enterWalk = 0.20f;
    [SerializeField] float exitWalk  = 0.10f;
    [SerializeField] float smoothHz  = 10f;
    [SerializeField] float animDamp  = 0.08f;

    int   speedHash;
    bool  hasSpeedParam;

    Vector2 lastPos;
    float   smoothSpeed;
    bool    moving;
    float   lastDeltaX;

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>(true);
        if (!body) body = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>(true);
        if (!rb)
        {
            var root = transform.root;
            rb = root ? root.GetComponent<Rigidbody2D>() : GetComponentInParent<Rigidbody2D>();
        }

        speedHash = Animator.StringToHash(speedParam);
        hasSpeedParam = CheckParam(anim, speedParam);
        if (!hasSpeedParam)
            Debug.LogError($"[EnemyAnimation] Animator içinde Float parametre '{speedParam}' yok. Animator → Parameters'tan ekleyin ve Blend Tree parametresini buna ayarlayın.");

        lastPos = rb ? rb.position : (Vector2)transform.position;
    }

    static bool CheckParam(Animator a, string name)
    {
        if (!a) return false;
        foreach (var p in a.parameters) if (p.name == name) return true;
        return false;
    }

    void FixedUpdate()
    {
        Vector2 now   = rb ? rb.position : (Vector2)transform.position;
        Vector2 delta = now - lastPos;

        float raw = delta.magnitude / Mathf.Max(Time.fixedDeltaTime, 0.0001f);
        float a = 1f - Mathf.Exp(-smoothHz * Time.fixedDeltaTime);
        smoothSpeed = Mathf.Lerp(smoothSpeed, raw, a);

        if (moving) { if (smoothSpeed < exitWalk) moving = false; }
        else        { if (smoothSpeed > enterWalk) moving = true; }

        if (body)
        {
            if (delta.x >  0.001f) body.flipX = false;
            if (delta.x < -0.001f) body.flipX = true;
        }

        lastDeltaX = delta.x;
        lastPos = now;
    }

    void Update()
    {
        if (anim && hasSpeedParam)
            anim.SetFloat(speedHash, moving ? smoothSpeed : 0f, animDamp, Time.deltaTime);
    }
}
