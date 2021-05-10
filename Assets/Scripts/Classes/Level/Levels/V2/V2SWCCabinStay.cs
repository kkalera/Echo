using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V2SWCCabinStay : CraneLevel
{
    [Header("Height")]
    [SerializeField] [Range(0.0001f, 1)] float _discount = 0.01f;
    [SerializeField] [Range(0.01f, 10)] private float timeTarget = 0.01f;
    [SerializeField] [Range(0.01f, 10)] private float finalTimeTarget = 0.01f;
    [Space(10)]

    [Space(10)]
    [SerializeField] int _maxStep = 5000;
    [SerializeField] Transform targetIndicator;
    [SerializeField] TMPro.TextMeshPro _tmpro;

    private ICrane crane;
    private Vector3 _targetLocation;
    private readonly List<float> swings = new List<float>();
    private bool goalReached;
    private bool endEpisodeAtNextStep;
    private float enterTime = -1f;
    private int attempts = 0;


    public override Vector3 TargetLocation => _targetLocation;

    public override void ResetEnvironment(ICrane crane)
    {
        this.crane = crane;
    }

    public override void OnEpisodeBegin()
    {
        crane.CabinMovementDisabled = false;
        crane.SwingDisabled = false;
        crane.WinchMovementDisabled = false;

        _targetLocation = new Vector3(0, Random.Range(15, 27), Random.Range(-10, 35));
        targetIndicator.transform.localPosition = _targetLocation;

        crane.SetWinchLimits(0, 30);

        Vector3 newCraneLocation = new Vector3(0, Random.Range(15, 27), Random.Range(-10, 35));
        crane.ResetToPosition(newCraneLocation);

        endEpisodeAtNextStep = false;
        goalReached = false;
        Utils.ReportStat(timeTarget, "Time target");
        Utils.ReportStat(attempts, "Attempts");
        attempts = 0;
    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        // Create a base negative reward to promote speed
        float startReward = -0.5f / _maxStep;

        // Set the base reward to positive whenever the agent is at the target to promote staying there
        if (goalReached) startReward = 1f / _maxStep;

        // Define rewardData variable
        RewardData rd = new RewardData(startReward);

        // Check for collisions with objects that end the episode
        if (col != null)
        {
            if (col.collider.CompareTag("dead")) { rd.reward += -1f; rd.endEpisode = true; return rd; }
        }

        if (other != null)
        {
            if (other.CompareTag("dead")) { rd.reward += -1f; rd.endEpisode = true; return rd; }
        }

        // Add the swing reward
        rd.reward += GetSwingReward();

        // Add a reward for finishing the environment
        if (endEpisodeAtNextStep) rd.reward += 1;
        if (endEpisodeAtNextStep) timeTarget = Mathf.Min(timeTarget + _discount, finalTimeTarget);

        // Calculate the distance to the target to determine wether or not to end the episode at the next step       
        rd.endEpisode = endEpisodeAtNextStep;

        return rd;

    }
    private void Update()
    {
        // Calculate and save the amount of swing
        float swing = Mathf.Abs(crane.CabinPosition.z - crane.SpreaderPosition.z + 1);
        swings.Add(swing);

        // Check if the episode is finished
        if (!endEpisodeAtNextStep)
        {
            // Determine if the distance between the target and the spreader is close enough
            goalReached = Vector3.Distance(_targetLocation, crane.SpreaderPosition) < 1;

            // Set the time of reaching the goal to the current time if no time is set
            if (goalReached && enterTime == -1f) { enterTime = Time.time; attempts += 1; }

            // Reset the time whenever the agent moves too far from the target
            if (!goalReached && enterTime != -1f) enterTime = -1f;

            // Determine if the episode is finished.
            if (goalReached) endEpisodeAtNextStep = Time.time > enterTime + timeTarget;
        }
    }

    private float GetSwingReward()
    {
        if (swings.Count == 0 || (crane.CabinVelocity.magnitude == 0 && !goalReached)) return 0;

        // Calculate the amount of average swing between steps.
        float totalSwing = 0;

        for (int i = 0; i < swings.Count; i++)
        {
            totalSwing += swings[i];
        }

        float avgSwing = totalSwing / swings.Count;
        swings.Clear();

        Utils.ReportStat(avgSwing, "Swing");

        // Calculate a reward based upon the average swing. 
        float maxSwing = Vector3.Distance(crane.CabinPosition, crane.SpreaderPosition);
        //float maxSwing = Mathf.Abs(crane.CabinPosition.y - crane.SpreaderPosition.y);

        float swingNorm = Utils.Normalize(avgSwing, 0, maxSwing);
        float swingReward = Mathf.Pow(1 - swingNorm, 8) / _maxStep;
        if (swingNorm > 0.5) swingReward = -1f / _maxStep;
        swingReward = Mathf.Clamp(swingReward, -1f, 1f);

        return swingReward;
    }
}
