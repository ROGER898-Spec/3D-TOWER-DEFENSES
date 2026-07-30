using UnityEngine;

public enum DifficultyMode { Easy, Normal, Hard }

/// <summary>
/// DifficultyManager - Menyimpan difficulty yang dipilih pemain, dan sediakan
/// multiplier tambahan buat stat musuh (di ATAS stage scaling yang sudah ada).
///
/// CATATAN: Angka multiplier di bawah ini PLACEHOLDER (belum ada di tabel balancing
/// resmi). Sengaja dipusatkan di 1 method (GetEnemyHealthMultiplier) supaya gampang
/// diubah nanti begitu tim kasih angka final — tidak perlu ubah script lain.
///
/// CARA SETUP DI UNITY:
/// 1. GameObject > Create Empty, rename "DifficultyManager".
/// 2. Attach script ini.
/// 3. Set "Current Difficulty" di Inspector (sementara manual, nanti dari Main Menu).
/// 4. DontDestroyOnLoad aktif otomatis, supaya nanti kalau pindah scene
///    (Main Menu -> Gameplay), pilihan difficulty tetap kebawa.
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Difficulty aktif")]
    [Tooltip("Sementara diatur manual di sini. Nanti dipanggil dari Main Menu lewat SetDifficulty().")]
    public DifficultyMode currentDifficulty = DifficultyMode.Normal;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Dipanggil dari Main Menu nanti, saat pemain pilih difficulty</summary>
    public void SetDifficulty(DifficultyMode mode)
    {
        currentDifficulty = mode;
        Debug.Log($"[DifficultyManager] Difficulty diset ke {mode}");
    }

    /// <summary>
    /// Multiplier HP musuh berdasar difficulty. PLACEHOLDER — konfirmasi ke tim
    /// balancing untuk angka final, tinggal ubah di sini kalau sudah ada.
    /// </summary>
    public float GetEnemyHealthMultiplier()
    {
        switch (currentDifficulty)
        {
            case DifficultyMode.Easy: return 0.8f;  // musuh 20% lebih lemah
            case DifficultyMode.Hard: return 1.3f;  // musuh 30% lebih kuat
            default:                  return 1f;    // Normal = tanpa perubahan
        }
    }
}
