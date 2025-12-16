using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawnerOutsideCamera : MonoBehaviour
{
    public GameObject enemyPrefab;   // Spawnlanacak düşman prefabı
    public float interval = 1.25f;   // Kaç saniyede bir spawn
    public float margin = 2f;        // Ekranın dışına ne kadar taşsın
    public Camera cam;               // Boş bırakırsan Main Camera kullanılır

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void OnEnable()  { StartCoroutine(SpawnLoop()); }
    void OnDisable() { StopAllCoroutines(); }

    IEnumerator SpawnLoop()
    {
        var wait = new WaitForSeconds(interval);
        while (true)
        {
            if (enemyPrefab && cam)
            {
                Vector3 pos = RandomPointOutsideCamera(cam, margin);
                Instantiate(enemyPrefab, pos, Quaternion.identity);
            }
            yield return wait;
        }
    }

    static Vector3 RandomPointOutsideCamera(Camera cam, float pad)
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 c = cam.transform.position;

        float left   = c.x - halfW - pad;
        float right  = c.x + halfW - pad * -1f; // = c.x + halfW + pad
        float bottom = c.y - halfH - pad;
        float top    = c.y + halfH + pad;

        int side = Random.Range(0, 4); // 0=L,1=R,2=T,3=B
        switch (side)
        {
            case 0:  return new Vector3(left,  Random.Range(bottom, top),  0f);
            case 1:  return new Vector3(right, Random.Range(bottom, top),  0f);
            case 2:  return new Vector3(Random.Range(left, right),  top,   0f);
            default: return new Vector3(Random.Range(left, right),  bottom,0f);
        }
    }
}
