using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

/// <summary>
/// Class to teach the AI to control the swing of the crane
/// Every time the environment is set, it is set to either the back of the crane, or the front randomly.
/// The goal for the AI is to move to the other side of the crane, keeping its swing below a set value.
/// </summary>
public class SwingControl : CraneLevel
{
    [SerializeField] [Range(0, 1)] float maximumSwing;
    private readonly float maxstep = 5000;
    private Vector3 target;
    private ICrane crane;
    private float maxSwingDistance;
    private float lastDistance;

    public override Vector3 TargetLocation => target;

    public override void OnEpisodeBegin()
    {
        bool frontSpawn = Random.value > 0.5;
        if (frontSpawn)
        {
            crane.ResetToPosition(new Vector3(0, 25, 35));
            target = new Vector3(0, 0, -10);
        }
        else
        {
            crane.ResetToPosition(new Vector3(0, 25, -10));
            target = new Vector3(0, 0, 35);
        }
        maxSwingDistance = (crane.CabinPosition.y - crane.SpreaderPosition.y) * maximumSwing;
        lastDistance = Vector3.Distance(new Vector3(0, 0, crane.SpreaderPosition.z), target);

    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        // Create the base reward data
        RewardData rd = new RewardData();

        // Calculate the amount of swing in the cable and give a reward accordingly
        float swingDistance = Vector3.Distance(new Vector3(0, 0, crane.CabinPosition.z), new Vector3(0, 0, crane.SpreaderPosition.z));
        Utils.ReportStat(swingDistance, "swing");

        if (crane.CabinVelocity.z > 0 && swingDistance <= maxSwingDistance)
        {
            float distanceNormalized = Mathf.Min(Utils.Normalize(swingDistance, 0, maxSwingDistance), 1);
            float invertedDistance = 1 - distanceNormalized;
            rd.reward += Mathf.Min(Mathf.Pow(invertedDistance, 2), 1) / maxstep;
        }

        // Calculate the distance to the target and give a reward when it's at the location. Also end the episode
        float targetDistance = Vector3.Distance(target, new Vector3(0, 0, crane.SpreaderPosition.z));
        if (targetDistance < 0.5f)
        {
            rd.reward += 1;
            rd.endEpisode = true;
        }

        if (targetDistance < lastDistance)
        {
            rd.reward += 1 / maxstep;
            lastDistance = targetDistance;
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

        return rd;
    }

    public override void ResetEnvironment(ICrane crane)
    {
        this.crane = crane;
    }
}
