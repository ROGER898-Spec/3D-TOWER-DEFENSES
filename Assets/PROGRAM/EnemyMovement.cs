using UnityEngine;

/// <summary>
/// EnemyMovement - Menggerakkan musuh melalui waypoint satu per satu.
/// Letakkan script ini pada prefab Enemy.
/// 
/// REQUIREMENT:
/// - GameObject enemy harus bertag "Enemy"
/// - Enemy prefab sebaiknya punya Capsule/Sphere Collider & Rigidbody (Kinematic)
/// - Script EnemyHealth juga diletakkan di prefab yang sama
/// </summary>
[RequireComponent(typeof(EnemyHealth))]
public class EnemyMovement : MonoBehaviour
{
    // ─── Inspector Fields ──────────────────────────────────────────────────────
    [Header("Movement")]
    [Tooltip("Kecepatan gerak musuh")]
    public float speed = 3f;

    [Tooltip("Radius untuk dianggap sudah mencapai waypoint")]
    public float waypointReachThreshold = 0.1f;

    [Header("Rotation")]
    [Tooltip("Apakah musuh memutar badan ke arah gerak?")]
    public bool rotateTowardsTarget = true;

    [Tooltip("Kecepatan rotasi musuh")]
    public float rotationSpeed = 10f;

    // ─── Private State ────────────────────────────────────────────────────────
    private Transform[] path;
    private int targetWaypointIndex = 0;
    private bool isMoving = false;

    // ─── Inisialisasi jalur dari WaveSpawner ─────────────────────────────────
    /// <summary>Dipanggil oleh WaveSpawner setelah musuh di-spawn</summary>
    public void InitPath(Transform[] waypoints)
    {
        path = waypoints;
        targetWaypointIndex = 0;
        isMoving = (path != null && path.Length > 0);
    }

    // ─────────────────────────────────────────────────────────────────────────

    private void Update()
    {
        if (!isMoving || path == null || path.Length == 0) return;
        if (targetWaypointIndex >= path.Length) return;

        MoveTowardsWaypoint();
    }

    // ─── Gerak ke waypoint saat ini ──────────────────────────────────────────
    private void MoveTowardsWaypoint()
    {
        Transform target = path[targetWaypointIndex];
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;

        // Gerak
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Rotasi
        if (rotateTowardsTarget && direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                Time.deltaTime * rotationSpeed
            );
        }

        // Cek apakah sudah cukup dekat dengan waypoint
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= waypointReachThreshold)
        {
            ReachWaypoint();
        }
    }

    // ─── Dipanggil ketika waypoint tercapai ──────────────────────────────────
    private void ReachWaypoint()
    {
        targetWaypointIndex++;

        // Jika ini waypoint terakhir (Main Tower)
        if (targetWaypointIndex >= path.Length)
        {
            ReachMainTower();
        }
    }

    // ─── Musuh mencapai Main Tower ────────────────────────────────────────────
    private void ReachMainTower()
    {
        isMoving = false;

        // Kurangi nyawa Main Tower
        MainTower tower = FindObjectOfType<MainTower>();
        if (tower != null)
            tower.TakeDamage(1);

        Debug.Log("[EnemyMovement] Musuh mencapai Main Tower!");
        Destroy(gameObject);
    }

    // ─── Getter publik ────────────────────────────────────────────────────────
    public int GetCurrentWaypointIndex() => targetWaypointIndex;
    public float GetSpeed() => speed;
}
