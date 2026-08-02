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

    // ─── Kurangi HP (dipanggil musuh biasa yang lolos) ───────────────────────
    public void TakeDamage(int amount)
    {
        currentLives -= amount;
        currentLives = Mathf.Max(0, currentLives);

        Debug.Log($"[MainTower] HP tersisa: {currentLives}/{maxLives}");
        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
            GameOver();
    }

    // ─── Hancur seketika (dipanggil kalau BOSS yang lolos) ───────────────────
    public void DestroyInstantly()
    {
        currentLives = 0;
        Debug.Log("[MainTower] Boss sampai ke Main Tower — HANCUR SEKETIKA!");
        OnLivesChanged?.Invoke(currentLives);
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

    // ─── Persentase HP, dipakai untuk hitung bintang ──────────────────────────
    public float GetHealthPercent() => maxLives > 0 ? (float)currentLives / maxLives : 0f;

    /// <summary>
    /// Hitung bintang (1-3) berdasar sisa HP Main Tower saat menang.
    /// Kriteria dari tim: 1★ di bawah 50%, 2★ 51-99%, 3★ 100%.
    /// CATATAN: kriteria asli tidak menyebut kasus TEPAT 50% masuk kategori mana —
    /// di sini sengaja dihitung sebagai 1★ (konservatif). Konfirmasi ke tim kalau
    /// perlu diubah.
    /// </summary>
    public int GetStarRating()
    {
        float percent = GetHealthPercent() * 100f;

        if (percent >= 100f) return 3;
        if (percent >= 51f) return 2;
        return 1;
    }
}