using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Objeleri")]
    public GameObject pausePanel;       // Kararan arka plan + butonların parent'ı
    public CanvasGroup canvasGroup;     // Fade için (pausePanel'e ekle)

    [Header("Ayarlar")]
    public float fadeDuration = 0.2f;   // Açılış/kapanış hızı
    public string mainMenuScene = "MainMenu";

    bool isPaused = false;

    void Start()
    {
        // Başta kapalı olsun
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Pause butonu OnClick'e bağla
    public void OnPauseClick()
    {
        if (!isPaused) OpenPause();
    }

    public void OnContinueClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        ClosePause();
    }

    public void OnExitClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        // Önce timeScale sıfırla, sonra sahne geç
        Time.timeScale = 1f;
        isPaused = false;

        if (SceneTransitioner.Instance != null)
            SceneTransitioner.Instance.LoadScene(mainMenuScene);
        else
            SceneManager.LoadScene(mainMenuScene);
    }

    void OpenPause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        StartCoroutine(FadeCanvasGroup(0f, 1f));
        Time.timeScale = 0f; // Oyunu durdur
    }

    void ClosePause()
    {
        StartCoroutine(ClosePauseRoutine());
    }

    IEnumerator ClosePauseRoutine()
    {
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f));
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Oyunu devam ettir
        isPaused = false;
    }

    IEnumerator FadeCanvasGroup(float from, float to)
    {
        if (canvasGroup == null) yield break;

        float elapsed = 0f;
        // Time.timeScale 0 olunca WaitForSeconds çalışmaz — unscaledDeltaTime kullan
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}