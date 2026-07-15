using UnityEngine;

/// <summary>
/// WaypointManager - Menyimpan dan menyediakan array waypoint (rute musuh).
/// 
/// CARA SETUP DI UNITY:
/// 1. Buat GameObject kosong bernama "WaypointManager" di scene.
/// 2. Buat beberapa child GameObject bernama "Waypoint_01", "Waypoint_02", dst.
///    Posisikan sesuai jalur yang diinginkan, titik terakhir = Main Tower.
/// 3. Assign script ini ke GameObject "WaypointManager".
/// 4. Drag semua child waypoint ke array "waypoints" di Inspector.
/// </summary>
public class WaypointManager : MonoBehaviour
{
    // ─── Singleton ────────────────────────────────────────────────────────────
    public static WaypointManager Instance { get; private set; }

    // ─── Inspector Fields ──────────────────────────────────────────────────────
    [Header("Waypoints (Urutan dari Spawn → Main Tower)")]
    [Tooltip("Drag semua Transform waypoint ke sini, berurutan dari titik spawn hingga Main Tower")]
    public Transform[] waypoints;

    [Header("Gizmo Settings")]
    public Color lineColor   = Color.yellow;
    public Color pointColor  = Color.cyan;
    public float pointRadius = 0.3f;

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

    // ─── API publik ───────────────────────────────────────────────────────────
    /// <summary>Kembalikan seluruh array waypoint</summary>
    public Transform[] GetWaypoints() => waypoints;

    /// <summary>Kembalikan waypoint pada index tertentu</summary>
    public Transform GetWaypoint(int index)
    {
        if (index < 0 || index >= waypoints.Length)
        {
            Debug.LogWarning($"[WaypointManager] Index {index} di luar batas!");
            return null;
        }
        return waypoints[index];
    }

    /// <summary>Total jumlah waypoint</summary>
    public int Count => waypoints.Length;

    // ─── Gizmo (visualisasi jalur di Editor) ─────────────────────────────────
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            // Gambar bola di setiap waypoint
            Gizmos.color = (i == waypoints.Length - 1) ? Color.red : pointColor;
            Gizmos.DrawSphere(waypoints[i].position, pointRadius);

            // Gambar label index
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(
                waypoints[i].position + Vector3.up * 0.5f,
                i == waypoints.Length - 1 ? $"[{i}] MAIN TOWER" : $"[{i}]"
            );
            #endif

            // Gambar garis antar waypoint
            if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
            {
                Gizmos.color = lineColor;
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}
