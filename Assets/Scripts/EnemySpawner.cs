using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; // sahneye 3-6 boş Transform koy, buraya sürükle
    public float interval = 1.5f;
    public int maxAlive = 20;

    int alive;

    void OnEnable()  { StartCoroutine(Loop()); }
    void OnDisable() { StopAllCoroutines(); }

    IEnumerator Loop()
    {
        var wait = new WaitForSeconds(interval);
        while (true)
        {
            if (alive < maxAlive && spawnPoints.Length > 0)
            {
                Transform p = spawnPoints[Random.Range(0, spawnPoints.Length)];
                var go = Instantiate(enemyPrefab, p.position, Quaternion.identity);
                alive++;

                // düşman ölünce sayımı azalt
                var counter = go.AddComponent<OnDestroyCounter>();
                counter.onDestroyed = () => alive--;
            }
            yield return wait;
        }
    }

    // küçük yardımcı
    class OnDestroyCounter : MonoBehaviour
    {
        public System.Action onDestroyed;
        void OnDestroy() { if (onDestroyed != null) onDestroyed(); }
    }
}
