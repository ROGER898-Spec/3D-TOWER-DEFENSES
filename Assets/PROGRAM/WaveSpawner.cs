using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WaveSpawner - Sistem untuk spawn musuh per gelombang (wave).
/// Letakkan script ini pada GameObject "WaveSpawner" di scene.
/// Pastikan ada WaypointManager di scene untuk menentukan jalur musuh.
/// </summary>
public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING }

    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave 1";
        public GameObject enemyPrefab;
        public int count = 10;
        public float spawnRate = 0.5f;
    }

    [Header("Wave Settings")]
    public Wave[] waves;
    public float timeBetweenWaves = 5f;

    [Header("Stage Scaling")]
    [Tooltip("Stage saat ini. HP musuh = Base HP x (1 + Stage x 0.15). Stage 1 = HP normal (x1.15).")]
    public int currentStage = 1;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("UI / Debug")]
    public bool showDebugLog = true;

    [HideInInspector] public SpawnState state = SpawnState.COUNTING;
    [HideInInspector] public int currentWaveIndex = 0;
    [HideInInspector] public float countdown = 3f;

    private bool allWavesDone = false;

    public static event System.Action<int> OnWaveStart;
    public static event System.Action<int> OnWaveComplete;
    public static event System.Action OnAllWavesComplete;

    private void Start()
    {
        if (spawnPoint == null)
            spawnPoint = transform;

        if (waves.Length == 0)
            Debug.LogWarning("[WaveSpawner] Tidak ada wave yang dikonfigurasi!");
    }

    private void Update()
    {
        if (allWavesDone) return;

        if (state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
            }
            return;
        }

        if (countdown <= 0f)
        {
            if (state != SpawnState.SPAWNING)
                StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }
        else
        {
            countdown -= Time.deltaTime;
        }
    }

    private bool EnemyIsAlive()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length > 0;
    }

    private void WaveCompleted()
    {
        if (showDebugLog)
            Debug.Log($"[WaveSpawner] Wave {currentWaveIndex + 1} selesai!");

        OnWaveComplete?.Invoke(currentWaveIndex);
        state = SpawnState.COUNTING;

        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            allWavesDone = true;
            if (showDebugLog)
                Debug.Log("[WaveSpawner] Semua wave telah selesai! Pemain menang!");
            OnAllWavesComplete?.Invoke();
            return;
        }

        countdown = timeBetweenWaves;
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        state = SpawnState.SPAWNING;

        if (showDebugLog)
            Debug.Log($"[WaveSpawner] Memulai {wave.waveName} - {wave.count} musuh (Stage {currentStage})");

        OnWaveStart?.Invoke(currentWaveIndex);

        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(1f / wave.spawnRate);
        }

        state = SpawnState.WAITING;

        if (showDebugLog)
            Debug.Log($"[WaveSpawner] Semua musuh {wave.waveName} telah di-spawn. Menunggu musuh habis...");
    }

    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("[WaveSpawner] Enemy prefab null! Periksa konfigurasi wave.");
            return;
        }

        GameObject enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        EnemyMovement em = enemy.GetComponent<EnemyMovement>();
        if (em != null)
            em.InitPath(WaypointManager.Instance.GetWaypoints());

        // ★ BARU: terapkan scaling HP sesuai stage sebelum musuh mulai jalan
        EnemyHealth eh = enemy.GetComponent<EnemyHealth>();
        if (eh != null)
            eh.ApplyStageScaling(currentStage);
    }

    /// <summary>Dipanggil dari luar (nanti StageManager) untuk pindah ke stage berikutnya</summary>
    public void SetStage(int stage)
    {
        currentStage = stage;
        if (showDebugLog)
            Debug.Log($"[WaveSpawner] Stage di-set ke {currentStage}");
    }

    /// <summary>
    /// Dipanggil oleh StageManager untuk memulai stage baru dengan wave-nya sendiri.
    /// Reset semua state spawning dan mulai dari wave pertama stage ini.
    /// </summary>
    public void StartStage(int stageNumber, Wave[] stageWaves)
    {
        currentStage = stageNumber;
        waves = stageWaves;
        currentWaveIndex = 0;
        allWavesDone = false;
        state = SpawnState.COUNTING;
        countdown = 1f; // jeda singkat sebelum wave pertama stage ini mulai

        if (showDebugLog)
            Debug.Log($"[WaveSpawner] StartStage dipanggil — Stage {stageNumber}, {waves.Length} wave.");
    }

    public int GetCurrentWaveDisplay() => currentWaveIndex + 1;
    public int GetTotalWaves() => waves.Length;
    public float GetCountdown() => Mathf.Max(0f, countdown);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPoint != null ? spawnPoint.position : transform.position, 0.5f);
        Gizmos.DrawIcon(spawnPoint != null ? spawnPoint.position : transform.position, "sv_icon_dot4_pix16_gizmo", true);
    }
}
