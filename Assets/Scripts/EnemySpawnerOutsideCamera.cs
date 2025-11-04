using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class EnemySpawnerOutsideCamera : MonoBehaviour
{
    [Header("Zorunlu")]
    public GameObject enemyPrefab;

    [Header("Zamanlama")]
    public float interval = 1.25f;
    public int   maxAlive = 20;

    [Header("Konumlandırma")]
    public float screenMargin = 2.0f;         // Ekranın dışına taşma mesafesi (dünya birimi)
    public float minDistanceFromPlayer = 1.5f;// Oyuncuya çok yakın olmasın
    public int   maxTries = 12;               // Uygun yer arama deneme sayısı

    [Header("Çakışma Kontrol (opsiyonel)")]
    public LayerMask blockedMask;             // Duvar/engel layer’ı (boş bırakabilirsin)
    public float   overlapRadius = 0.3f;      // Spawn çakışma yarıçapı

    Camera cam;
    Transform player;
    int alive;

    void Awake()
    {
        cam = Camera.main;
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void OnEnable()  { StartCoroutine(Loop()); }
    void OnDisable() { StopAllCoroutines(); }

    IEnumerator Loop()
    {
        var wait = new WaitForSeconds(interval);
        while (true)
        {
            if (enemyPrefab && alive < maxAlive && cam)
            {
                if (TryGetSpawnPos(out Vector3 pos))
                {
                    var go = Instantiate(enemyPrefab, pos, Quaternion.identity);
                    alive++;

                    // Öldüğünde sayaç düşsün
                    var counter = go.AddComponent<OnDestroyCounter>();
                    counter.onDestroyed = () => alive--;
                }
            }
            yield return wait;
        }
    }

    bool TryGetSpawnPos(out Vector3 pos)
    {
        for (int i = 0; i < maxTries; i++)
        {
            pos = RandomPointOutsideCamera(cam, screenMargin);
            pos.z = 0f;

            if (player && (pos - player.position).sqrMagnitude < minDistanceFromPlayer * minDistanceFromPlayer)
                continue;

            if (blockedMask.value != 0 && Physics2D.OverlapCircle(pos, overlapRadius, blockedMask))
                continue;

            return true;
        }
        pos = Vector3.zero;
        return false;
    }

    static Vector3 RandomPointOutsideCamera(Camera cam, float pad)
    {
        // Ortografik kamera varsayıldı
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float cx = cam.transform.position.x;
        float cy = cam.transform.position.y;

        float left   = cx - halfW - pad;
        float right  = cx + halfW + pad;
        float bottom = cy - halfH - pad;
        float top    = cy + halfH + pad;

        int side = Random.Range(0, 4); // 0=L,1=R,2=T,3=B
        switch (side)
        {
            case 0:  return new Vector3(left,  Random.Range(bottom, top),  0f); // Sol kenar
            case 1:  return new Vector3(right, Random.Range(bottom, top),  0f); // Sağ kenar
            case 2:  return new Vector3(Random.Range(left, right),  top,   0f); // Üst kenar
            default: return new Vector3(Random.Range(left, right),  bottom,0f); // Alt kenar
        }
    }

    // küçük yardımcı sınıf
    class OnDestroyCounter : MonoBehaviour
    {
        public System.Action onDestroyed;
        void OnDestroy() { onDestroyed?.Invoke(); }
    }
}
