using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menampilkan informasi tower yang dipilih pada HUD.
/// Untuk sementara menggunakan warna dan huruf sebagai ikon dummy.
/// </summary>
public class SelectedObjectUI : MonoBehaviour
{
    [Header("Status Panel")]
    [SerializeField] private GameObject emptyState;
    [SerializeField] private GameObject towerDetails;

    [Header("Referensi UI")]
    [SerializeField] private Image towerIconDummy;
    [SerializeField] private TMP_Text towerInitialText;
    [SerializeField] private TMP_Text towerNameText;
    [SerializeField] private TMP_Text towerInfoText;

    [Header("Warna Dummy Tower")]
    [SerializeField] private Color fireColor = Color.red;
    [SerializeField] private Color waterColor = Color.blue;
    [SerializeField] private Color windColor = Color.cyan;
    [SerializeField] private Color earthColor =
        new Color(0.45f, 0.25f, 0.1f);

    private void Start()
    {
        ClearSelection();
    }

    /// <summary>
    /// Dipanggil oleh tombol Fire.
    /// </summary>
    public void ShowFireTower()
    {
        ShowTowerInformation(
            fireColor,
            "F",
            "Fire Tower",
            "Damage: Tinggi\n" +
            "Range: Sedang\n" +
            "Kecepatan: Sedang"
        );
    }

    /// <summary>
    /// Dipanggil oleh tombol Water.
    /// </summary>
    public void ShowWaterTower()
    {
        ShowTowerInformation(
            waterColor,
            "W",
            "Water Tower",
            "Damage: Sedang\n" +
            "Efek: Memperlambat\n" +
            "Range: Sedang"
        );
    }

    /// <summary>
    /// Dipanggil oleh tombol Wind.
    /// </summary>
    public void ShowWindTower()
    {
        ShowTowerInformation(
            windColor,
            "A",
            "Wind Tower",
            "Damage: Rendah\n" +
            "Kecepatan: Tinggi\n" +
            "Range: Panjang"
        );
    }

    /// <summary>
    /// Dipanggil oleh tombol Earth.
    /// </summary>
    public void ShowEarthTower()
    {
        ShowTowerInformation(
            earthColor,
            "E",
            "Earth Tower",
            "Damage: Sangat Tinggi\n" +
            "Kecepatan: Rendah\n" +
            "Pertahanan: Tinggi"
        );
    }

    /// <summary>
    /// Menampilkan informasi tower pada panel.
    /// </summary>
    private void ShowTowerInformation(
        Color iconColor,
        string towerInitial,
        string towerName,
        string towerInformation
    )
    {
        if (emptyState != null)
        {
            emptyState.SetActive(false);
        }

        if (towerDetails != null)
        {
            towerDetails.SetActive(true);
        }

        if (towerIconDummy != null)
        {
            towerIconDummy.color = iconColor;
        }

        if (towerInitialText != null)
        {
            towerInitialText.text = towerInitial;
        }

        if (towerNameText != null)
        {
            towerNameText.text = towerName;
        }

        if (towerInfoText != null)
        {
            towerInfoText.text = towerInformation;
        }
    }

    /// <summary>
    /// Mengembalikan panel ke kondisi belum memilih tower.
    /// </summary>
    public void ClearSelection()
    {
        if (emptyState != null)
        {
            emptyState.SetActive(true);
        }

        if (towerDetails != null)
        {
            towerDetails.SetActive(false);
        }
    }
}