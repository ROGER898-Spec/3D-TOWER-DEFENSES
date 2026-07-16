using UnityEngine;

/// <summary>
/// EnemyHealth - Mengelola HP musuh dan kematiannya.
/// Mengambil data dari EnemyData ScriptableObject.
/// Letakkan pada prefab Enemy bersama EnemyMovement.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyData enemyData;

    [Header("Effects")]
    public GameObject deathEffectPrefab;

    private float currentHealth;
    private float baseHealth;
    private bool isDead = false;

    public static event System.Action<int> OnEnemyKilled;


    private void Awake()
    {
        if (enemyData != null)
        {
            baseHealth = enemyData.maxHealth;
            currentHealth = baseHealth;
        }
        else
        {
            Debug.LogWarning($"[EnemyHealth] EnemyData belum diisi pada {gameObject.name}");
        }
    }


    /// <summary>
    /// Terapkan scaling HP sesuai stage.
    /// Dipanggil oleh WaveSpawner setelah enemy spawn.
    /// </summary>
    public void ApplyStageScaling(int stage)
    {
        if (enemyData == null) return;

        float multiplier = 1f + (stage * 0.15f);

        currentHealth = baseHealth * multiplier;

        Debug.Log(
            $"[EnemyHealth] {gameObject.name} Stage {stage} HP: {currentHealth}"
        );
    }


    public void TakeDamage(float amount)
    {
        ApplyDamage(amount);
    }


    public void TakeDamage(float amount, ElementType attackerElement)
    {
        if (enemyData == null)
        {
            ApplyDamage(amount);
            return;
        }

        float multiplier = ElementSystem.GetMultiplier(
            attackerElement,
            enemyData.element
        );

        float finalDamage = amount * multiplier;


        if (multiplier > 1f)
        {
            Debug.Log(
                $"[EnemyHealth] {gameObject.name} kena UNGGUL! {amount} -> {finalDamage}"
            );
        }
        else if (multiplier < 1f)
        {
            Debug.Log(
                $"[EnemyHealth] {gameObject.name} kena LEMAH! {amount} -> {finalDamage}"
            );
        }


        ApplyDamage(finalDamage);
    }


    private void ApplyDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
            Die();
    }


    public float GetHealthPercent()
    {
        if (enemyData == null) return 0;

        return currentHealth / (baseHealth);
    }


    public float GetCurrentHealth()
    {
        return currentHealth;
    }


    public float GetMaxHealth()
    {
        return baseHealth;
    }


    private void Die()
    {
        if (isDead) return;

        isDead = true;


        if (deathEffectPrefab != null)
        {
            Instantiate(
                deathEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }


        if (enemyData != null)
        {
            OnEnemyKilled?.Invoke(enemyData.rewardOnDeath);

            Debug.Log(
                $"[EnemyHealth] {gameObject.name} mati! Reward: {enemyData.rewardOnDeath}"
            );
        }


        Destroy(gameObject);
    }
}