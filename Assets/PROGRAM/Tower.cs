using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tower - Sistem utama tower: deteksi musuh, targeting, dan serangan.
/// Letakkan script ini pada prefab tower.
///
/// PENTING: sejak Milestone 4, damage/range/fireRate/element di Inspector
/// hanya jadi nilai FALLBACK untuk testing manual. Kalau tower dibangun
/// lewat BuildManager, nilai-nilai ini akan DITIMPA otomatis oleh Init()
/// sesuai data dari TowerBlueprint yang dipilih.
/// </summary>
public class Tower : MonoBehaviour
{
    [Header("Attack Stats")]
    public float range = 5f;
    public float damage = 25f;
    public float fireRate = 1f;

    [Header("Element")]
    public ElementType element = ElementType.Fire;

    [Header("Targeting")]
    public TargetingMode targetingMode = TargetingMode.First;

    [Header("References")]
    public Transform turretHead;
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Special: Splash (opsional)")]
    public bool useSplashDamage = false;
    public float splashRadius = 1.5f;

    [Header("Special: Slow (opsional)")]
    public bool useSlowEffect = false;
    [Range(0f, 1f)] public float slowAmount = 0.3f;
    public float slowDuration = 2f;

    [Header("Gizmo")]
    public bool showRangeGizmo = true;

    public enum TargetingMode { First, Nearest, Strongest }

    private Transform currentTarget = null;
    private float fireCooldown = 0f;
    private int currentLevel = 1;
    private TowerBlueprint sourceBlueprint;

    // ─── Dipanggil BuildManager tepat setelah Instantiate ─────────────────────
    public void Init(TowerBlueprint blueprint)
    {
        sourceBlueprint = blueprint;
        currentLevel = 1;

        element  = blueprint.element;
        damage   = blueprint.GetDamageAtLevel(currentLevel);
        range    = blueprint.GetRangeAtLevel(currentLevel);
        fireRate = blueprint.baseFireRate;

        Debug.Log($"[Tower] {blueprint.towerName} ({element}) siap. Damage: {damage}, Range: {range}, FireRate: {fireRate}");
    }

    // ─── Upgrade level (dipanggil nanti dari UI upgrade) ──────────────────────
    public bool UpgradeLevel()
    {
        if (sourceBlueprint == null || sourceBlueprint.IsMaxLevel(currentLevel))
            return false;

        currentLevel++;
        damage = sourceBlueprint.GetDamageAtLevel(currentLevel);
        range  = sourceBlueprint.GetRangeAtLevel(currentLevel);
        return true;
    }

    public int GetCurrentLevel() => currentLevel;
    public int GetNextUpgradeCost() =>
        sourceBlueprint != null ? sourceBlueprint.GetUpgradeCostToLevel(currentLevel + 1) : 0;

    private void Update()
    {
        FindTarget();

        if (currentTarget == null) return;

        if (turretHead != null)
            RotateTurretToTarget();

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    private void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> enemiesInRange = new List<GameObject>();

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= range)
                enemiesInRange.Add(enemy);
        }

        if (enemiesInRange.Count == 0)
        {
            currentTarget = null;
            return;
        }

        currentTarget = SelectTarget(enemiesInRange).transform;
    }

    private GameObject SelectTarget(List<GameObject> candidates)
    {
        switch (targetingMode)
        {
            case TargetingMode.First: return GetFirstEnemy(candidates);
            case TargetingMode.Nearest: return GetNearestEnemy(candidates);
            case TargetingMode.Strongest: return GetStrongestEnemy(candidates);
            default: return candidates[0];
        }
    }

    private GameObject GetFirstEnemy(List<GameObject> candidates)
    {
        GameObject best = null;
        int highestIndex = -1;

        foreach (GameObject e in candidates)
        {
            EnemyMovement em = e.GetComponent<EnemyMovement>();
            if (em != null && em.GetCurrentWaypointIndex() > highestIndex)
            {
                highestIndex = em.GetCurrentWaypointIndex();
                best = e;
            }
        }
        return best ?? candidates[0];
    }

    private GameObject GetNearestEnemy(List<GameObject> candidates)
    {
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject e in candidates)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < minDist) { minDist = d; nearest = e; }
        }
        return nearest;
    }

    private GameObject GetStrongestEnemy(List<GameObject> candidates)
    {
        GameObject strongest = null;
        float maxHP = -1f;

        foreach (GameObject e in candidates)
        {
            EnemyHealth eh = e.GetComponent<EnemyHealth>();
            if (eh != null && eh.GetCurrentHealth() > maxHP)
            {
                maxHP = eh.GetCurrentHealth();
                strongest = e;
            }
        }
        return strongest ?? candidates[0];
    }

    private void RotateTurretToTarget()
    {
        Vector3 dir = currentTarget.position - turretHead.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        turretHead.rotation = Quaternion.Lerp(
            turretHead.rotation,
            Quaternion.Euler(0f, lookRot.eulerAngles.y, 0f),
            Time.deltaTime * 10f
        );
    }

    private void Shoot()
    {
        if (currentTarget == null) return;

        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletGO.GetComponent<Bullet>();

            if (bullet != null)
            {
                bullet.SetTarget(currentTarget);
                bullet.damage       = damage;
                bullet.element      = element;
                bullet.splash       = useSplashDamage;
                bullet.splashRadius = splashRadius;
                bullet.slow         = useSlowEffect;
                bullet.slowAmount   = slowAmount;
                bullet.slowDuration = slowDuration;
            }
        }
        else
        {
            InstantHit();
        }
    }

    private void InstantHit()
    {
        if (currentTarget == null) return;

        EnemyHealth eh = currentTarget.GetComponent<EnemyHealth>();
        if (eh != null)
            eh.TakeDamage(damage, element);

        if (useSplashDamage)
            ApplySplash(currentTarget.position);
    }

    public void ApplySplash(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, splashRadius);
        foreach (Collider c in hits)
        {
            if (c.CompareTag("Enemy"))
            {
                EnemyHealth eh = c.GetComponent<EnemyHealth>();
                if (eh != null)
                    eh.TakeDamage(damage * 0.5f, element);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showRangeGizmo) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);

        if (useSplashDamage)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, splashRadius);
        }
    }
}
