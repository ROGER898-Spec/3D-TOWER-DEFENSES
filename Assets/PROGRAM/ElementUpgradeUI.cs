using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mengatur level upgrade satu elemen pada panel Power Up Element.
/// Untuk sementara hanya memperbarui level pada UI.
/// </summary>
public class ElementUpgradeUI : MonoBehaviour
{
    [Header("Identitas Elemen")]
    [SerializeField] private ElementType elementType;

    [Header("Referensi UI")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button upgradeButton;

    [Header("Level")]
    [SerializeField, Min(1)] private int currentLevel = 1;
    [SerializeField, Min(1)] private int maxLevel = 5;

    private void Start()
    {
        RefreshUI();
    }

    /// <summary>
    /// Dipanggil oleh tombol Upgrade.
    /// </summary>
    public void Upgrade()
    {
        if (currentLevel >= maxLevel)
            return;

        currentLevel++;

        RefreshUI();

        Debug.Log(
            $"[ElementUpgrade] {elementType} naik ke Level {currentLevel}"
        );
    }

    private void RefreshUI()
    {
        if (levelText != null)
        {
            levelText.text = $"Level {currentLevel}";
        }

        if (upgradeButton != null)
        {
            upgradeButton.interactable = currentLevel < maxLevel;
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}