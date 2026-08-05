using UnityEngine;

/// <summary>
/// GameManager - "otak" paling atas yang membungkus semua sistem lain.
/// Tidak tahu detail internal Tower/Enemy — cuma dengarkan event
/// menang/kalah dari MainTower dan WaveSpawner, lalu atur state game.
///
/// CARA SETUP DI UNITY:
/// 1. GameObject > Create Empty, rename "GameManager".
/// 2. Attach script ini. Tidak perlu drag referensi apa pun di Inspector
///    karena event yang didengarkan semuanya static.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, Win, Lose, Paused }

    [Header("State saat ini (read-only, buat debug)")]
    [SerializeField] private GameState currentState = GameState.Playing;

    [Header("Score")]
    [SerializeField] private int totalScore = 0;

    /// <summary>
    /// Menambahkan poin ke total score selama permainan berlangsung.
    /// </summary>
    public void AddScore(int amount)
    {
        // Tolak nilai nol, negatif, atau penambahan setelah permainan selesai.
        if (amount <= 0 || currentState != GameState.Playing)
            return;

        totalScore += amount;
    }

    /// <summary>
    /// Mengembalikan total score saat ini kepada UIManager.
    /// </summary>
    public int GetTotalScore()
    {
        return totalScore;
    }

    public static event System.Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        MainTower.OnGameOver += HandleLose;
        StageManager.OnAllStagesComplete += HandleWin; // ★ diganti dari WaveSpawner, sekarang menang = semua STAGE selesai
    }

    private void OnDisable()
    {
        MainTower.OnGameOver -= HandleLose;
        StageManager.OnAllStagesComplete -= HandleWin;
    }

    private void Start()
    {
        totalScore = 0;
        SetState(GameState.Playing);
    }

    // ─── Handler event kalah ───────────────────────────────────────────────────
    private void HandleLose()
    {
        if (currentState == GameState.Lose || currentState == GameState.Win) return;

        Debug.Log("[GameManager] KALAH — Main Tower hancur.");
        SetState(GameState.Lose);
        Time.timeScale = 0f; // hentikan semua gameplay (musuh, tower, spawn)
    }

    // ─── Handler event menang ───────────────────────────────────────────────────
    private void HandleWin()
    {
        if (currentState == GameState.Lose || currentState == GameState.Win) return;

        Debug.Log("[GameManager] MENANG — semua STAGE berhasil dilewati!");

        MainTower tower = FindAnyObjectByType<MainTower>();
        if (tower != null)
        {
            int stars = tower.GetStarRating();
            Debug.Log($"[GameManager] Hasil akhir: {stars} BINTANG (HP Main Tower: {tower.GetCurrentLives()}/{tower.GetMaxLives()} = {tower.GetHealthPercent() * 100f}%)");
        }

        SetState(GameState.Win);
        Time.timeScale = 0f;
    }

    private void SetState(GameState newState)
    {
        currentState = newState;
        OnStateChanged?.Invoke(currentState);
    }

    /// <summary>
/// Menghentikan permainan dan mengubah state menjadi Paused.
/// </summary>
public void PauseGame()
{
    // Pause hanya dapat dilakukan saat permainan sedang berjalan.
    if (currentState != GameState.Playing)
        return;

    SetState(GameState.Paused);
    Time.timeScale = 0f;
}

    /// <summary>
    /// Melanjutkan permainan setelah Pause.
    /// </summary>
    public void ResumeGame()
    {
        // Resume hanya dapat dilakukan ketika game sedang Pause.
        if (currentState != GameState.Paused)
            return;

        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }
    
    // ─── Dipanggil dari tombol UI nanti (Restart / Main Menu) ─────────────────
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public GameState GetCurrentState() => currentState;
}