using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

/// <summary>
/// Class to teach the AI to control the swing of the crane
/// Every time the environment is set, it is set to either the back of the crane, or the front randomly.
/// The goal for the AI is to move to the other side of the crane, keeping its swing below a set value.
/// In adition to the first swing lesson, the height of the spreader changes every episode.
/// </summary>
public class SwingControlDynamicHold : CraneLevel
{
    [SerializeField] float maximumSwing;
    private readonly float maxstep = 5000;
    private Vector3 target;
    private ICrane crane;
    float timeOnTarget = 0f;
    float timeToStay = 0.2f;
    int stayCounter = 0;
    int stayCounterThreshold = 100;
    int stayMax = 5;

    public override Vector3 TargetLocation => target;

    public override void OnEpisodeBegin()
    {
        bool frontSpawn = Random.value > 0.5;
        if (frontSpawn)
        {
            crane.ResetToPosition(new Vector3(0, Random.Range(10, 25), 35));
            target = new Vector3(0, 0, -10);
        }
        else
        {
            crane.ResetToPosition(new Vector3(0, Random.Range(10, 25), -10));
            target = new Vector3(0, 0, 35);
        }

    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        // Create the base reward data
        RewardData rd = new RewardData(-1f / maxstep);

        // Calculate the amount of swing in the cable and give a reward accordingly
        float swingDistance = Vector3.Distance(new Vector3(0, 0, crane.CabinPosition.z), new Vector3(0, 0, crane.SpreaderPosition.z));
        Utils.ReportStat(swingDistance, "swing");
        if (crane.CabinVelocity.z > 0 && swingDistance < 2)
        {
            rd.reward += 1f / maxstep;
        }
        else
        {
            rd.reward += -5f / maxstep;
        }

        // Calculate the distance to the target and give a reward when it's at the location. Also end the episode
        float targetDistance = Vector3.Distance(target, new Vector3(0, 0, crane.SpreaderPosition.z));
        if (targetDistance < 0.5f)
        {
            rd.reward += 1 / maxstep;

            if (timeOnTarget == 0) timeOnTarget = Time.time;

            if (Time.time - timeOnTarget >= timeToStay)
            {
                Utils.ReportStat(Time.time - timeOnTarget, "Time on target");
                rd.reward += 1;
                rd.endEpisode = true;
                stayCounter += 1;
                timeOnTarget = 0;
                if (stayCounter >= stayCounterThreshold)
                {
                    timeToStay = Mathf.Min(timeToStay + 0.2f, stayMax);
                    stayCounter = 0;
                }
            }
            
        }
        else
        {
            timeOnTarget = 0;
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
