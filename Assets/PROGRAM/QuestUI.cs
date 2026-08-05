using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private TMP_Text questText;
    [SerializeField] private TMP_Text progressText;
    private int killedEnemies;
    private int totalEnemies;
    private bool questActive;

    private void OnEnable()
    {
        WaveSpawner.OnWaveStart += ShowQuest;
        WaveSpawner.OnWaveComplete += CompleteQuest;
        EnemyHealth.OnEnemyKilled += CountEnemyKilled;
    }

    private void OnDisable()
    {
        WaveSpawner.OnWaveStart -= ShowQuest;
        WaveSpawner.OnWaveComplete -= CompleteQuest;
        EnemyHealth.OnEnemyKilled -= CountEnemyKilled;
    }

    private void ShowQuest(int waveIndex)
    {
        int waveNumber = waveIndex + 1;
        int stageNumber = waveSpawner.currentStage;

        killedEnemies = 0;
        totalEnemies = waveSpawner.waves[waveIndex].count;
        questActive = true;

        questText.text = GetQuest(stageNumber, waveNumber);

        UpdateProgressText();
    }

    private void CompleteQuest(int waveIndex)
    {
        questActive = false;

        int waveNumber = waveIndex + 1;

        questText.text = $"Wave {waveNumber} selesai!";
    }

    private void CountEnemyKilled(int reward)
    {
        if (!questActive)
            return;

        killedEnemies++;

        killedEnemies = Mathf.Clamp(
            killedEnemies,
            0,
            totalEnemies
        );

        UpdateProgressText();
    }

    private void UpdateProgressText()
    {
        if (progressText != null)
        {
            progressText.text = $"{killedEnemies}/{totalEnemies}";
        }
    }

    private string GetQuest(int stage, int wave)
    {
        if (stage == 1)
        {
            switch (wave)
            {
                case 1:
                    return "Quest: Kalahkan semua musuh pada Wave 1";

                case 2:
                    return "Quest: Kalahkan semua musuh pada Wave 2";

                case 3:
                    return "Quest: Kalahkan boss";
            }
        }

        if (stage == 2)
        {
            switch (wave)
            {
                case 1:
                    return "Quest: Kalahkan semua musuh pada Wave 1";

                case 2:
                    return "Quest: Kalahkan semua musuh pada Wave 2";

                case 3:
                    return "Quest: Kalahkan boss";
            }
        }

        return $"Quest Stage {stage} - Wave {wave}";
    }
}