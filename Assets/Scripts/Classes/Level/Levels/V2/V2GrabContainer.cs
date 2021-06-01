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
    [SerializeField] [Range(0.01f, 10)] private float timeToGrab = 2f; // The amount of time to stay at the target
    [SerializeField] [Range(0.01f, 1f)] private float grabRadius = 1f; // The radius wherein the agent can grab the container
    [SerializeField] [Range(0.001f, 1f)] private float discount = 0.001f; // The amount to decrease the grabRadius on every succesful episode
    [SerializeField] [Range(8f, 35f)] private float shipZ = 8f;
    [SerializeField] [Range(8f, 35f)] private float maxShipZ = 35f;
    [Space(10)]

    [SerializeField] bool training = true;
    [SerializeField] GameObject containerObject;
    [SerializeField] Transform containerTarget;

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
    private Vector3 lanePosition;
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
        finalTraining = shipZ == maxShipZ;

        // Choose a random Lane to position the container at
        _targetLocation = new Vector3(0, 3f, lanes[Random.Range(0, lanes.Count)]);

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
        RewardData rd = new RewardData();

        // Add swing and speed reward
        rd.reward += GetSwingReward();
        rd.reward += GetSpeedReward();

        // Add a reward when lowering
        rd.reward += GetLoweredBetweenLegsReward();

        // Check for collisions with objects that end the episode
        if (col != null) { if (col.collider.CompareTag("dead")) { rd.reward += -1f; rd.endEpisode = true; return rd; } }
        if (other != null) { if (other.CompareTag("dead")) { rd.reward += -1f; rd.endEpisode = true; return rd; } }

        // Set the endEpisode boolean
        rd.endEpisode = endEpisodeAtNextStep;

        // Add a reward for completing the episode
        if (endEpisodeAtNextStep)
        {
            rd.reward += 1;
            if (flawlessEpisode)
            {
                grabRadius = Mathf.Max(grabRadius - discount, 0.1f);
                shipZ = Mathf.Max(shipZ + discount, maxShipZ);
            }
        }

        if (finalTraining) rd.reward += 1f / _maxStep;
        _tmpro.text = "" + rd.reward;

        return rd;

    }
    private void Update()
    {
        // Calculate and save the amount of swing
        float swing = Mathf.Abs(crane.CabinPosition.z - crane.SpreaderPosition.z + 1);
        swings.Add(swing);
        //---------------------------------------------------------------------------------


        // Check if the episode is finished
        if (!endEpisodeAtNextStep && grabbedContainer)
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
        //---------------------------------------------------------------------------------


        // Check if the crane can grab the container
        if (!grabbedContainer)
        {
            // Determine if the distance between the target and the spreader is close enough
            if (!goalReached)
            {
                goalReached = Vector3.Distance(_targetLocation, crane.SpreaderPosition) < grabRadius;
                if (goalReached && enterTime == -1f) { enterTime = Time.time; }

                // Reset the time whenever the agent moves too far from the target
                if (!goalReached && enterTime != -1f) enterTime = -1f;
            }

            if (goalReached)
            {
                if (Time.time > enterTime + timeTarget)
                {
                    crane.GrabContainer(targetIndicator);
                    _targetLocation = new Vector3(0, 16f, shipZ);
                    containerTarget.localPosition = _targetLocation - new Vector3(0, 2.75f, 0);
                    enterTime = -1f;
                    grabbedContainer = true;
                }
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
    private float GetLoweredBetweenLegsReward()
    {
        float r = 0;

        // Only calculate a reward when below 15m
        if (crane.SpreaderPosition.y < 15)
        {
            // Calculate the distance allowed from the container position
            // The allowed distance is a cone of 20% of the height.
            float radiusAllowed = Mathf.Max(crane.SpreaderPosition.y, 2f) * 0.2f;

            // Calculate the distance from the lane 
            float distance = Mathf.Abs(crane.SpreaderPosition.x - lanePosition.z);

            // If the distance is below the radius, add a positive reward            
            if (distance <= radiusAllowed)
            {
                r = 1f / _maxStep;
            }

            // If the agent is outside of the allowed range, add a negative reward
            else
            {
                flawlessEpisode = false;
                r = -1f / _maxStep;
            }

        }

        return r;
    }
}
