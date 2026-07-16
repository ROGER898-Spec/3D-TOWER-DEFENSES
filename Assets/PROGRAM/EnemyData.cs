using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Tower Defense/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyId;
    public string enemyName;

    [Header("Stats")]
    public float maxHealth;
    public float moveSpeed;
    public int rewardOnDeath;

    [Header("Element")]
    public ElementType element;

    [Header("Prefab")]
    public GameObject enemyPrefab;

    [Header("Description")]
    [TextArea]
    public string description;
}