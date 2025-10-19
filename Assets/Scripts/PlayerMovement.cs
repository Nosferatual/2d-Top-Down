using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D> ();
    }
    public float speedThreshold = 0.01f;
    public bool isRunning;

    void Update()
    {

        float speed = rb != null ? rb.linearVelocity.sqrMagnitude : 0.0f;

        isRunning = speed > speedThreshold * speedThreshold;

    }    
}
