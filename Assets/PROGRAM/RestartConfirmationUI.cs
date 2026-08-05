using UnityEngine;

public class RestartConfirmationUI : MonoBehaviour
{
    /// <summary>
    /// Membuka panel konfirmasi restart.
    /// </summary>
    public void OpenPanel()
    {
        transform.SetAsLastSibling();
        gameObject.SetActive(true);

        Debug.Log("[RestartConfirmationUI] Panel restart dibuka.");
    }

    /// <summary>
    /// Membatalkan restart dan kembali ke PausePanel.
    /// </summary>
    public void CancelRestart()
    {
        gameObject.SetActive(false);

        Debug.Log("[RestartConfirmationUI] Restart dibatalkan.");
    }

    /// <summary>
    /// Mengulang Battle Scene dari awal.
    /// </summary>
    public void ConfirmRestart()
    {
        Debug.Log("[RestartConfirmationUI] Restart dikonfirmasi.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
        else
        {
            Debug.LogError(
                "[RestartConfirmationUI] GameManager tidak ditemukan."
            );
        }
    }
}