using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class WinchMovementSettings : MonoBehaviour
{

    [SerializeField] [Range(0.01f,4f)]public float velocityTarget = 4f;
    [SerializeField] [Range(0.01f, 4f)] public float finalVelocityTarget = 0.05f;
    [SerializeField] [Range(0, 25)] public float spreaderHeight = 25f;
    [SerializeField] [Range(0, 25)] public float finalSpreaderHeight = 17;
    [SerializeField] [Range(0.5f, 10)] public float goalRadius = 10f;
    [SerializeField] [Range(0.5f, 5)] public float finalGoalRadius = 1f;
    [SerializeField] public float discount = 0.1f;

    private float currentHeight = -1f;

    public bool FinalDifficulty => Mathf.Approximately(spreaderHeight, finalSpreaderHeight) 
        && Mathf.Approximately(velocityTarget, finalVelocityTarget) 
        && Mathf.Approximately(goalRadius, finalGoalRadius);

    public void IncreaseDifficulty()
    {
        goalRadius = Mathf.Max(goalRadius - discount,finalGoalRadius);
    }

    public void OnEpisodeBegin()
    {
        spreaderHeight = Academy.Instance.EnvironmentParameters.GetWithDefault("spreaderHeight", 17);
        if(Mathf.Approximately(currentHeight, -1f) || !Mathf.Approximately(currentHeight, spreaderHeight))
        { 
            currentHeight = spreaderHeight;
            goalRadius = 10f;
        }
    }
}
