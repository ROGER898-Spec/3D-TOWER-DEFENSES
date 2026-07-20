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

        // Isi nilai awal supaya tidak kosong sebelum event pertama terjadi
        if (moneyText != null && BuildManager.Instance != null)
            UpdateMoney(BuildManager.Instance.GetMoney());

        UpdateWaveDisplay(0);
    }

    private void UpdateMoney(int amount)
    {
        if (moneyText != null)
            moneyText.text = $"Energi Alam: {amount}";
    }

    private void UpdateLives(int lives)
    {
        if (livesText != null)
            livesText.text = $"Nyawa: {lives}";
    }

    private void UpdateStageDisplay(int stageNumber)
    {
        UpdateWaveDisplay(0); // reset tampilan wave tiap kali pindah stage baru
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

        /// <summary>
    /// Mengambil total score dari GameManager
    /// lalu menampilkannya pada Victory Panel.
    /// </summary>
    private void UpdateTotalScoreText()
    {
        int score = GameManager.Instance != null
            ? GameManager.Instance.GetTotalScore()
            : 0;

        if (totalScoreText != null)
        {
            totalScoreText.text = $"TOTAL SCORE: {score:N0}";
        }
    }

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
    /// Dipanggil oleh tombol Restart pada PausePanel.
    /// Mengulang scene battle dari awal.
    /// </summary>
    public void OnRestartButtonClicked()
    {
        Debug.Log("[UIManager] Tombol Restart diklik.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
        else
        {
            Debug.LogError("[UIManager] GameManager tidak ditemukan.");
        }
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
}
