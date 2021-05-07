using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelManager3 : MonoBehaviour
{
    [SerializeField] public List<CraneLevel> levels;

    public ICrane Crane { get; set; }
    public Vector3 TargetPosition { get => currentLevel.TargetLocation; }

    private CraneLevel currentLevel;

    public void SetLevel(int level)
    {
        if (level == -1) StopSimulation();

        for (int i = 0; i < levels.Count; i++)
        {
            if (i != level)
            {
                var l = levels[i];
                l.enabled = false;
            }
        }

        currentLevel = levels[level];

        currentLevel.enabled = true;
        currentLevel.ResetEnvironment(Crane);
    }

    public void OnEpisodeBegin()
    {
        if (Crane != null && currentLevel != null)
        {
            currentLevel.OnEpisodeBegin();
        }
        else
        {
            StopSimulation();
            throw new System.Exception("Crane or level not provided at episode beginning, stopping training");
        }
    }

    public RewardData Step(Collision col = null, Collider other = null)
    {
        if (currentLevel != null)
        {
            return currentLevel.Step(col, other);
        }
        else
        {
            StopSimulation();
            throw new System.Exception("Level is null when taking a step, stopping training");
        }
    }

    private void StopSimulation()
    {
        EditorApplication.isPlaying = false; //EditorApplication.ExecuteMenuItem("Edit/Play");
    }
}
