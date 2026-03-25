using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;
    [HideInInspector] public bool canMove = true;

    Rigidbody2D rb;
    InputAction moveAction;
    Animator animator; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Aksiyonu oluştur
        moveAction = new InputAction(type: InputActionType.Value, expectedControlType: "Vector2");
        
        // Klavye WASD Bağlantısı
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // Ok Tuşları Bağlantısı
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        // --- MOBİL İÇİN YENİ EKLENEN KISIM (Sanal Joystick) ---
        moveAction.AddBinding("<Gamepad>/leftStick");
    }

    void OnEnable()  => moveAction.Enable();
    void OnDisable() => moveAction.Disable();

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero; // Unity sürümün eskiyse 'velocity' yapabilirsin
            return;
        }

        // Girdiyi oku
        Vector2 input = moveAction.ReadValue<Vector2>();
        
        // Çapraz gidişlerde hızlanmayı önlemek için normalize et
        if (input.sqrMagnitude > 1f) input.Normalize();

        // Karakteri hareket ettir
        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);
    }
}