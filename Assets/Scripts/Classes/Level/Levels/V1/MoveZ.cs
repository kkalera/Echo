using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

/// <summary>
/// Class to teach the AI to control the swing of the crane
/// Every time the environment is set, it is set to either the back of the crane, or the front randomly.
/// The goal for the AI is to move to the other side of the crane, keeping its swing below a set value.
/// </summary>
public class MoveZ : CraneLevel
{
    //private readonly float maxstep = 5000;
    private Vector3 target;
    private ICrane crane;

    public override Vector3 TargetLocation => target;

    public override void OnEpisodeBegin()
    {
        crane.ResetToPosition(new Vector3(0, 25, Random.Range(-15, 40)));
        target = new Vector3(0, 25, Random.Range(-15, 40));

        crane.SwingDisabled = true;
        crane.WinchMovementDisabled = true;
        crane.CabinMovementDisabled = false;
    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        // Create the base reward data
        RewardData rd = new RewardData();

        // Calculate the distance to the target and give a reward when it's at the location. Also end the episode
        float targetDistance = Vector3.Distance(target, crane.SpreaderPosition);
        if (targetDistance < 0.5f)
        {
            rd.reward += 1;
            rd.endEpisode = true;
        }

        // Check wether the crane collided with an object
        // Its in here as an example, in this level the crane should not have collisions
        if (col != null || other != null)
        {
            string tag = "";
            if (col != null) tag = col.collider.tag;
            if (other != null) tag = other.tag;

            switch (tag)
            {
                case "target": rd.reward += 1; rd.endEpisode = true; break;
                case "dead": rd.reward += -1; rd.endEpisode = true; break;
                default: break;
            }
        }
        if (crane.SpreaderPosition.y >= 30)
        {
            rd.reward += -1;
            rd.endEpisode = true;
        }

        return rd;
    }

    public override void ResetEnvironment(ICrane crane)
    {
        this.crane = crane;
    }
}
