using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public bool aimUsesUpAxis = true;   // sprite ileri yönün "up" ise true, "right" ise false

    Rigidbody2D rb;
    Camera cam;
    InputAction moveAction;

    void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        moveAction = new InputAction(type: InputActionType.Value, binding: "");
        var c = moveAction.AddCompositeBinding("2DVector");
        c.With("Up", "<Keyboard>/w");    c.With("Down", "<Keyboard>/s");
        c.With("Left","<Keyboard>/a");   c.With("Right","<Keyboard>/d");
        c.With("Up", "<Keyboard>/upArrow");  c.With("Down","<Keyboard>/downArrow");
        c.With("Left","<Keyboard>/leftArrow"); c.With("Right","<Keyboard>/rightArrow");
    }

    void OnEnable()  => moveAction.Enable();
    void OnDisable() => moveAction.Disable();

    void Update()
    {

   
        if (cam == null) cam = Camera.main;
        if (cam == null || Mouse.current == null) return;

        var mp = Mouse.current.position.ReadValue();
        float depth = cam.WorldToScreenPoint(transform.position).z; // kaymayı önler
        Vector3 mWorld = cam.ScreenToWorldPoint(new Vector3(mp.x, mp.y, depth));
        Vector2 aimDir = (mWorld - transform.position);

        if (aimDir.sqrMagnitude > 0.0001f)
        {
            if (aimUsesUpAxis) transform.up = aimDir.normalized;
            else transform.right = aimDir.normalized;
        }
        

    }

    void FixedUpdate()
    {
        Vector2 move = moveAction.ReadValue<Vector2>().normalized * moveSpeed;
        rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
    }
}
