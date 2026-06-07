using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CharacterSelect : MonoBehaviour
{
    [Header("Sahne")]
    public string gameScene = "WorldOne";

    [Header("Karakter Butonları")]
    public Button mageButton;
    public Button archerButton;
    public Button comingSoonButton;

    [Header("Kilit")]
    public GameObject archerLockIcon;
    public TextMeshProUGUI archerLockText;

    [Header("Geri Butonu")]
    public Button backButton;

    void Start()
    {
        if (archerButton != null)
            archerButton.interactable = false;

        if (archerLockIcon != null)
            archerLockIcon.SetActive(true);

        if (comingSoonButton != null)
            comingSoonButton.interactable = false;
    }

    public void OnMageSelected()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMageSelect();

        PlayerPrefs.SetString("SelectedCharacter", "Mage");
        PlayerPrefs.Save();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameMusic();

        SceneManager.LoadScene(gameScene);
    }

    public void OnArcherSelected()
    {
        // Kilitli — kilit sesi çal
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayLockedCharacter();
    }

    public void OnComingSoonSelected()
    {
        // Kilitli — kilit sesi çal
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayLockedCharacter();
    }

    public void OnBackClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        SceneManager.LoadScene("MainMenu");
    }
}