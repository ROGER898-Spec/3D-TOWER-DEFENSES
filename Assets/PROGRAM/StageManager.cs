using UnityEngine;

/// <summary>
/// StageManager - Mengatur transisi otomatis antar stage.
/// Setiap kali WaveSpawner bilang "wave di stage ini habis", StageManager
/// otomatis mulai stage berikutnya (atau umumkan menang total kalau sudah stage terakhir).
///
/// CARA SETUP DI UNITY:
/// 1. GameObject > Create Empty, rename "StageManager".
/// 2. Attach script ini.
/// 3. Drag GameObject "WaveSpawner" yang sudah ada ke field "Wave Spawner".
/// 4. Isi array "Stages" — tiap elemen adalah 1 stage dengan wave-nya sendiri.
///    (Pindahkan data Wave yang sudah kamu isi di WaveSpawner ke Stages[0] di sini.)
/// </summary>
public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Referensi")]
    [Tooltip("Drag GameObject WaveSpawner yang sudah ada di scene")]
    public WaveSpawner waveSpawner;

    [Header("Daftar Stage")]
    public StageData[] stages;

    private int currentStageIndex = 0;

    public static event System.Action<int> OnStageChanged; // membawa nomor stage
    
    public static event System.Action<int> OnStageCompleted;     
    public static event System.Action OnAllStagesComplete;      // MENANG TOTAL

    private int pendingNextStageIndex = -1; // Menyimpan indeks stage berikutnya.
    private bool waitingForNextStage = false; // Menandakan bahwa permainan sedang menunggu pemain menekan tombol Next Stage

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
        WaveSpawner.OnAllWavesComplete += HandleStageComplete;
    }

    private void OnDisable()
    {
        WaveSpawner.OnAllWavesComplete -= HandleStageComplete;
    }

    private void Start()
    {
        if (waveSpawner == null)
        {
            Debug.LogError("[StageManager] Wave Spawner belum di-drag di Inspector!");
            return;
        }

        if (stages == null || stages.Length == 0)
        {
            Debug.LogWarning("[StageManager] Belum ada data stage di array 'Stages'!");
            return;
        }

        BeginStage(0);
    }

    private void BeginStage(int index)
    {
        currentStageIndex = index;
        StageData data = stages[index];

        Debug.Log($"[StageManager] Memulai Stage {data.stageNumber} ({index + 1}/{stages.Length})");

        waveSpawner.StartStage(data.stageNumber, data.waves);
        OnStageChanged?.Invoke(data.stageNumber);
    }
// Sebelumnya:
    // private void HandleStageComplete()
    // {
    //     int nextIndex = currentStageIndex + 1;

    //     if (nextIndex >= stages.Length)
    //     {
    //         Debug.Log("[StageManager] SEMUA STAGE SELESAI! Pemain menang total.");
    //         OnAllStagesComplete?.Invoke();
    //         return;
    //     }

    //     BeginStage(nextIndex);
    // }

    private void HandleStageComplete()
    {
         if (waitingForNextStage)
            return;
            
        int completedStageNumber =
            stages[currentStageIndex].stageNumber;

        int nextIndex = currentStageIndex + 1;

        if (nextIndex >= stages.Length)
        {
            Debug.Log(
                "[StageManager] SEMUA STAGE SELESAI! Pemain menang total."
            );

            OnAllStagesComplete?.Invoke();
            return;
        }

        pendingNextStageIndex = nextIndex;
        waitingForNextStage = true;

        Debug.Log(
            $"[StageManager] Stage {completedStageNumber} selesai. " +
            "Menunggu tombol Next Stage."
        );

        OnStageCompleted?.Invoke(completedStageNumber);
    }
    public int GetCurrentStageNumber() => stages.Length > 0 ? stages[currentStageIndex].stageNumber : 1;
    public int GetCurrentStageDisplay() => currentStageIndex + 1;
    public int GetTotalStages() => stages.Length;

    public void ContinueToNextStage()
    {
        if (!waitingForNextStage || pendingNextStageIndex < 0)
            return;

        int nextIndex = pendingNextStageIndex;

        pendingNextStageIndex = -1;
        waitingForNextStage = false;

        BeginStage(nextIndex);
    }
}
