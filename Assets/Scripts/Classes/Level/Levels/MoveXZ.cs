using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

/// <summary>
/// Class to teach the AI to control the swing of the crane
/// Every time the environment is set, it is set to either the back of the crane, or the front randomly.
/// The goal for the AI is to move to the other side of the crane, keeping its swing below a set value.
/// </summary>
public class MoveXZ : CraneLevel
{
    private readonly float maxstep = 5000;
    private Vector3 target;
    private ICrane crane;
    private float yHeight = 25;
    private float range = 5;

    public override Vector3 TargetLocation => target;

    public override void OnEpisodeBegin()
    {
        crane.ResetToPosition(new Vector3(0, 25, Random.Range(-15, 40)));
        target = new Vector3(0, Random.Range(yHeight, 25), Random.Range(-15, 40));
        yHeight = Mathf.Max(yHeight - 0.1f, 10);
        range = Mathf.Max(range - 0.1f, 0.5f);
        crane.SwingDisabled = true;
        crane.CabinMovementDisabled = false;
        crane.WinchMovementDisabled = false;
    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        // Create the base reward data
        RewardData rd = new RewardData();

        // Calculate the distance to the target and give a reward when it's at the location. Also end the episode
        float targetDistance = Vector3.Distance(target, crane.SpreaderPosition);

        if (targetDistance < range)
        {
            if (yHeight == 10f && range == 0.5f)
            {
                rd.reward += 1;
            }
            else
            {
                rd.reward += 0.8f;
            }
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
