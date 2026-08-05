using UnityEngine;

public class ExitConfirmationUI : MonoBehaviour
{
    /// <summary>
    /// Membuka panel konfirmasi keluar.
    /// </summary>
    public void OpenPanel()
    {
        transform.SetAsLastSibling();
        gameObject.SetActive(true);

        Debug.Log("[ExitConfirmationUI] Panel exit dibuka.");
    }

    /// <summary>
    /// Menutup panel dan kembali ke tampilan sebelumnya.
    /// </summary>
    public void CancelExit()
    {
        gameObject.SetActive(false);

        Debug.Log("[ExitConfirmationUI] Exit dibatalkan.");
    }

    /// <summary>
    /// Menghentikan game.
    /// </summary>
    public void ConfirmExit()
    {
        Debug.Log("[ExitConfirmationUI] Keluar dari game.");

        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}