using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAboveContainerSettings : MonoBehaviour
{
    [SerializeField]public float distanceToTarget = 1f;
    [SerializeField]public float speedAboveTarget = 4f;

    public void IncreaseDifficulty()
    {
        speedAboveTarget = Mathf.Max(speedAboveTarget - 0.01f, 0.05f);    
    }

    public bool MaxDificulty { get => Mathf.Approximately(speedAboveTarget, 0.05f); }
}
