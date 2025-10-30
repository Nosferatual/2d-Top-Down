// SmoothCameraMovement.cs (güncel)
using UnityEngine;

public class SmoothCameraMovement : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = Vector3.zero;
    [SerializeField] float smoothTime = 0.12f;
    [SerializeField] bool pixelSnap = true;
    [SerializeField] float pixelsPerUnit = 32f;
    [SerializeField] float snapWhenSpeedBelow = 0.05f; // durmaya yakınken snap

    Vector3 velocity;
    Rigidbody2D targetRb;

    void Awake()
    {
        if (target) targetRb = target.GetComponentInParent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = target.position + offset;
        desired.z = transform.position.z;

        Vector3 smoothed = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);

        // Sadece oyuncu yavaşken/ dururken snap yap
        bool shouldSnap = pixelSnap && pixelsPerUnit > 0f &&
                          (!targetRb || targetRb.linearVelocity.sqrMagnitude < snapWhenSpeedBelow * snapWhenSpeedBelow);

        if (shouldSnap)
        {
            smoothed.x = Mathf.Round(smoothed.x * pixelsPerUnit) / pixelsPerUnit;
            smoothed.y = Mathf.Round(smoothed.y * pixelsPerUnit) / pixelsPerUnit;
        }

        transform.position = smoothed;
    }
}
