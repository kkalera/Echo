using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The goal of this training is to teach the agent to pick up a container and end the episode when it picks it up
/// To prevent unlearning of previous lessons, there is a 50% chance the environment will be set to perform a previous lesson
/// by placing a random target and the agent having to move there
/// </summary>

public class V2GrabContainer : CraneLevel
{
    [Header("Training variables")]
    [SerializeField] [Range(0.01f, 10)] private float timeTarget = 0.1f; // The amount of time to stay at the target
    [SerializeField] [Range(0.01f, 1f)] private float grabRadius = 1f; // The radius wherein the agent can grab the container
    [SerializeField] [Range(0.001f, 1f)] private float discount = 0.01f; // The amount to decrease the grabRadius on every succesful episode
    [Space(10)]

    [SerializeField] bool training = true;
    [SerializeField] GameObject containerObject;
    [SerializeField] Transform targetPlane;

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
    private Vector3 grabbedPosition;
    private bool grabbedContainer;
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
        // Set the local crane opject
        this.crane = crane;

        // Set the limits of the crane's winch to prevent pulling the spreader too high or going too low
        this.crane.SetWinchLimits(0, 30);
    }

    public override void OnEpisodeBegin()
    {
        // Set the abilities the crane can use
        crane.CabinMovementDisabled = false;
        crane.SwingDisabled = false;
        crane.WinchMovementDisabled = false;

        // Set flawlessEpisode bool to true. This will get set to false if the agent makes a mistake.
        // This boolean will determine wether or not the difficulty of the environment will be increased.
        flawlessEpisode = true;

        // Set the final training boolean that provides an increased reward when the final difficulty has been reached
        // This larger reward is used to determine wether or not the training is finished.
        // TODO: Define final training requirements
        finalTraining = false;

        // Choose a random Lane to position the container at
        _targetLocation = new Vector3(0, 3f, lanes[Random.Range(0, lanes.Count)]);

        targetPlane.localPosition = new Vector3(0, Random.Range(0f, 10f), Random.Range(15f, 35f));

        // Enable the container object
        containerObject.SetActive(true);

        // Disable the target sphere meshrenderer
        targetIndicator.GetComponent<MeshRenderer>().enabled = false;

        // Release the container
        crane.ReleaseContainer(crane.Transform);

        // Move the target to the position set above
        targetIndicator.transform.localPosition = _targetLocation;

        // Generate a random location for the crane to start at
        Vector3 newCraneLocation = new Vector3(0, Random.Range(15, 27), Random.Range(-10, 35));

        // Move the crane to that new position
        crane.ResetToPosition(newCraneLocation);

        // Set these booleans to false so the episode is not stopped without completing the goal again.
        endEpisodeAtNextStep = false;
        goalReached = false;
        grabbedContainer = false;

    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        // Create a reward float that will be determined later
        float startReward;


        // Calculate the distance forwards and backwards wherein the crane is allowed to lower below 15m height
        float radius = ((crane.SpreaderPosition.y - 3) * 0.3f) + 1;

        // Determine wether the spreader is close enough to be lowered.
        bool closeEnoughToLowerWinch = Mathf.Abs(_targetLocation.z - crane.SpreaderPosition.z) < radius;

        // If the spreader is nog close enough to lower the winch, and the spreader is below
        // 15 meters, add a negative reward to incentivise the agent to stay above 15m.
        if (!closeEnoughToLowerWinch && crane.SpreaderPosition.y < 15)
        {
            startReward = -10f / _maxStep; flawlessEpisode = false;
        }

        // Set a base reward of 0 when the agent is above 15m, or is close enough to the target lane that it can lower the spreader.
        else
        {
            if (crane.SpreaderPosition.y > 15)
            {
                startReward = 0.5f / _maxStep;
            }
            else
            {
                startReward = -0.5f / _maxStep;
            }

        }

        if (grabbedContainer && crane.SpreaderPosition.y < 15)
        {
            if (crane.SpreaderPosition.z > grabbedPosition.z + radius || crane.SpreaderPosition.z < grabbedPosition.z - radius)
            {
                startReward = -10f / _maxStep; flawlessEpisode = false;
            }
        }

        // Set the base reward to positive whenever the agent is at the target to promote staying there
        if (goalReached) startReward = 1f / _maxStep;

        // Define rewardData variable
        RewardData rd = new RewardData(startReward);

        // Check for collisions with objects that end the episode
        if (col != null)
        {
            if (col.collider.CompareTag("dead")) { rd.reward += -1f; rd.endEpisode = true; return rd; }
            if (col.collider.CompareTag("target")) { rd.reward += 1f; rd.endEpisode = true; return rd; }
        }

        if (other != null)
        {
            if (other.CompareTag("dead")) { rd.reward += -1f; rd.endEpisode = true; return rd; }
            if (col.collider.CompareTag("target")) { rd.reward += 1f; rd.endEpisode = true; return rd; }
        }

        // Add the swing reward
        rd.reward += GetSwingReward();

        // Add a reward based on speed, promoting high speed when far from the target, and low speed when close.
        rd.reward += GetSpeedReward();

        // Add a reward every step when at the final training stage to use in curriculum training as a trigger to continue
        if (finalTraining) rd.reward += 10 / _maxStep;

        if (endEpisodeAtNextStep && flawlessEpisode) grabRadius = Mathf.Max(grabRadius - discount, 0.05f);

        // Set the endEpisode boolean
        rd.endEpisode = endEpisodeAtNextStep;

        // Add a reward for finishing the environment
        if (endEpisodeAtNextStep) rd.reward += 1;

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
            if (!goalReached) goalReached = Vector3.Distance(_targetLocation, crane.SpreaderPosition) < grabRadius;

            if (goalReached)
            {
                crane.GrabContainer(targetIndicator);
                grabbedPosition = _targetLocation;
                _targetLocation = targetPlane.localPosition + new Vector3(0, 2.75f, 0);
            }

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
        return reward / _maxStep;
    }
}
