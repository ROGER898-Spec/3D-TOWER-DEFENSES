using UnityEngine;

/// <summary>
/// TowerBlueprint - ScriptableObject data untuk setiap jenis tower.
///
/// CARA MEMBUAT:
/// Klik kanan di Project → Create → TowerDefense → Tower Blueprint
/// Isi data prefab, harga, upgrade di Inspector.
///
/// CATATAN UPGRADE:
/// Versi ini TIDAK lagi pakai "upgradedVersion" (rantai blueprint terpisah).
/// Upgrade sekarang disimpan dalam 1 blueprint yang sama lewat array "levels",
/// sesuai format tabel balancing (1 tower = 10 level dalam 1 baris data).
/// </summary>
[CreateAssetMenu(fileName = "NewTowerBlueprint", menuName = "TowerDefense/Tower Blueprint")]
public class TowerBlueprint : ScriptableObject
{
    [Header("Identity")]
    public string towerId;              // contoh: TWR-01
    public string towerName = "Basic Tower";
    public ElementType element;         // Fire, Water, Wind, Earth
    [TextArea] public string description = "Tower serangan dasar.";
    public Sprite icon;

    [Header("Prefab")]
    [Tooltip("Prefab 3D tower yang akan di-spawn")]
    public GameObject towerPrefab;

    [Header("Base Stats (Level 1)")]
    public float baseDamage;
    [Tooltip("Serangan per detik. INI SUDAH DIKONVERSI, bukan 'detik/serang' dari tabel PDF.")]
    public float baseFireRate;
    public float baseRange;

    [Header("Economy")]
    [Tooltip("Biaya membangun tower (dipakai BuildManager.cs, jangan diganti nama field ini)")]
    public int cost = 100;

    [Header("Level Progression")]
    [Tooltip("Index 0 = Level 2, index 1 = Level 3, dst. Level 1 pakai Base Stats di atas.")]
    public LevelData[] levels;

    // ─── Helper: hitung stat aktual di level tertentu ──────────────────────
    public float GetDamageAtLevel(int level)
    {
        if (level <= 1 || levels == null || levels.Length == 0) return baseDamage;
        int idx = Mathf.Clamp(level - 2, 0, levels.Length - 1);
        return baseDamage * levels[idx].damageMultiplier;
    }

    public float GetRangeAtLevel(int level)
    {
        if (level <= 1 || levels == null || levels.Length == 0) return baseRange;
        int idx = Mathf.Clamp(level - 2, 0, levels.Length - 1);
        return baseRange * levels[idx].rangeMultiplier;
    }

    public int GetUpgradeCostToLevel(int level)
    {
        if (level <= 1 || levels == null || levels.Length == 0) return 0;
        int idx = Mathf.Clamp(level - 2, 0, levels.Length - 1);
        return levels[idx].upgradeCost;
    }

    public bool IsMaxLevel(int level) => levels == null || level >= levels.Length + 1;
}

[System.Serializable]
public class LevelData
{
    public int level;
    public int upgradeCost;
    public float damageMultiplier = 1f;
    public float rangeMultiplier = 1f;
}

public enum ElementType { Fire, Water, Wind, Earth }
