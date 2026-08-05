using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIManager - Menyambungkan event dari sistem lain (uang, nyawa, stage/wave, menang/kalah)
/// ke tampilan Text dan Panel di Canvas. Tidak punya logic gameplay sendiri.
///
/// CARA SETUP DI UNITY: lihat instruksi lengkap dari chat.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Referensi (drag manual)")]
    [Tooltip("Drag GameObject WaveSpawner")]
    public WaveSpawner waveSpawner;
    [Tooltip("Drag GameObject StageManager")]
    public StageManager stageManager;

    [Header("HUD Text")]
    public Text moneyText;
    public Text livesText;
    public Text stageWaveText;

    [Header("Panel Menang/Kalah")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Panel Pause")]
    [Tooltip("Drag PausePanel dari GameCanvas")]
    public GameObject pausePanel;

    [Header("Panel Awal dari Main Menu")]
    [Tooltip("Drag HUD dari Canvas")]
    public GameObject hudPanel;

    [Tooltip("Drag UpgradePanel dari Canvas")]
    public GameObject upgradePanel;

    [Tooltip("Drag SettingsPanel dari Canvas")]
    public GameObject settingsPanel;

    [Tooltip("Drag InventoryPanel dari Canvas")]
    public GameObject inventoryPanel;
    
    [Header("Kontrol Volume")]
    [Tooltip("Drag GameObject VolumeSlider dari PausePanel")]
    public GameObject volumeSlider;

    [Header("Informasi Panel Menang")]
    [Tooltip("Drag StageCompletedText dari VictoryPanel")]
    public TMP_Text stageCompletedText;

    [Tooltip("Drag TotalScoreText dari VictoryPanel")]
    public TMP_Text totalScoreText;

    [Tooltip("Drag NextStageText dari NextStageButton")]
    public TMP_Text nextStageText;

    [Tooltip("Scene yang dibuka setelah seluruh stage selesai")]
    public string nextSceneName = "MainMenu";

    [Tooltip("Nama Main Menu Scene tanpa .unity")]
    public string mainMenuSceneName = "MainMenuScene";

    // Menandakan apakah pemain sudah menyelesaikan seluruh stage.
    private bool allStagesCompleted = false;

    // Untuk Total score = energi alam
    // Mencatat total Energi Alam yang diperoleh selama stage aktif.
    private int stageEarnedEnergy = 0;

    // Menyimpan jumlah uang terakhir untuk menghitung perubahannya.
    private int lastMoneyAmount = 0;

    private bool hasMoneySnapshot = false;

    private enum PanelReturnTarget
    {
        HUD,
        Pause,
        MainMenu
    }

private PanelReturnTarget panelReturnTarget =
    PanelReturnTarget.HUD;

    private void OnEnable()
    {
        BuildManager.OnMoneyChanged   += UpdateMoney;
        MainTower.OnLivesChanged      += UpdateLives;
        StageManager.OnStageChanged   += UpdateStageDisplay;
        StageManager.OnStageCompleted += HandleStageCompleted;
        WaveSpawner.OnWaveStart       += UpdateWaveDisplay;
        GameManager.OnStateChanged    += HandleStateChanged;
    }

    private void OnDisable()
    {
        BuildManager.OnMoneyChanged   -= UpdateMoney;
        MainTower.OnLivesChanged      -= UpdateLives;
        StageManager.OnStageChanged   -= UpdateStageDisplay;
        StageManager.OnStageCompleted -= HandleStageCompleted;
        WaveSpawner.OnWaveStart       -= UpdateWaveDisplay;
        GameManager.OnStateChanged    -= HandleStateChanged;
    }

    private void Start()
    {
        if (winPanel != null)  winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (volumeSlider != null)
            volumeSlider.SetActive(false);

        OpenRequestedPanelFromMainMenu();

        // Isi nilai awal supaya tidak kosong sebelum event pertama terjadi
        if (moneyText != null && BuildManager.Instance != null)
            UpdateMoney(BuildManager.Instance.GetMoney());

        UpdateWaveDisplay(0);
    }

    private void UpdateMoney(int amount)
    {
        if (moneyText != null)
            moneyText.text = $"Energi Alam: {amount}";

        // Untuk total score = energi alam

        // Pemanggilan pertama hanya dijadikan nilai awal.
        if (!hasMoneySnapshot)
        {
            lastMoneyAmount = amount;
            hasMoneySnapshot = true;
            return;
        }

        int moneyDifference = amount - lastMoneyAmount;

        // Hanya perubahan positif yang dihitung sebagai Energi Alam diperoleh.
        // Pengeluaran untuk summon/upgrade tidak mengurangi Stage Reward.
        if (moneyDifference > 0)
        {
            stageEarnedEnergy += moneyDifference;
        }

        lastMoneyAmount = amount;
    }

    private void UpdateLives(int lives)
    {
        if (livesText != null)
            livesText.text = $"Nyawa: {lives}";
    }

    private void UpdateStageDisplay(int stageNumber)
    {
        ResetStageRewardTracking(); 
        UpdateWaveDisplay(0); // reset tampilan wave tiap kali pindah stage baru
    }

    // Untuk total score = energi alam
    private void ResetStageRewardTracking()
    {
        stageEarnedEnergy = 0;

        if (BuildManager.Instance != null)
        {
            lastMoneyAmount = BuildManager.Instance.GetMoney();
            hasMoneySnapshot = true;
        }
        else
        {
            lastMoneyAmount = 0;
            hasMoneySnapshot = false;
        }
    }

    private void UpdateWaveDisplay(int waveIndex)
    {
        if (stageWaveText == null) return;

        int stageDisplay = stageManager != null ? stageManager.GetCurrentStageDisplay() : 1;
        int totalStages  = stageManager != null ? stageManager.GetTotalStages() : 1;
        int waveDisplay  = waveIndex + 1;
        int totalWaves   = waveSpawner != null ? waveSpawner.GetTotalWaves() : 0;

        stageWaveText.text = $"Stage {stageDisplay}/{totalStages}   Wave {waveDisplay}/{totalWaves}";
    }

    // Untuk total score = energi alam
    private void UpdateTotalScoreText()
    {
        if (totalScoreText != null)
        {
            totalScoreText.text =
                $"STAGE REWARD: +{stageEarnedEnergy:N0}";
        }
    }

        /// <summary>
    /// Mengambil total score dari GameManager
    /// lalu menampilkannya pada Victory Panel.
    /// </summary>
    // private void UpdateTotalScoreText()
    // {
    //     int score = GameManager.Instance != null
    //         ? GameManager.Instance.GetTotalScore()
    //         : 0;

    //     if (totalScoreText != null)
    //     {
    //         totalScoreText.text = $"TOTAL SCORE: {score:N0}";
    //     }
    // }

 

        /// <summary>
    /// Dipanggil ketika satu stage selesai,
    /// tetapi masih ada stage berikutnya.
    /// </summary>
    private void HandleStageCompleted(int completedStageNumber)
    {
         // Masih ada stage berikutnya dalam scene ini.
        allStagesCompleted = false;

        // Isi nomor stage yang baru diselesaikan.
        if (stageCompletedText != null)
        {
            stageCompletedText.text =
                $"STAGE {completedStageNumber} COMPLETED";
        }

        // Isi total score terbaru.
        UpdateTotalScoreText();

        // Tulisan tombol untuk melanjutkan stage berikutnya.
        if (nextStageText != null)
        {
            nextStageText.text = "NEXT STAGE";
        }

        // Pastikan panel kalah tidak ikut terbuka.
        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        // Tampilkan panel kemenangan di atas panel UI lainnya.
        if (winPanel != null)
        {
            winPanel.transform.SetAsLastSibling();
            winPanel.SetActive(true);
        }

        // Jeda permainan sampai tombol Next Stage ditekan.
        Time.timeScale = 0f;
    }

        /// <summary>
    /// Dipanggil oleh tombol Next Stage.
    /// Melanjutkan stage dalam scene atau membuka scene berikutnya.
    /// </summary>
    public void OnNextStageButtonClicked()
    {
        Debug.Log("[UIManager] Tombol NEXT STAGE berhasil diklik.");
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        // Waktu harus dijalankan kembali sebelum melanjutkan permainan
        // atau berpindah scene.
        Time.timeScale = 1f;

        // Jika seluruh stage sudah selesai,
        // buka scene yang ditentukan pada Inspector.
        if (allStagesCompleted)
        {
            if (string.IsNullOrWhiteSpace(nextSceneName))
            {
                Debug.LogError(
                    "[UIManager] Next Scene Name belum diisi."
                );
                return;
            }

            SceneManager.LoadScene(nextSceneName);
            return;
        }

        // Jika masih ada stage dalam scene yang sama,
        // minta StageManager memulainya.
        if (stageManager != null)
        {
            stageManager.ContinueToNextStage();
        }
        else
        {
            Debug.LogError(
                "[UIManager] StageManager belum di-drag."
            );
        }
    }


    public void OnTryAgainButtonClicked()
    {
        if (stageManager != null)
        {
            PlayerPrefs.SetInt(
                "RetryStageIndex",
                stageManager.GetCurrentStageIndex()
            );

            PlayerPrefs.Save();
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

        /// <summary>
    /// Dipanggil oleh tombol Pause pada HUD.
    /// </summary>
    public void OnPauseButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
        }
    }

    /// <summary>
    /// Dipanggil oleh tombol X atau Resume pada PausePanel.
    /// </summary>
    public void OnResumeButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    public void OnPauseSettingButtonClicked()
    {
        if (settingsPanel == null)
        {
            Debug.LogError(
                "[UIManager] SettingsPanel belum di-drag ke Inspector."
            );

            return;
        }

        panelReturnTarget = PanelReturnTarget.Pause;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        settingsPanel.SetActive(true);
        settingsPanel.transform.SetAsLastSibling();

        Debug.Log(
            "[UIManager] SettingsPanel dibuka dari PausePanel."
        );
    }

        /// <summary>
    /// Menampilkan atau menyembunyikan slider volume.
    /// </summary>
    public void OnSoundButtonClicked()
    {
        if (volumeSlider == null)
        {
            Debug.LogError(
                "[UIManager] VolumeSlider belum di-drag ke Inspector."
            );
            return;
        }

        bool sliderAkanDitampilkan = !volumeSlider.activeSelf;

        volumeSlider.SetActive(sliderAkanDitampilkan);

        Debug.Log(
            sliderAkanDitampilkan
                ? "[UIManager] Volume Slider ditampilkan."
                : "[UIManager] Volume Slider disembunyikan."
        );
    }


        /// <summary>
    /// Dipanggil oleh tombol Home pada PausePanel.
    /// Kembali ke Main Menu Scene.
    /// </summary>
    public void OnHomeButtonClicked()
    {
        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Debug.LogError(
                "[UIManager] Main Menu Scene Name belum diisi."
            );
            return;
        }

        Debug.Log(
            $"[UIManager] Membuka Main Menu Scene: {mainMenuSceneName}"
        );

        // Waktu harus kembali berjalan sebelum berpindah scene.
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void HandleStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.Win)
        {
            // Pastikan PausePanel tertutup ketika pemain menang.
            if (pausePanel != null)
                pausePanel.SetActive(false);

            ShowWinPanel();
        }
        else if (state == GameManager.GameState.Lose)
        {
            // Tutup panel lain.
            if (pausePanel != null)
                pausePanel.SetActive(false);

            if (winPanel != null)
                winPanel.SetActive(false);

            // Tampilkan LosePanel.
            if (losePanel != null)
            {
                losePanel.transform.SetAsLastSibling();
                losePanel.SetActive(true);
            }
        }
        else if (state == GameManager.GameState.Paused)
        {
            // Tampilkan PausePanel di depan panel lainnya.
            if (pausePanel != null)
            {
                pausePanel.transform.SetAsLastSibling();
                pausePanel.SetActive(true);
            }
        }
        else if (state == GameManager.GameState.Playing)
        {
            // Tutup PausePanel ketika permainan dilanjutkan.
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }
    }

    private void ShowWinPanel()
    {

        // Seluruh stage dalam scene sudah selesai.
        allStagesCompleted = true;

        int completedStage = stageManager != null
            ? stageManager.GetCurrentStageNumber()
            : 1;

        if (stageCompletedText != null)
            stageCompletedText.text = $"STAGE {completedStage} COMPLETED";

        // Tampilkan skor akhir.
        UpdateTotalScoreText();

        if (nextStageText != null)
        {
            nextStageText.text = "NEXT STAGE";
        }

        if (losePanel != null)
            losePanel.SetActive(false);

        if (winPanel != null)
        {
            winPanel.transform.SetAsLastSibling();
            winPanel.SetActive(true);
        }
    }

    private void OpenRequestedPanelFromMainMenu()
    {
        
        BattleEntryPanel requestedPanel =
            BattleSceneRequest.ConsumeRequestedPanel();

        panelReturnTarget =
            requestedPanel == BattleEntryPanel.HUD
                ? PanelReturnTarget.HUD
                : PanelReturnTarget.MainMenu;

        if (hudPanel != null)
            hudPanel.SetActive(false);

        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        switch (requestedPanel)
        {
            case BattleEntryPanel.Upgrade:
                ShowEntryPanel(upgradePanel);

                Time.timeScale = 0f;
                break;

            case BattleEntryPanel.Settings:
                ShowEntryPanel(settingsPanel);

                Time.timeScale = 0f;
                break;

            case BattleEntryPanel.Inventory:
                ShowEntryPanel(inventoryPanel);

                Time.timeScale = 0f;
                break;

            case BattleEntryPanel.HUD:
            default:
                ShowEntryPanel(hudPanel);

                Time.timeScale = 1f;
                break;
        }

        Debug.Log(
            $"[UIManager] Panel awal yang dibuka: {requestedPanel}"
        );
    }

    public void OnCloseEntryPanelClicked()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (volumeSlider != null)
            volumeSlider.SetActive(false);

        switch (panelReturnTarget)
        {
            case PanelReturnTarget.MainMenu:
                Time.timeScale = 1f;
                SceneManager.LoadScene(mainMenuSceneName);
                break;

            case PanelReturnTarget.Pause:
                if (pausePanel != null)
                {
                    pausePanel.SetActive(true);
                    pausePanel.transform.SetAsLastSibling();
                }

                Time.timeScale = 0f;
                break;

            case PanelReturnTarget.HUD:
            default:
                if (hudPanel != null)
                {
                    hudPanel.SetActive(true);
                    hudPanel.transform.SetAsLastSibling();
                }

                Time.timeScale = 1f;
                break;
        }
    }

    private void ShowEntryPanel(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogError(
                "[UIManager] Salah satu referensi panel belum diisi di Inspector."
            );

            return;
        }

        panel.SetActive(true);

        panel.transform.SetAsLastSibling();
    }
}
