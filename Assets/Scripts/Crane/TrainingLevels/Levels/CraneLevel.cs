using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraneLevel : MonoBehaviour
{
    public abstract ICrane SetCraneRestrictions(ICrane crane);
    public abstract void InitializeEnvironment(Transform environment);
    public abstract void ResetEnvironment(Transform environment, ICrane crane);
    public abstract void ClearEnvironment(Transform environment);
    public abstract RewardData GetReward(ICrane crane);
    public abstract Vector3 TargetLocation { get; }
    public abstract void IncreaseDifficulty();
}
