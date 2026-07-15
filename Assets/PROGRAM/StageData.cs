using UnityEngine;

/// <summary>
/// StageData - Data 1 stage: nomor stage (untuk HP scaling) + daftar wave di stage itu.
/// Ini BUKAN ScriptableObject, cuma data biasa yang diisi langsung di Inspector
/// StageManager (lewat array "Stages").
/// </summary>
[System.Serializable]
public class StageData
{
    [Tooltip("Nomor stage, dipakai untuk rumus HP scaling (1 + Stage x 0.15)")]
    public int stageNumber = 1;

    [Tooltip("Daftar wave yang akan di-spawn selama stage ini")]
    public WaveSpawner.Wave[] waves;
}
