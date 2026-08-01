using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class TutorialStep
{
    [TextArea(3, 6)]
    public string information;

    public RectTransform tutorialBoxTarget;
}
public class TutorialManager : MonoBehaviour
{
    public TMP_Text informationText;

    public RectTransform tutorialBox;

    public TutorialStep[] steps;

    public GameObject nextButton;

    public GameObject finishButton;

    public GameObject skipButton;

    public GameObject tutorialPanel;

    private int currentStep = 0;

    private void Start()
    {
        if (!TutorialModeManager.isTutorialMode)
        {
            gameObject.SetActive(false);
            return;
        }

        Time.timeScale = 0f;

        finishButton.SetActive(false);

        UpdateStep();
    }

    public void NextStep()
    {
        if (currentStep < steps.Length - 1)
        {
            currentStep++;
            UpdateStep();
        }
    }

    public void PreviousStep()
    {
        if (currentStep > 0)
        {
            currentStep--;
            UpdateStep();
        }
    }

    public void SkipTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    public void SetNextButton(bool state)
    {
        nextButton.SetActive(state);
    }

    public void FinishTutorial()
    {
        
        TutorialModeManager.isTutorialMode = false;

        SceneManager.LoadScene("MainMenuScene");

        Time.timeScale = 1f;
    }
    private void UpdateStep()
    {
        informationText.text =
            steps[currentStep].information;

        tutorialBox.position =
            steps[currentStep].tutorialBoxTarget.position;

        if (currentStep == steps.Length - 1)
        {
            nextButton.SetActive(false);
            finishButton.SetActive(true);
            skipButton.SetActive(false);
        }
        else
        {
            nextButton.SetActive(true);
            finishButton.SetActive(false);
            skipButton.SetActive(true);
        }
    }
}