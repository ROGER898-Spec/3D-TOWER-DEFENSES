using UnityEngine;

/// <summary>
/// BuildNode - Titik di mana player bisa membangun tower.
/// 
/// CARA SETUP DI UNITY:
/// 1. Buat beberapa GameObject di scene sebagai titik build (misalnya platform/pad kecil).
/// 2. Attach script ini pada setiap titik.
/// 3. Tambahkan Collider (MeshCollider atau BoxCollider) agar bisa di-raycast.
/// 4. Tag bisa "BuildNode" untuk filter raycast.
/// </summary>
public class BuildNode : MonoBehaviour
{
    // ─── Inspector Fields ──────────────────────────────────────────────────────
    [Header("Build Offset")]
    [Tooltip("Offset posisi tower dari node (agar tidak tenggelam di tanah)")]
    public Vector3 buildOffset = new Vector3(0f, 0.5f, 0f);

    [Header("Visual")]
    [Tooltip("Renderer node ini (untuk highlight warna)")]
    public Renderer nodeRenderer;

    // ─── State ────────────────────────────────────────────────────────────────
    private GameObject placedTower = null;
    private TowerBlueprint currentBlueprint = null;
    private Color originalColor;

    // ─────────────────────────────────────────────────────────────────────────

    private void Start()
    {
        if (nodeRenderer == null)
            nodeRenderer = GetComponent<Renderer>();

        if (nodeRenderer != null)
            originalColor = nodeRenderer.material.color;
    }

    // ─── Mouse Events ─────────────────────────────────────────────────────────
    private void OnMouseEnter()
    {
        if (!BuildManager.Instance.IsBuildModeActive()) return;

        if (nodeRenderer != null)
        {
            nodeRenderer.material.color = HasTower()
                ? BuildManager.Instance.cannotBuildColor
                : BuildManager.Instance.canBuildColor;
        }
    }

    private void OnMouseExit()
    {
        if (nodeRenderer != null)
            nodeRenderer.material.color = originalColor;
    }

    private void OnMouseDown()
    {
        if (BuildManager.Instance == null) return;

        if (!BuildManager.Instance.IsBuildModeActive())
        {
            // Jika tidak dalam build mode, mungkin buka panel upgrade/sell
            if (HasTower())
                OpenTowerPanel();
            return;
        }

        // Coba bangun tower
        bool success = BuildManager.Instance.BuildTowerOn(this);

        if (success)
        {
            currentBlueprint = BuildManager.Instance.GetSelectedBlueprint();
            // Reset warna
            if (nodeRenderer != null)
                nodeRenderer.material.color = originalColor;
        }
    }

    // ─── Panel upgrade/sell (placeholder) ────────────────────────────────────
    private void OpenTowerPanel()
    {
        // TODO: Tampilkan UI panel untuk upgrade atau jual tower
        Debug.Log($"[BuildNode] Membuka panel untuk tower di {gameObject.name}");
    }

    // ─── API Publik ───────────────────────────────────────────────────────────
    public Vector3 GetBuildPosition() => transform.position + buildOffset;
    public bool HasTower() => placedTower != null;
    public TowerBlueprint GetTowerBlueprint() => currentBlueprint;

    public void SetTower(GameObject tower)
    {
        placedTower = tower;
    }

    public void RemoveTower()
    {
        if (placedTower != null)
            Destroy(placedTower);
        placedTower = null;
        currentBlueprint = null;
    }

    // ─── Gizmo ───────────────────────────────────────────────────────────────
    private void OnDrawGizmos()
    {
        Gizmos.color = HasTower() ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position + buildOffset, Vector3.one * 0.4f);
    }
}
