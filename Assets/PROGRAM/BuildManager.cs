using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Tower Blueprints")]
    public TowerBlueprint[] towerBlueprints;

    [Header("Node Highlight")]
    public Color canBuildColor    = new Color(0f, 1f, 0f, 0.4f);
    public Color cannotBuildColor = new Color(1f, 0f, 0f, 0.4f);

    [Header("Currency")]
    public int startingMoney = 200;

    private TowerBlueprint selectedBlueprint = null;
    private BuildNode selectedNode = null;
    private int currentMoney;

    public static event System.Action<int> OnMoneyChanged;
    public static event System.Action<TowerBlueprint> OnTowerSelected;
    public static event System.Action OnBuildCancelled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        currentMoney = startingMoney;
        OnMoneyChanged?.Invoke(currentMoney);
        EnemyHealth.OnEnemyKilled += AddMoney;
    }

    private void OnDestroy()
    {
        EnemyHealth.OnEnemyKilled -= AddMoney;
    }

    private void Update()
    {
        if (selectedBlueprint != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                CancelBuild();
        }
    }

    public void SelectTowerToBuild(int blueprintIndex)
    {
        if (blueprintIndex < 0 || blueprintIndex >= towerBlueprints.Length)
        {
            Debug.LogWarning("[BuildManager] Blueprint index tidak valid!");
            return;
        }

        selectedBlueprint = towerBlueprints[blueprintIndex];
        selectedNode = null;

        Debug.Log($"[BuildManager] Tower dipilih: {selectedBlueprint.towerName} (Biaya: {selectedBlueprint.cost})");
        OnTowerSelected?.Invoke(selectedBlueprint);
    }

    public void SelectTowerToBuild(TowerBlueprint blueprint)
    {
        selectedBlueprint = blueprint;
        selectedNode = null;
        OnTowerSelected?.Invoke(selectedBlueprint);
    }

    public void CancelBuild()
    {
        selectedBlueprint = null;
        selectedNode = null;
        OnBuildCancelled?.Invoke();
        Debug.Log("[BuildManager] Build dibatalkan.");
    }

    public bool BuildTowerOn(BuildNode node)
    {
        if (selectedBlueprint == null)
        {
            Debug.Log("[BuildManager] Tidak ada tower yang dipilih.");
            return false;
        }

        if (node.HasTower())
        {
            Debug.Log("[BuildManager] Node sudah ada tower!");
            return false;
        }

        if (!HasEnoughMoney(selectedBlueprint.cost))
        {
            Debug.Log($"[BuildManager] Uang tidak cukup! Butuh: {selectedBlueprint.cost}, Punya: {currentMoney}");
            return false;
        }

        SpendMoney(selectedBlueprint.cost);

        GameObject towerGO = Instantiate(
            selectedBlueprint.towerPrefab,
            node.GetBuildPosition(),
            Quaternion.identity
        );

        // ★ BARU: isi stat tower sesuai blueprint yang dipilih (damage/range/fireRate/element)
        Tower towerScript = towerGO.GetComponent<Tower>();
        if (towerScript != null)
            towerScript.Init(selectedBlueprint);

        node.SetTower(towerGO);

        Debug.Log($"[BuildManager] Tower '{selectedBlueprint.towerName}' berhasil dibangun!");
        return true;
    }

    public void SellTowerOn(BuildNode node)
    {
        if (!node.HasTower()) return;

        TowerBlueprint bp = node.GetTowerBlueprint();
        int sellPrice = bp != null ? Mathf.RoundToInt(bp.cost * 0.5f) : 0;

        node.RemoveTower();
        AddMoney(sellPrice);

        Debug.Log($"[BuildManager] Tower dijual seharga {sellPrice}.");
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public void SpendMoney(int amount)
    {
        currentMoney -= amount;
        currentMoney = Mathf.Max(0, currentMoney);
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public bool HasEnoughMoney(int amount) => currentMoney >= amount;
    public int GetMoney() => currentMoney;

    public TowerBlueprint GetSelectedBlueprint() => selectedBlueprint;
    public bool IsBuildModeActive() => selectedBlueprint != null;
}
