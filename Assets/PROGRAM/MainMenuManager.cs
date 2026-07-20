using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Nama Scene Battle")]
    [Tooltip("Isi nama scene battle tanpa ekstensi .unity")]
    public string battleSceneName = "SampleScene";

    /// <summary>
    /// Dipanggil oleh PlayButton untuk membuka Battle Scene.
    /// </summary>
    public void OnPlayButtonClicked()
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

        // Menghindari game tetap berhenti apabila sebelumnya keluar dari Pause.
        Time.timeScale = 1f;

        SceneManager.LoadScene(battleSceneName);
    }
}