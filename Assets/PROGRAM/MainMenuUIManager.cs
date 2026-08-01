using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Nama Scene Battle")]
    [Tooltip("Isi nama scene battle tanpa ekstensi .unity")]
    public string battleSceneName = "SampleScene";

    [Header("Panel Main Menu")]
    [Tooltip("Drag HowToPlayPanel dari Canvas")]
    public GameObject howToPlayPanel;

    [Tooltip("Drag object MainMenu yang berisi tombol Play, Setting, How, dan Quit")]
    public GameObject mainMenuButtons;

    [Tooltip("Drag instance prefab SettingsPanel dari Hierarchy")]
    public GameObject settingsPanel;

     private void Start()
    {
        AudioManager.Instance.PlayMainMenuMusic();
    }
    
    /// Membuka Battle Scene dengan tampilan awal HUD.
    public void OnPlayButtonClicked()
    {
        TutorialModeManager.isTutorialMode = false;
        // Menyimpan permintaan bahwa HUD harus menjadi tampilan awal.
        BattleSceneRequest.SetRequestedPanel(
            BattleEntryPanel.HUD
        );

        // Memuat Battle Scene melalui fungsi bersama.
        LoadBattleScene();
    }

    /// Membuka Battle Scene dan meminta UpgradePanel ditampilkan.
    public void OnUpgradeButtonClicked()
    {
        BattleSceneRequest.SetRequestedPanel(
            BattleEntryPanel.Upgrade
        );

        LoadBattleScene();
    }

    /// Membuka Battle Scene dan meminta InventoryPanel ditampilkan.
    public void OnInventoryButtonClicked()
    {
        BattleSceneRequest.SetRequestedPanel(
            BattleEntryPanel.Inventory
        );

        LoadBattleScene();
    }

    
    public void OnSettingButtonClicked()
    {
        mainMenuButtons.SetActive(false);
        settingsPanel.SetActive(true);
    }

   public void OnCloseSettingButtonClicked()
    {
        settingsPanel.SetActive(false);
        mainMenuButtons.SetActive(true);
    }

    /// Membuka panel How To Play tanpa berpindah scene.
    public void OnHowToPlayButtonClicked()
    {
        TutorialModeManager.isTutorialMode = true;

        BattleSceneRequest.SetRequestedPanel(
            BattleEntryPanel.HUD
        );

        LoadBattleScene();
    }

    /// Menutup panel How To Play.
    public void OnCloseHowToPlayButtonClicked()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }
    
    private void LoadBattleScene()
    {
        if (string.IsNullOrWhiteSpace(battleSceneName))
        {
            Debug.LogError(
                "[MainMenuUIManager] Battle Scene Name belum diisi."
            );

            return;
        }

        Debug.Log(
            $"[MainMenuUIManager] Membuka scene: {battleSceneName}"
        );

        Time.timeScale = 1f;

        SceneManager.LoadScene(battleSceneName);
    }
}