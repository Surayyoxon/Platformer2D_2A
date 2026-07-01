using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("--- GAME SCENE PANELS ---")]
    [SerializeField] private GameObject gameModePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject youWinPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("--- IN-GAME FRUIT TEXTS ---")]
    [SerializeField] private Text appleText;
    [SerializeField] private Text bananaText;
    [SerializeField] private Text strawberryText;

    [Header("--- EFFECTS ---")]
    [SerializeField] private GameObject confettiEffect;

    private bool hasCheckpoint = false;
    private Vector3 checkpointPosition;
    private GameObject player;

    // Next tugmasi bosilganda player aynan eshik nuqtasiga o'tishi uchun eshik transformini saqlaymiz
    private Transform targetDoorTransform;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ShowGameMode();
    }

    private void Update()
    {
        // Keyboard orqali Pause ochish
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameModePanel.activeSelf) TogglePause(true);
            else if (pausePanel.activeSelf) TogglePause(false);
        }
    }

    #region PANEL CONTROL

    public void ShowGameMode()
    {
        Time.timeScale = 1f;
        gameModePanel.SetActive(true);
        pausePanel.SetActive(false);
        youWinPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        SetPlayerMovement(true);
    }

    // 2) PAUSE TUGMASI BOSILGANDA ISHLAYDIGAN ASOSIY METOD
    public void TogglePause(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0f; // O'yin vaqtini to'xtatish
            gameModePanel.SetActive(false);
            pausePanel.SetActive(true);
            SetPlayerMovement(false);
        }
        else
        {
            Time.timeScale = 1f; // O'yin vaqtini davom ettirish
            gameModePanel.SetActive(true);
            pausePanel.SetActive(false);
            SetPlayerMovement(true);
        }
    }

    // Door skriptidan yutganda keyingi eshik (door) ob'ektini qabul qilamiz
    public void OnPlayerWin(int apples, int bananas, int strawberries, Transform nextRoomDoor = null)
    {
        Time.timeScale = 0f;
        gameModePanel.SetActive(false);
        youWinPanel.SetActive(true);
        SetPlayerMovement(false);

        if (confettiEffect != null) confettiEffect.SetActive(true);

        targetDoorTransform = nextRoomDoor; // 1) Aynan eshik pozitsiyasini eslab qolamiz

        CheckAndSaveBestScores(apples, bananas, strawberries);
    }

    public void OnGameOver()
    {
        Time.timeScale = 0f;
        gameModePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        SetPlayerMovement(false);
    }

    private void SetPlayerMovement(bool canMove)
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var movement = player.GetComponent<MonoBehaviour>(); // Harakat skriptingiz nomi
            if (movement != null) movement.enabled = canMove;
        }
    }

    #endregion

    #region BUTTON FUNCTIONS

    public void Button_PauseGame() // UI dagi pause tugmasiga shu funksiyani berasiz
    {
        TogglePause(true);
    }

    public void Button_Resume()
    {
        TogglePause(false);
    }

    public void Button_Restart()
    {
        Time.timeScale = 1f;
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerRespawn respawn = player.GetComponent<PlayerRespawn>();
            if (respawn != null)
            {
                respawn.RespawnCheck();
                ShowGameMode();
            }
            else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Button_MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    // 1) KEYINGI LEVEL ESHIGIDAN BOSHLASH REJIM
    // "You Win" panelidagi NEXT tugmasi bosilganda shu funksiya ishlaydi
    public void Button_NextLevel()
    {
        if (confettiEffect != null) confettiEffect.SetActive(false);
        Time.timeScale = 1f; // O'yin vaqtini yana harakatga keltiramiz

        // 1. Hozirgi turgan sahnaning indeks raqamini olamiz (masalan: Level 1 -> Index 2)
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 2. Keyingi sahnaning indeksini hisoblaymiz (Index 3)
        int nextSceneIndex = currentSceneIndex + 1;

        // 3. Agar keyingi sahna Build Settings-da mavjud bo'lsa, uni yuklaymiz
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Agar 5-sahnadan keyin boshqa level qolmagan bo'lsa -> Main Menuga qaytadi
            Debug.Log("O'yin butunlay tugadi! Barcha levellar bajarildi.");
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    #endregion

    #region DATA UPDATES

    public void UpdateInGameFruits(int apples, int bananas, int strawberries)
    {
        if (appleText != null) appleText.text = apples.ToString();
        if (bananaText != null) bananaText.text = bananas.ToString();
        if (strawberryText != null) strawberryText.text = strawberries.ToString();
    }

    private void CheckAndSaveBestScores(int apples, int bananas, int strawberries)
    {
        if (apples > PlayerPrefs.GetInt("BestApple", 0)) PlayerPrefs.SetInt("BestApple", apples);
        if (bananas > PlayerPrefs.GetInt("BestCoin", 0)) PlayerPrefs.SetInt("BestCoin", bananas);
        if (strawberries > PlayerPrefs.GetInt("BestDiamond", 0)) PlayerPrefs.SetInt("BestDiamond", strawberries);
        PlayerPrefs.Save();
    }

    public void SetCheckpoint(Vector3 pos) { checkpointPosition = pos; hasCheckpoint = true; }
    public void SetHasCheckpoint(bool state) { hasCheckpoint = state; }

    #endregion
}