using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Play button clicked!");
    }

    public void OpenStageSelect()
    {
        Debug.Log("Stage Select button clicked!");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button clicked!");
    }

    public void ExitGame()
    {
        Debug.Log("SAYA AKAN LAWAN!");
        Application.Quit();
    }
}