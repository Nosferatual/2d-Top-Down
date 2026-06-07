using UnityEngine;

[DisallowMultipleComponent]
public class EnemyAnimation : MonoBehaviour
{
    [Header("Bağlantılar")]
    [SerializeField] SpriteRenderer body;
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody2D rb;

    [Header("Animator Param Adı")]
    [SerializeField] string speedParam = "Speed";

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

    Transform player; // Flip için player yönü

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
            Debug.LogError($"[EnemyAnimation] Animator içinde Float parametre '{speedParam}' yok.");

        lastPos = rb ? rb.position : (Vector2)transform.position;
    }

    void Start()
    {
        // Player'ı bul — flip için
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
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
        float alpha = 1f - Mathf.Exp(-smoothHz * Time.fixedDeltaTime);
        smoothSpeed = Mathf.Lerp(smoothSpeed, raw, alpha);

        if (moving) { if (smoothSpeed < exitWalk) moving = false; }
        else        { if (smoothSpeed > enterWalk) moving = true; }

        // Flip: SADECE player'ın sağda/solda olmasına göre
        // Separation veya başka bir harekete bakma
        if (body && player)
        {
            float dirX = player.position.x - transform.position.x;
            if (Mathf.Abs(dirX) > 0.05f) // Çok yakınsa flip yapma — titreme önleme
            {
                body.flipX = dirX < 0f;
                // Eğer sprite varsayılan sola bakıyorsa üstteki satırı şununla değiştir:
                // body.flipX = dirX > 0f;
            }
        }

        lastPos = now;
    }

    void Update()
    {
        if (anim && hasSpeedParam)
            anim.SetFloat(speedHash, moving ? smoothSpeed : 0f, animDamp, Time.deltaTime);
    }
}