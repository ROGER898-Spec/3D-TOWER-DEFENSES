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

    [Header("Stun (Debug, live saat Play Mode)")]
    [Tooltip("Sedang kena stun (Stone Impact) atau tidak — read-only")]
    [SerializeField] private bool isStunned = false;
    [SerializeField] private float stunTimer = 0f;

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

    // ─── Stone Impact (Earth): hentikan gerak sementara ──────────────────────
    /// <summary>
    /// Terapkan stun. Kalau sedang stun dan kena stun lagi, ambil durasi yang
    /// lebih panjang (tidak menjumlahkan/stacking, cukup extend kalau perlu).
    /// </summary>
    public void ApplyStun(float duration)
    {
        if (isStunned)
        {
            stunTimer = Mathf.Max(stunTimer, duration);
            return;
        }

        isStunned = true;
        stunTimer = duration;
        Debug.Log($"[EnemyMovement] {gameObject.name} kena STUN selama {duration} detik!");
    }

    public bool IsStunned() => isStunned;

    // ─────────────────────────────────────────────────────────────────────────

    private void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
                Debug.Log($"[EnemyMovement] {gameObject.name} stun berakhir, lanjut jalan.");
            }
            return; // skip pergerakan selagi stun
        }

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

        MainTower tower = FindAnyObjectByType<MainTower>();

        if (tower != null)
        {
            EnemyHealth eh = GetComponent<EnemyHealth>();
            bool isBoss = eh != null && eh.isBoss;

            if (isBoss)
            {
                Debug.Log("[EnemyMovement] BOSS sampai ke Main Tower!");
                tower.DestroyInstantly();
            }
            else
            {
                tower.TakeDamage(1);
            }
        }

        Debug.Log("[EnemyMovement] Musuh mencapai Main Tower!");
        Destroy(gameObject);
    }

    // ─── Getter publik ────────────────────────────────────────────────────────
    public int GetCurrentWaypointIndex() => targetWaypointIndex;
    public float GetSpeed() => speed;
}