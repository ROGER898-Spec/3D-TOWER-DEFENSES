using UnityEngine;
public class WaypointManager : MonoBehaviour
{
    // ─── Singleton ────────────────────────────────────────────────────────────
    public static WaypointManager Instance { get; private set; }

    // ─── Setup LAMA (1 jalur, dipertahankan untuk kompatibilitas) ────────────
    [Header("[LAMA] 1 Jalur Saja (dipakai kalau 'Paths' di bawah kosong)")]
    [Tooltip("Drag semua Transform waypoint ke sini, berurutan dari titik spawn hingga Main Tower")]
    public Transform[] waypoints;

    // ─── Setup BARU (multi-jalur) ─────────────────────────────────────────────
    [System.Serializable]
    public class PathData
    {
        public string pathName = "Jalur Baru";
        [Tooltip("Berurutan dari titik spawn hingga Main Tower")]
        public Transform[] waypoints;
    }

    [Header("[BARU] Multi-Jalur (tiap musuh diundi lewat jalur yang mana)")]
    [Tooltip("Isi 2+ elemen untuk aktifkan multi-jalur. Kosongkan untuk pakai setup lama (1 jalur) di atas.")]
    public PathData[] paths;

    [Header("Gizmo Settings")]
    public Color lineColor = Color.yellow;
    public Color pointColor = Color.cyan;
    public float pointRadius = 0.3f;

    [Tooltip("Warna garis per-jalur waktu multi-jalur aktif, diulang kalau jalur lebih banyak dari warna ini")]
    public Color[] pathColors = new Color[]
    {
        Color.yellow, Color.cyan, Color.magenta, Color.green
    };

    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[WaypointManager] Duplikat ditemukan! Menghancurkan duplikat.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ─── API publik — LAMA (tetap ada, jangan dihapus, dipakai script lain) ──
    public Transform[] GetWaypoints() => waypoints;

    /// <summary>Kembalikan waypoint pada index tertentu (LAMA, single-path)</summary>
    public Transform GetWaypoint(int index)
    {
        if (index < 0 || index >= waypoints.Length)
        {
            Debug.LogWarning($"[WaypointManager] Index {index} di luar batas!");
            return null;
        }
        return waypoints[index];
    }

    /// <summary>Total jumlah waypoint jalur pertama (LAMA, single-path)</summary>
    public int Count => waypoints.Length;

    // ─── API publik — BARU (multi-jalur) ─────────────────────────────────────
    
    public Transform[] GetRandomPath()
    {
        if (paths == null || paths.Length == 0)
            return waypoints; // fallback ke setup lama

        int index = Random.Range(0, paths.Length);
        Debug.Log($"[WaypointManager] Musuh diundi lewat: {paths[index].pathName} (index {index})");
        return paths[index].waypoints;
    }

    /// <summary>Ambil jalur tertentu berdasar index (dipakai kalau perlu spesifik, bukan acak)</summary>
    public Transform[] GetPath(int index)
    {
        if (paths == null || index < 0 || index >= paths.Length)
            return waypoints;
        return paths[index].waypoints;
    }

    /// <summary>Berapa banyak jalur yang terdaftar di "Paths"</summary>
    public int GetPathCount() => paths != null ? paths.Length : 0;

    /// <summary>Apakah mode multi-jalur aktif (Paths sudah diisi)?</summary>
    public bool IsMultiPathActive() => paths != null && paths.Length > 0;

    // ─── Gizmo (visualisasi jalur di Editor) ─────────────────────────────────
    private void OnDrawGizmos()
    {
        if (IsMultiPathActive())
        {
            for (int p = 0; p < paths.Length; p++)
            {
                Color pathColor = pathColors.Length > 0 ? pathColors[p % pathColors.Length] : lineColor;
                DrawPathGizmo(paths[p].waypoints, pathColor, paths[p].pathName);
            }
        }
        else
        {
            DrawPathGizmo(waypoints, lineColor, null);
        }
    }

    private void DrawPathGizmo(Transform[] pathWaypoints, Color color, string pathLabel)
    {
        if (pathWaypoints == null || pathWaypoints.Length == 0) return;

        for (int i = 0; i < pathWaypoints.Length; i++)
        {
            if (pathWaypoints[i] == null) continue;

            Gizmos.color = (i == pathWaypoints.Length - 1) ? Color.red : pointColor;
            Gizmos.DrawSphere(pathWaypoints[i].position, pointRadius);

#if UNITY_EDITOR
            string prefix = string.IsNullOrEmpty(pathLabel) ? "" : $"{pathLabel} ";
            UnityEditor.Handles.Label(
                pathWaypoints[i].position + Vector3.up * 0.5f,
                i == pathWaypoints.Length - 1 ? $"{prefix}[{i}] MAIN TOWER" : $"{prefix}[{i}]"
            );
#endif

            if (i < pathWaypoints.Length - 1 && pathWaypoints[i + 1] != null)
            {
                Gizmos.color = color;
                Gizmos.DrawLine(pathWaypoints[i].position, pathWaypoints[i + 1].position);
            }
        }
    }
}