using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelManager
{
    ICrane Crane { set; }
    Vector3 TargetPosition(int level);
    void SetEnvironment(int level);
    RewardData GetReward(int level, Collision col);
    RewardData GetRewardTrigger(int level, Collider other);
}
