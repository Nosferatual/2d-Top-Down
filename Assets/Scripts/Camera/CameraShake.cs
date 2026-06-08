using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    void Awake()
    {
        Instance = this;
    }

    // Dışarıdan çağır: CameraShake.Instance.Shake(0.15f, 0.3f)
    public void Shake(float magnitude = 0.15f, float duration = 0.3f)
    {
        StartCoroutine(ShakeRoutine(magnitude, duration));
    }

    IEnumerator ShakeRoutine(float magnitude, float duration)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Sarsıntı giderek azalsın
            float strength = Mathf.Lerp(magnitude, 0f, elapsed / duration);

            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;

            transform.localPosition = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}