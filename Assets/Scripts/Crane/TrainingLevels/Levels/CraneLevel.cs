using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraneLevel : MonoBehaviour
{
    public abstract void SetCraneRestrictions();
    public abstract void InitializeEnvironment(Transform environment, ICrane crane);
    public abstract void ResetEnvironment();
    public abstract void ClearEnvironment();
    public abstract RewardData GetReward();
    public abstract Vector3 TargetLocation { get; }
    public abstract ICrane Crane { get; set; }
    public abstract void IncreaseDifficulty();    
}
