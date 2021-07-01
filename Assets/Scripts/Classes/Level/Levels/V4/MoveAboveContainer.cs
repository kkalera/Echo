using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAboveContainer : CraneLevel    
{

    [SerializeField] private Transform targetContainer;
    [SerializeField] private MoveAboveContainerSettings settings;
    [SerializeField] private Transform environment;
    private ICrane crane;
    

    public override Vector3 TargetLocation => targetContainer.position - environment.position;

    private bool TargetReached()
    {
        Debug.Log(Mathf.Abs(crane.SpreaderPosition.z - TargetLocation.z));
        return Mathf.Abs(crane.SpreaderPosition.z - TargetLocation.z) < settings.distanceToTarget;
    }

    public override void OnEpisodeBegin()
    {
        crane.ResetToPosition(new Vector3(0, 25, Random.Range(-25, 40)));
        targetContainer.localPosition = new Vector3(0, 21.5f, Random.Range(-25, 40));
    }

    public override void ResetEnvironment(ICrane crane)
    {
        crane.CraneMovementDisabled = true;
        crane.WinchMovementDisabled = true;
        crane.SwingDisabled = true;

        crane.CabinMovementDisabled = false;
        this.crane = crane;
    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        RewardData rd = new RewardData();
        rd.reward = -1f / 5000;

        if (TargetReached())
        {
            rd.endEpisode = true;
            if (settings.MaxDificulty) rd.reward = 1f;
            if (!settings.MaxDificulty) { rd.reward = .8f; settings.IncreaseDifficulty(); }
        }

        if (ProcessCollision(col, other))
        {
            rd.reward = -1f;
            rd.endEpisode = true;
        }    

        return rd;
    }

    private bool ProcessCollision(Collision col = null, Collider other = null)
    {
        if (col != null)
        {
            return col.collider.CompareTag("dead");
        }

        if (other != null)
        {
            return other.CompareTag("dead");
        }

        return false;
    }
}
