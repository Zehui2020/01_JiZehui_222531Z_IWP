using UnityEngine;

public class WaveObjective : Objective
{
    private int waveCount = 0;
    private int maxWave;

    public WaveObjective(ObjectiveType objectiveType, string objectiveName, int maxWave) : base(objectiveType, objectiveName)
    {
        this.objectiveType = objectiveType;
        this.objectiveName = objectiveName;
        this.maxWave = maxWave;

        EnemySpawner.WaveEnded += WaveEnd;
    }

    private void WaveEnd()
    {
        waveCount++;
        objectiveUI.SetObjectiveName("Survive for " + maxWave + " waves (" + waveCount + "/" + maxWave + ")");

        if (waveCount >= maxWave)
        {
            CompleteObjective();
            ObjectiveManager.Instance.RemoveObjective(this);
            EnemySpawner.WaveEnded -= WaveEnd;
        }
    }

    ~WaveObjective()
    {
        EnemySpawner.WaveEnded -= WaveEnd;
    }
}
