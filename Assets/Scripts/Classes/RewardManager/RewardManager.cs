using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [HideInInspector] public int level;
    private CraneAgent agent;

    private void Start()
    {
        agent = GetComponent<CraneAgent>();
    }

    public RewardData GetReward(Collision collision)
    {
        return level switch
        {
            0 => GetSwingReward(collision),
            1 => GetSwingReward(collision),
            2 => GetSwingReward(collision),
            3 => GetSwingReward(collision),
            4 => Reward2(collision),
            _ => GetHitReward(collision),
        };
    }

    private RewardData GetHitReward(Collision collision)
    {
        RewardData rewardData = new RewardData(0, false);

        if (collision != null)
        {
            if (collision.collider.CompareTag("goal"))
            {
                rewardData.reward += 1;
                rewardData.endEpisode = true;
            }
            else if (collision.collider.CompareTag("dead"))
            {
                Debug.Log("Hit dead");
                rewardData.reward -= 1;
                rewardData.endEpisode = true;
            }
        }
        return rewardData;
    }
    private RewardData GetSwingReward(Collision collision)
    {
        RewardData rewardData = GetHitReward(collision);
        Crane crane = GetComponentInChildren<Crane>();
        // Add zwier reward
        if (crane.Spreader != null && crane.Kat != null)
        {
            float zwier = Vector2.Distance(
            new Vector2(crane.Spreader.transform.localPosition.x, crane.Spreader.transform.localPosition.z),
            new Vector2(crane.Kat.transform.localPosition.x, crane.Kat.transform.localPosition.z + 1.75f)
            );
            CraneAgent agent = GetComponent<CraneAgent>();
            agent.ReportZwier(zwier);
            if (zwier > 2)
            {
                rewardData.reward -= 10 / agent.MaxStep;
            }
            else
            {
                rewardData.reward = +1 / agent.MaxStep;
            }
            if (zwier > 5) { rewardData.endEpisode = true; }
        }
        return rewardData;
    }
    private RewardData Reward2(Collision collision)
    {
        RewardData rewardData = GetSwingReward(collision);

        if (collision != null && collision.collider.CompareTag("goal"))
        {
            Crane crane = GetComponentInChildren<Crane>();
            float distance = Vector2.Distance(
                new Vector2(crane.Spreader.transform.localPosition.z, crane.Spreader.transform.localPosition.x),
                new Vector2(collision.collider.transform.localPosition.z, collision.collider.transform.localPosition.x));
            float normalizedDistance = Utils.Normalize(Mathf.Min(distance, 10), 0, 10);
            rewardData.reward += 1 - (Mathf.Pow(normalizedDistance, 2));
        }

        return rewardData;
    }
}

public class RewardData
{
    public float reward;
    public bool endEpisode;
    public RewardData(float reward, bool endEpisode)
    {
        this.reward = reward;
        this.endEpisode = endEpisode;
    }
}
