using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WorldIntro : MonoBehaviour
{
    [Header("Yazı")]
    public TextMeshProUGUI worldText;   // "WORLD 1" yazısı
    public string worldName = "WORLD 1";

    [Header("Animasyon")]
    public float slideInDuration  = 0.6f;  // Sağdan ortaya gelme süresi
    public float holdDuration     = 1.2f;  // Ortada bekleme süresi
    public float slideOutDuration = 0.5f;  // Ortadan sola gitme süresi

    [Header("Spawner")]
    public MonoBehaviour[] spawnersToEnable; // Inspector'dan EnemySpawnerOutsideCamera'ları sürükle

    void Start()
    {
        // Spawner'ları başta kapat
        foreach (var s in spawnersToEnable)
            if (s) s.enabled = false;

        if (worldText) worldText.text = worldName;

        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        RectTransform rt = worldText.GetComponent<RectTransform>();

        float screenW = Screen.width;

        // Başlangıç: ekranın sağ dışında
        Vector2 startPos  = new Vector2(screenW + 300f, 0f);
        // Orta: ekran merkezi
        Vector2 centerPos = new Vector2(0f, 0f);
        // Bitiş: ekranın sol dışında
        Vector2 endPos    = new Vector2(-screenW - 300f, 0f);

        rt.anchoredPosition = startPos;
        worldText.gameObject.SetActive(true);

        // Sağdan ortaya gel
        yield return StartCoroutine(MoveUI(rt, startPos, centerPos, slideInDuration, true));

        // Ortada bekle
        yield return new WaitForSeconds(holdDuration);

        // Ortadan sola git
        yield return StartCoroutine(MoveUI(rt, centerPos, endPos, slideOutDuration, false));

        // Yazıyı gizle
        worldText.gameObject.SetActive(false);

        // Spawner'ları aç — düşmanlar gelmeye başlasın
        foreach (var s in spawnersToEnable)
            if (s) s.enabled = true;
    }

    IEnumerator MoveUI(RectTransform rt, Vector2 from, Vector2 to, float duration, bool easeOut)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Ease
            float curved = easeOut
                ? 1f - Mathf.Pow(1f - t, 3f)  // Ease out cubic — yavaşlayarak gelir
                : Mathf.Pow(t, 2f);             // Ease in quad — hızlanarak gider

            rt.anchoredPosition = Vector2.LerpUnclamped(from, to, curved);
            yield return null;
        }
        rt.anchoredPosition = to;
    }
}