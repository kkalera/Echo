using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraneLevel : MonoBehaviour
{
    public float Maxstep { get; set; }
    public abstract void ResetEnvironment(ICrane crane);
    public abstract void OnEpisodeBegin();
    public abstract RewardData Step(Collision col = null, Collider other = null);
    public abstract Vector3 TargetLocation { get; }
}
