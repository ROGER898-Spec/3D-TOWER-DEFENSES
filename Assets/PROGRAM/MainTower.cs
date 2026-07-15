using UnityEngine;

/// <summary>
/// MainTower - Bangunan utama yang harus dilindungi player.
/// Letakkan pada waypoint terakhir (titik tujuan musuh).
/// </summary>
public class MainTower : MonoBehaviour
{
    // ─── Inspector Fields ──────────────────────────────────────────────────────
    [Header("Health")]
    public int maxLives = 20;

    // ─── State ────────────────────────────────────────────────────────────────
    private int currentLives;

    // ─── Events ───────────────────────────────────────────────────────────────
    public static event System.Action<int> OnLivesChanged;
    public static event System.Action OnGameOver;

    // ─────────────────────────────────────────────────────────────────────────

    private void Start()
    {
        currentLives = maxLives;
        OnLivesChanged?.Invoke(currentLives);
    }

    // ─── Kurangi nyawa ───────────────────────────────────────────────────────
    public void TakeDamage(int amount)
    {
        currentLives -= amount;
        currentLives = Mathf.Max(0, currentLives);

        Debug.Log($"[MainTower] Nyawa tersisa: {currentLives}/{maxLives}");
        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
            GameOver();
    }

    private void GameOver()
    {
        Debug.Log("[MainTower] GAME OVER! Main Tower hancur!");
        OnGameOver?.Invoke();
        // Di sini bisa tambahkan: Time.timeScale = 0f; atau load scene GameOver
    }

    public int GetCurrentLives() => currentLives;
    public int GetMaxLives() => maxLives;
}
