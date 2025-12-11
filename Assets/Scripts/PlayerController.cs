using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;
    [HideInInspector] public bool canMove = true;

    Rigidbody2D rb;
    InputAction moveAction;
    Animator animator; // Walk bool'ünü PlayerAnimation set ediyor ama istersen burada da kullanabilirsin

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        moveAction = new InputAction(type: InputActionType.Value, binding: "");
        var c = moveAction.AddCompositeBinding("2DVector");
        c.With("Up", "<Keyboard>/w");           c.With("Down", "<Keyboard>/s");
        c.With("Left", "<Keyboard>/a");         c.With("Right", "<Keyboard>/d");
        c.With("Up", "<Keyboard>/upArrow");     c.With("Down", "<Keyboard>/downArrow");
        c.With("Left", "<Keyboard>/leftArrow"); c.With("Right", "<Keyboard>/rightArrow");
    }

    void OnEnable()  => moveAction.Enable();
    void OnDisable() => moveAction.Disable();

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero; // sürümün destekliyorsa linearVelocity
            return;
        }

        Vector2 input = moveAction.ReadValue<Vector2>().normalized;
        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);
    }
}
