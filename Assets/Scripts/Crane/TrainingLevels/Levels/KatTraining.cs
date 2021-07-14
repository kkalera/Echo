using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatTraining : CraneLevel
{
    [SerializeField] private float rewardDistance;
    [SerializeField] private float rewardSpeed;
    [SerializeField] private float spawnRange;  
    [SerializeField] private float maxDistance;
    [SerializeField] private GameObject targetPrefab;
    private GameObject target;

    public override Vector3 TargetLocation => target.transform.localPosition;

    public override void ClearEnvironment(Transform environment)
    {
        Destroy(environment.Find("target"));
    }

    public override RewardData GetReward(ICrane crane)
    {
        float distance = Vector3.Distance(crane.SpreaderPosition, target.transform.localPosition);
        float speed = crane.SpreaderVelocity.magnitude;

        if(distance <= rewardDistance && speed <= rewardSpeed)
        {
            IncreaseDifficulty();
            return new RewardData(1f, true);
        }

        if (distance > maxDistance) return new RewardData(0, true);

        return new RewardData();
    }

    public override void IncreaseDifficulty()
    {
        spawnRange = Mathf.Min(spawnRange + 0.1f, 50f);
        maxDistance = (spawnRange * 2) + 1;

        if (Mathf.Approximately(spawnRange, 50f)) rewardSpeed = Mathf.Max(rewardSpeed - 0.1f, 0.1f);

    }

    public override void InitializeEnvironment(Transform environment)
    {
        target = Instantiate(targetPrefab, environment);
        target.name = "target";
    }

    public override void ResetEnvironment(Transform environment, ICrane crane)
    {
        float randomZ = Random.Range(-20, 35);
        float spawnZ = Mathf.Clamp(Random.Range(randomZ - spawnRange, randomZ + spawnRange), -25, 40);

        float randomY = Random.Range(20, 25);
        float spawnY = Mathf.Clamp(Random.Range(randomY - spawnRange, randomY + spawnRange), -16, 30);

        target.transform.localPosition = new Vector3(0, randomY, randomZ);        
        crane.ResetToPosition(new Vector3(0, spawnY, spawnZ));
    }

    public override ICrane SetCraneRestrictions(ICrane crane)
    {
        crane.CabinMovementDisabled = false;
        crane.CraneMovementDisabled = true;
        crane.WinchMovementDisabled = false;
        crane.SwingDisabled = true;
        return crane;
    }
}
