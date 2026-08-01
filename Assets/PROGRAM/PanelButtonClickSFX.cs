using UnityEngine;
using UnityEngine.UI;

public class PanelButtonClickSFX : MonoBehaviour
{
    [Header("Suara Klik")]
    [SerializeField] private AudioClip clickSFX;

    private Button[] buttons;

    private void Awake()
    {
        // Mengambil semua Button di dalam panel,
        // termasuk Button pada child yang sedang nonaktif.
        buttons = GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning(
                "[PanelButtonClickSFX] AudioManager tidak ditemukan."
            );

            return;
        }

        if (clickSFX == null)
        {
            Debug.LogWarning(
                "[PanelButtonClickSFX] Click SFX belum diisi."
            );

            return;
        }

        AudioManager.Instance.PlaySFX(clickSFX);
    }

    private void OnDestroy()
    {
        if (buttons == null)
            return;

        foreach (Button button in buttons)
        {
            if (button != null)
            {
                button.onClick.RemoveListener(PlayClickSound);
            }
        }
    }
}