using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
