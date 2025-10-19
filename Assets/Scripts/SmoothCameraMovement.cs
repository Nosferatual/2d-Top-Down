using UnityEngine;

public class SmoothCameraMovement : MonoBehaviour
{
   [SerializeField] private float damping;
   [SerializeField]private Vector3 offset;
   public Transform target;
  private Vector3 vel = Vector3.zero;
  private void Update()
  {
    Vector3 targetPosition = target.position + offset;
    targetPosition.z = transform.position.z;
    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref
    vel, damping);
  }
   
   // CameraFollow.cs
void LateUpdate()
{
    Vector3 pos = target.position;
    pos.z = transform.position.z;

    // 32, sprite başına piksel sayısı (Pixels Per Unit)
    pos.x = Mathf.Round(pos.x * 32f) / 32f;
    pos.y = Mathf.Round(pos.y * 32f) / 32f;

    transform.position = pos;
}

} 
