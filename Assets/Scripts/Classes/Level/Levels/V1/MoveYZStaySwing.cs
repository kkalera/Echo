using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

/// <summary>
/// Class to teach the AI to control the swing of the crane
/// Every time the environment is set, it is set to either the back of the crane, or the front randomly.
/// The goal for the AI is to move to the other side of the crane, keeping its swing below a set value.
/// </summary>
public class MoveYZStaySwing : CraneLevel
{

    [SerializeField] private Transform targetObject;
    [SerializeField] private TMPro.TextMeshPro tmpro;
    [SerializeField] private float stayTime = 0;
    [SerializeField] private float maxStayTime = 5;
    [SerializeField] private float stayIncrement = 0.01f;
    [SerializeField] private float minY = 15;
    [SerializeField] private float startMaxSwing = 5;
    [SerializeField] private float finalMaxSwing = 1f;
    [SerializeField] private float yDiscount = 0.01f;

    private readonly float maxstep = 5000;
    private Vector3 target;
    private ICrane crane;
    private float enterTime = 0;

    public override Vector3 TargetLocation => target;

    public override void OnEpisodeBegin()
    {
        minY = Mathf.Max(minY - yDiscount, 15);
        startMaxSwing = Mathf.Max(finalMaxSwing, startMaxSwing - yDiscount);
        crane.ResetToPosition(new Vector3(0, 25, Random.Range(-15, 40)));
        target = new Vector3(0, Random.Range(minY, 25), Random.Range(-15, 40));
        targetObject.localScale = Vector3.one;
        targetObject.localPosition = target;

        crane.MinSpreaderHeight = minY;
        crane.SwingDisabled = false;
        crane.WinchMovementDisabled = false;
        crane.CabinMovementDisabled = false;
    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        // Create the base reward data
        RewardData rd = new RewardData();

        // Calculate the distance to the target and give a reward when it's at the location. Also end the episode
        float targetDistance = Vector3.Distance(target, crane.SpreaderPosition);
        if (targetDistance < 1f)
        {
            if (enterTime == 0)
            {
                enterTime = Time.time;
            }
            else
            {
                if (tmpro != null) { tmpro.text = "Time.time : " + Time.time + "  ||  goal: " + (enterTime + stayTime); }
                if (Time.time > enterTime + stayTime)
                {
                    if (stayTime == maxStayTime && minY == 15 && startMaxSwing == finalMaxSwing) { rd.reward += 1; } else { rd.reward += 0.8f; }
                    rd.endEpisode = true;
                }
                else
                {
                    rd.reward += 1f / maxstep;
                }
            }
        }
        else
        {
            enterTime = 0;
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
                case "dead": rd.reward += -1; rd.endEpisode = true; break;
                default: break;
            }
        }

        if (crane.SpreaderPosition.y >= 30)
        {
            rd.reward += -1;
            rd.endEpisode = true;
        }

        // Calculate the amount of swing in the cable and give a reward accordingly
        float swingDistance = Vector3.Distance(new Vector3(0, 0, crane.CabinPosition.z), new Vector3(0, 0, crane.SpreaderPosition.z));
        Utils.ReportStat(swingDistance, "swing");

        if (crane.CabinVelocity.z > 0 && swingDistance <= startMaxSwing)
        {
            float distanceNormalized = Mathf.Min(Utils.Normalize(swingDistance, 0, startMaxSwing), 1);
            float invertedDistance = 1 - distanceNormalized;
            rd.reward += Mathf.Min(Mathf.Pow(invertedDistance, 2), 1) / maxstep;
        }

        return rd;
    }

    public override void ResetEnvironment(ICrane crane)
    {
        this.crane = crane;
    }
}
