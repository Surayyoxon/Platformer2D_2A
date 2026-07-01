using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("--- MAIN MENU PANELS ---")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("--- BEST SCORE DESK TEXTS ---")]
    [SerializeField] private Text bestAppleText;
    [SerializeField] private Text bestBananaText;
    [SerializeField] private Text bestStrawberryText;

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        UpdateBestScoresDesk();
    }

    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void UpdateBestScoresDesk()
    {
        // PlayerPrefs'dan eng yuqori rekordlarni olib doskaga yozish
        if (bestAppleText != null) bestAppleText.text = PlayerPrefs.GetInt("BestApple", 0).ToString();
        if (bestBananaText != null) bestBananaText.text = PlayerPrefs.GetInt("BestCoin", 0).ToString();
        if (bestStrawberryText != null) bestStrawberryText.text = PlayerPrefs.GetInt("BestDiamond", 0).ToString();
    }

    #region BUTTONS

    public void Button_Start()
    {
        PlayerPrefs.DeleteKey("HasCheckpoint");

        // GameScene o'rniga birinchi level sahnasining nomini aniq yozamiz
        SceneManager.LoadScene("Level1");
    }

    public void Button_Quit()
    {
        Application.Quit();
        Debug.Log("O'yin yopildi.");
    }

    #endregion
}