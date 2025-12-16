using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; 
    public float interval = 1.5f;
    public int maxAlive = 30;

    private int aliveCount = 0;

    void OnEnable()  { StartCoroutine(SpawnLoop()); }
    void OnDisable() { StopAllCoroutines(); }

    IEnumerator SpawnLoop()
    {
        var wait = new WaitForSeconds(interval);
        while (true)
        {
            if (aliveCount < maxAlive && spawnPoints.Length > 0 && enemyPrefab != null)
            {
                Transform p = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject go = Instantiate(enemyPrefab, p.position, Quaternion.identity);
                aliveCount++;

                // Düşman ölünce sayacı düşür
                var tracker = go.AddComponent<EnemyTracker>();
                tracker.spawner = this;
            }
            yield return wait;
        }
    }

    public void EnemyDied()
    {
        aliveCount--;
        if (aliveCount < 0) aliveCount = 0;
    }
}

public class EnemyTracker : MonoBehaviour
{
    public EnemySpawner spawner;
    void OnDestroy()
    {
        if (spawner != null) spawner.EnemyDied();
    }
}