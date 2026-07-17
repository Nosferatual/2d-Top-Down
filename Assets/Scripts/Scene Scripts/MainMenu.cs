using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Sahne İsimleri")]
    public string characterSelectScene = "CharacterSelect";

    void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();
    }

    public void OnBaslaClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        if (SceneTransitioner.Instance != null)
            SceneTransitioner.Instance.LoadScene(characterSelectScene);
        else
            SceneManager.LoadScene(characterSelectScene);
    }

    public void OnAyarlarClick()
    {
        // OptionsMenu scripti hallediyor — bu fonksiyon artık kullanılmıyor
        // OnClick'i OptionsMenu'deki OnOptionsClick'e bağla
    }

    public void OnCikisClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        Application.Quit();
    }
}