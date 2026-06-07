using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Sahne İsimleri")]
    public string characterSelectScene = "CharacterSelect";

    void Start()
    {
        // Ana menü açılınca müzik başlat
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();
    }

    public void OnBaslaClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        SceneManager.LoadScene(characterSelectScene);
    }

    public void OnAyarlarClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        // Şimdilik boş
    }

    public void OnCikisClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        Application.Quit();
    }
}