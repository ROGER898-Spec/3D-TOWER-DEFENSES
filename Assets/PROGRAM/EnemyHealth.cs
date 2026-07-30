using System.Collections;
using UnityEngine;

/// <summary>
/// EnemyHealth - Mengelola HP musuh dan kematiannya.
/// Letakkan pada prefab Enemy bersama EnemyMovement.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [Tooltip("HP dasar (Stage 1, belum di-scaling)")]
    public float maxHealth = 100f;

    [Header("Element")]
    public ElementType element = ElementType.Fire;

    [Header("Reward")]
    public int rewardOnDeath = 10;

    [Header("Effects")]
    public GameObject deathEffectPrefab;

    [Header("Debug (Live saat Play Mode)")]
    [Tooltip("HP saat ini — otomatis update real-time di Inspector selagi Play, tidak perlu diisi manual")]
    [SerializeField] private float currentHealth;
    [Tooltip("Persentase HP saat ini (0-1), buat cek cepat tanpa hitung manual")]
    [SerializeField] private float currentHealthPercent;

    private bool isDead = false;
    private Coroutine burnCoroutine;

    public static event System.Action<int> OnEnemyKilled;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    /// <summary>
    /// Terapkan scaling HP sesuai stage, formula dari data balancing:
    /// HP Stage = Base HP x (1 + (Stage x 0.15))
    /// DITAMBAH multiplier Difficulty (Easy/Normal/Hard) dari DifficultyManager.
    /// Dipanggil oleh WaveSpawner TEPAT SETELAH Instantiate, sebelum musuh mulai jalan.
    /// </summary>
    public void ApplyStageScaling(int stage)
    {
        float stageMultiplier = 1f + (stage * 0.15f);
        float difficultyMultiplier = DifficultyManager.Instance != null
            ? DifficultyManager.Instance.GetEnemyHealthMultiplier()
            : 1f; // fallback kalau DifficultyManager belum ada di scene (tidak error, cuma dianggap Normal)

        maxHealth *= stageMultiplier * difficultyMultiplier;
        currentHealth = maxHealth;
        UpdateHealthBar();

        Debug.Log($"[EnemyHealth] {gameObject.name} discale Stage {stage} (x{stageMultiplier}) x Difficulty (x{difficultyMultiplier}) -> HP: {maxHealth}");
    }

    public void TakeDamage(float amount)
    {
        ApplyDamage(amount);
    }

    public void TakeDamage(float amount, ElementType attackerElement)
    {
        float multiplier = ElementSystem.GetMultiplier(attackerElement, element);
        float finalDamage = amount * multiplier;

        if (multiplier > 1f)
            Debug.Log($"[EnemyHealth] {gameObject.name} kena UNGGUL! {amount} -> {finalDamage}");
        else if (multiplier < 1f)
            Debug.Log($"[EnemyHealth] {gameObject.name} kena LEMAH! {amount} -> {finalDamage}");

        ApplyDamage(finalDamage);
    }

    private void ApplyDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        UpdateHealthBar();

        if (currentHealth <= 0f)
            Die();
    }

    private void UpdateHealthBar()
    {
        currentHealthPercent = Mathf.Clamp01(GetHealthPercent());
    }

    /// <summary>
    /// Terapkan efek Burning Core (Fire): damage bertahap tiap 1 detik selama beberapa detik.
    /// Kalau kena tembak Fire lagi selagi masih terbakar, durasi di-reset (tidak stacking dobel).
    /// </summary>
    public void ApplyBurn(float damagePerSecond, float duration)
    {
        if (isDead) return;

        if (burnCoroutine != null)
            StopCoroutine(burnCoroutine);

        burnCoroutine = StartCoroutine(BurnRoutine(damagePerSecond, duration));
    }

    private IEnumerator BurnRoutine(float damagePerSecond, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration && !isDead)
        {
            yield return new WaitForSeconds(1f);
            elapsed += 1f;

            if (!isDead)
            {
                Debug.Log($"[EnemyHealth] {gameObject.name} terbakar! -{damagePerSecond} HP");
                ApplyDamage(damagePerSecond);
            }
        }

        burnCoroutine = null;
    }

    public float GetHealthPercent() => currentHealth / maxHealth;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        OnEnemyKilled?.Invoke(rewardOnDeath);

        Debug.Log($"[EnemyHealth] {gameObject.name} mati! Reward: {rewardOnDeath}");
        Destroy(gameObject);
    }
}