using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V2SWCCabin : CraneLevel
{
    [Header("Height")]
    [SerializeField] [Range(-0.0001f, 1)] float _discount = 0.01f;
    [SerializeField] [Range(1, 25)] private float spawnRadius = 2;
    [Space(10)]

    [Space(10)]
    [SerializeField] int _maxStep = 5000;
    [SerializeField] Transform targetIndicator;
    [SerializeField] TMPro.TextMeshPro _tmpro;

    private ICrane crane;
    private Vector3 _targetLocation;
    private readonly List<float> swings = new List<float>();
    private bool goalReached;
    private bool flawlessEpisode;

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

        float randomSpreaderHeight = Random.Range(15, 27);
        float randomTargetHeight = Mathf.Clamp(Random.Range(randomSpreaderHeight - spawnRadius / 2, randomSpreaderHeight + spawnRadius / 2), 15, 27);
        _targetLocation = new Vector3(0, randomTargetHeight, Random.Range(-10, 35));
        targetIndicator.transform.localPosition = _targetLocation;

        crane.SetWinchLimits(randomSpreaderHeight - spawnRadius / 2, randomSpreaderHeight + spawnRadius / 2);

        Vector3 newCraneLocation = new Vector3(0, randomSpreaderHeight, Random.Range(-10, 35));
        crane.ResetToPosition(newCraneLocation);

        //ended = false;
        goalReached = false;
    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        // Define rewardData variable
        RewardData rd = new RewardData(-0.5f / _maxStep);

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
        if (goalReached) rd.reward += 1;
        if (goalReached && flawlessEpisode) UpdateDynamicValues();

        // Calculate the distance to the target to determine wether or not to end the episode at the next step       
        rd.endEpisode = goalReached;

        return rd;

    }
    private void Update()
    {
        // Calculate and save the amount of swing
        float swing = Mathf.Abs(crane.CabinPosition.z - crane.SpreaderPosition.z + 1);
        swings.Add(swing);

        // Check if the episode is finished
        //if (!ended)
        goalReached = Vector3.Distance(_targetLocation, crane.SpreaderPosition) < 1;
    }

    private void UpdateDynamicValues()
    {
        spawnRadius = Mathf.Min(spawnRadius + _discount, 20);
        Utils.ReportStat(spawnRadius, "Level 1 / SpawnRadius");
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
        if (swingNorm > 0.5) { swingReward = -1f / _maxStep; flawlessEpisode = false; }
        swingReward = Mathf.Clamp(swingReward, -1f, 1f);

        return swingReward;
    }
}
