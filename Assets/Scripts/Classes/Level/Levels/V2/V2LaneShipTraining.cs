using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V2LaneShipTraining : CraneLevel
{
    [Header("Height")]
    [SerializeField] [Range(0.01f, 10)] private float timeTarget = 0.1f;
    [SerializeField] [Range(0.01f, 10)] private float finalTimeTarget = 5f;
    [SerializeField] [Range(0.001f, 10)] private float discount = 0.1f;
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
    private bool flawlessEpisode;
    private bool finalTraining;
    private readonly List<int> lanes = new List<int>();

    private void Start()
    {
        lanes.Add(-4);
        lanes.Add(4);
        lanes.Add(0);
    }

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
        flawlessEpisode = true;
        if (timeTarget == finalTimeTarget) finalTraining = true;

        // Generate a random number
        float r = Random.Range(0f, 1f);

        // 1/2 chance of getting the target location to be a random spot above the ship
        if (r <= 0.5f)
        {
            _targetLocation = new Vector3(0, Random.Range(3f, 25f), Random.Range(15, 35));
        }

        // 1/2 chance of getting the target location to be a lane to go to
        else if (r > 0.5f)
        {
            _targetLocation = new Vector3(0, Random.Range(3f, 25f), lanes[Random.Range(0, lanes.Count)]);
        }

        targetIndicator.transform.localPosition = _targetLocation;

        crane.SetWinchLimits(0, 30);

        Vector3 newCraneLocation = new Vector3(0, Random.Range(15, 27), Random.Range(-10, 35));
        crane.ResetToPosition(newCraneLocation);

        endEpisodeAtNextStep = false;
        goalReached = false;
    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        // Create a base negative reward to promote speed
        float startReward = -0.5f / _maxStep;

        float radius = ((crane.SpreaderPosition.y - 3) * 0.3f) + 1;

        bool closeEnoughToLowerWinch = Mathf.Abs(_targetLocation.z - crane.SpreaderPosition.z) < radius;

        if (!closeEnoughToLowerWinch && crane.SpreaderPosition.y < 15)
        {
            startReward = -1f / _maxStep; flawlessEpisode = false;
        }
        else
        {
            startReward = 0f;
        }

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
        rd.reward += GetSpeedReward();

        // Add a reward for finishing the environment
        if (endEpisodeAtNextStep) rd.reward += 1;
        if (finalTraining) rd.reward += 1 / _maxStep;
        if (endEpisodeAtNextStep && flawlessEpisode) timeTarget = Mathf.Min(timeTarget + discount, finalTimeTarget);

        // Set the endEpisode boolean
        rd.endEpisode = endEpisodeAtNextStep;

        //_tmpro.text = "" + rd.reward;
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
            if (goalReached && enterTime == -1f) { enterTime = Time.time; }

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

        float swingNorm = Utils.Normalize(avgSwing, 0, maxSwing);
        float swingReward = Mathf.Pow(1 - swingNorm, 8) / _maxStep;
        if (swingNorm > 0.5) { swingReward = -1f / _maxStep; flawlessEpisode = false; }
        swingReward = Mathf.Clamp(swingReward, -1f, 1f);

        return swingReward;
    }
    private float GetSpeedReward()
    {
        float distance = Utils.Normalize(Mathf.Clamp(Mathf.Abs(_targetLocation.z - crane.SpreaderPosition.z), 0, 8), 0, 8);
        float speed = Utils.Normalize(Mathf.Clamp(crane.SpreaderVelocity.x, 0, 4), 0f, 4);
        float reward = (1 - Mathf.Abs(distance - speed)) / 2;
        //        _tmpro.text = "" + reward;

        return reward / _maxStep;
    }
}
