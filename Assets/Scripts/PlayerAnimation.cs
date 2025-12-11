using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Flip için gövde SpriteRenderer")]
    [SerializeField] private SpriteRenderer body; // boş bırakılırsa auto bulunur

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerController pc;

    // Animator parametreleri
    private static readonly int IsMovingParam = Animator.StringToHash("IsMoving");

    // Hız eşiği ve pozisyon takibi
    [SerializeField] private float moveEps = 0.0004f; // ~0.02 world unit/frame
    private Vector2 lastPos;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        pc = GetComponent<PlayerController>();
        if (!body) body = GetComponentInChildren<SpriteRenderer>();
        lastPos = rb ? rb.position : (Vector2)transform.position;
    }

    void LateUpdate()
    {
        // MovePosition kullansan bile pozisyon farkı kesin çalışır
        Vector2 now = rb ? rb.position : (Vector2)transform.position;
        Vector2 delta = now - lastPos;

        bool canMove = pc == null || pc.canMove;
        bool isMoving = canMove && (delta.sqrMagnitude > moveEps);
        if (animator) animator.SetBool(IsMovingParam, isMoving);

        // Flip – kök ölçeğe dokunmadan sadece sprite’ı çevir
        if (body)
        {
            if (delta.x > 0.001f)      body.flipX = false;
            else if (delta.x < -0.001f) body.flipX = true;
        }

        lastPos = now;
    }
}
