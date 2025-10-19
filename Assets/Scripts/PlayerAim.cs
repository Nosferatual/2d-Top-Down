using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    Camera cam;

    void Awake() => cam = Camera.main;

    void Update()
    {
        if (cam == null || Mouse.current == null) return;

        Vector3 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        // Sadece sağa-sola bakma
        if (mousePos.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }
}
