using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardData
{
    public float reward;
    public bool endEpisode;
    public RewardData(float reward = 0, bool endEpisode = false)
    {
        this.reward = reward;
        this.endEpisode = endEpisode;
    }
}
