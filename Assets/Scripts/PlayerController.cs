using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;
    [HideInInspector] public bool canMove = true;

    Rigidbody2D rb;
    Camera cam;
    Animator animator;
    InputAction moveAction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        animator = GetComponent<Animator>();

        // WASD + Ok tuşları
        moveAction = new InputAction(type: InputActionType.Value, binding: "");
        var c = moveAction.AddCompositeBinding("2DVector");
        c.With("Up", "<Keyboard>/w");           c.With("Down", "<Keyboard>/s");
        c.With("Left", "<Keyboard>/a");         c.With("Right", "<Keyboard>/d");
        c.With("Up", "<Keyboard>/upArrow");     c.With("Down", "<Keyboard>/downArrow");
        c.With("Left", "<Keyboard>/leftArrow"); c.With("Right", "<Keyboard>/rightArrow");
    }

    void OnEnable()  => moveAction.Enable();
    void OnDisable() => moveAction.Disable();

    void Update()
    {
        if (!cam) cam = Camera.main; // sadece referans tazeleme
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            if (animator) animator.SetBool("isRunning", false);
            return;
        }

        Vector2 input = moveAction.ReadValue<Vector2>().normalized;
        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);

        if (animator) animator.SetBool("isRunning", input.sqrMagnitude > 0.01f);
    }
}
