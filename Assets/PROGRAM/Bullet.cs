using UnityEngine;

/// <summary>
/// Bullet - Proyektil yang dikeluarkan tower, mengikuti target (homing).
/// Letakkan pada prefab proyektil (bullet, rocket, arrow, dll).
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f;

    [Tooltip("Destroy proyektil jika target hilang setelah detik ini")]
    public float lifetimeIfTargetLost = 3f;

    [Header("Hit Effect")]
    [Tooltip("(Opsional) Efek partikel saat mengenai target")]
    public GameObject hitEffectPrefab;

    // ─── Data dari Tower (diset via kode) ────────────────────────────────────
    [HideInInspector] public float       damage       = 25f;
    [HideInInspector] public ElementType element      = ElementType.Fire;
    [HideInInspector] public bool        splash       = false;
    [HideInInspector] public float       splashRadius = 1.5f;
    [HideInInspector] public bool        slow         = false;
    [HideInInspector] public float       slowAmount   = 0.3f;
    [HideInInspector] public float       slowDuration = 2f;

    private Transform target;
    private float lifetimeTimer;

    public void SetTarget(Transform t)
    {
        target = t;
        lifetimeTimer = lifetimeIfTargetLost;
    }

    private void Update()
    {
        if (target == null)
        {
            lifetimeTimer -= Time.deltaTime;
            if (lifetimeTimer <= 0f)
                Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distThisFrame, Space.World);
        transform.LookAt(target);
    }

    private void HitTarget()
    {
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        if (splash)
            SplashDamage(target.position);
        else
            DamageEnemy(target);

        Destroy(gameObject);
    }

    private void DamageEnemy(Transform enemy)
    {
        EnemyHealth eh = enemy.GetComponent<EnemyHealth>();
        if (eh != null)
            eh.TakeDamage(damage, element);

        if (slow)
            ApplySlow(enemy);
    }

    private void SplashDamage(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, splashRadius);
        foreach (Collider c in hits)
        {
            if (c.CompareTag("Enemy"))
            {
                EnemyHealth eh = c.GetComponent<EnemyHealth>();
                if (eh != null)
                    eh.TakeDamage(damage, element);

                if (slow)
                    ApplySlow(c.transform);
            }
        }
    }

    private void ApplySlow(Transform enemy)
    {
        EnemyMovement em = enemy.GetComponent<EnemyMovement>();
        if (em != null)
            StartCoroutine(SlowCoroutine(em));
    }

    private System.Collections.IEnumerator SlowCoroutine(EnemyMovement em)
    {
        if (em == null) yield break;

        float originalSpeed = em.GetSpeed();
        em.speed = originalSpeed * (1f - slowAmount);

        yield return new WaitForSeconds(slowDuration);

        if (em != null)
            em.speed = originalSpeed;
    }

    private void OnDrawGizmos()
    {
        if (!splash) return;
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}
