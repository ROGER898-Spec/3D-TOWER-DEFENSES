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

    private void OnEnable()
    {
        BuildManager.OnMoneyChanged   += UpdateMoney;
        MainTower.OnLivesChanged      += UpdateLives;
        StageManager.OnStageChanged   += UpdateStageDisplay;
        WaveSpawner.OnWaveStart       += UpdateWaveDisplay;
        GameManager.OnStateChanged    += HandleStateChanged;
    }

    private void OnDisable()
    {
        BuildManager.OnMoneyChanged   -= UpdateMoney;
        MainTower.OnLivesChanged      -= UpdateLives;
        StageManager.OnStageChanged   -= UpdateStageDisplay;
        WaveSpawner.OnWaveStart       -= UpdateWaveDisplay;
        GameManager.OnStateChanged    -= HandleStateChanged;
    }

    private void Start()
    {
        if (winPanel != null)  winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

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

    private void HandleStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.Win && winPanel != null)
            winPanel.SetActive(true);

        if (state == GameManager.GameState.Lose && losePanel != null)
            losePanel.SetActive(true);
    }
}
