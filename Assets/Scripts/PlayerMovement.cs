using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;

    public float speedThreshold = 0.01f;
    public bool isRunning;
    private Vector2 lastPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        lastPos = rb.position;
    }

    void Update()
    {
        // Gerçek hızı pozisyon farkından hesapla
        float speed = ((rb.position - lastPos).sqrMagnitude) / Time.deltaTime;
        lastPos = rb.position;

        isRunning = speed > speedThreshold;

        if (animator != null)
            animator.SetBool("isRunning", isRunning);
    }
}
