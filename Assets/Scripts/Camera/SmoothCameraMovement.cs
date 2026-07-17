using UnityEngine;

public class SmoothCameraMovement : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = Vector3.zero;
    [SerializeField] float smoothTime = 0.12f;
    [SerializeField] bool pixelSnap = true;
    [SerializeField] float pixelsPerUnit = 32f;
    [SerializeField] float snapWhenSpeedBelow = 0.05f;

    [Header("Harita Sınırları")]
    [SerializeField] bool useBounds = true;
    [Tooltip("Haritanın sol alt köşesi")]
    [SerializeField] Vector2 mapMin = new Vector2(-50f, -35f);
    [Tooltip("Haritanın sağ üst köşesi")]
    [SerializeField] Vector2 mapMax = new Vector2(50f, 35f);

    Vector3 velocity;
    Rigidbody2D targetRb;
    Camera cam;

    void Awake()
    {
        if (target) targetRb = target.GetComponentInParent<Rigidbody2D>();
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = target.position + offset;
        desired.z = transform.position.z;

        Vector3 smoothed = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);

        // Kamera sınırı
        if (useBounds && cam != null)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            smoothed.x = Mathf.Clamp(smoothed.x, mapMin.x + halfW, mapMax.x - halfW);
            smoothed.y = Mathf.Clamp(smoothed.y, mapMin.y + halfH, mapMax.y - halfH);
        }

        // Pixel snap
        bool shouldSnap = pixelSnap && pixelsPerUnit > 0f &&
                          (!targetRb || targetRb.linearVelocity.sqrMagnitude < snapWhenSpeedBelow * snapWhenSpeedBelow);

        if (shouldSnap)
        {
            smoothed.x = Mathf.Round(smoothed.x * pixelsPerUnit) / pixelsPerUnit;
            smoothed.y = Mathf.Round(smoothed.y * pixelsPerUnit) / pixelsPerUnit;
        }

        transform.position = smoothed;
    }

    // Editor'da sınırları göster
    void OnDrawGizmosSelected()
    {
        if (!useBounds) return;
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((mapMin.x + mapMax.x) / 2f, (mapMin.y + mapMax.y) / 2f, 0f);
        Vector3 size   = new Vector3(mapMax.x - mapMin.x, mapMax.y - mapMin.y, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}