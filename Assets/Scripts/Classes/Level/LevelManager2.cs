using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEditor;
using UnityEngine;

public class LevelManager2 : MonoBehaviour, ILevelManager
{
    private ICrane crane;
    public ICrane Crane { set => crane = value; }

    [SerializeField] public List<CraneLevel> levels;

    public Vector3 TargetPosition(int level)
    {
        Vector3 location = levels[level].TargetLocation;
        if (location != null) return location;
        else { return Vector3.zero; }
    }

    public RewardData GetReward(int level, Collision col)
    {
        switch (level)
        {
            case 0: return levels[0].Step();
            case -1: return new RewardData();
            default: return new RewardData();
        }
    }
    public void SetEnvironment(int level)
    {
        switch (level)
        {
            //case 0: crane.ResetToRandomPosition(); SetTargetPlane(); break;
            case 0: levels[0].ResetEnvironment(crane); levels[0].Maxstep = GetComponent<Agent>().MaxStep; break;
            case -1: StopEnvironment(); break;
            default: break;
        }
    }
    private void StopEnvironment()
    {
        //EditorApplication.isPlaying = false; //EditorApplication.ExecuteMenuItem("Edit/Play");
        Application.Quit();
    }
    private RewardData GetCollisionReward(Collision col, RewardData rData)
    {
        if (col == null) { rData.reward += 0; return rData; }

        switch (col.collider.tag)
        {
            case "target": rData.reward += 1; rData.endEpisode = true; return rData;
            case "dead": rData.reward += -1; rData.endEpisode = true; return rData;
            default: return rData;
        }
    }
    private RewardData GetTriggerReward(Collider other, RewardData rData)
    {
        if (other == null) { return rData; }

        switch (other.tag)
        {
            case "target": rData.reward += 1; rData.endEpisode = true; return rData;
            case "dead": rData.reward += -1; rData.endEpisode = true; return rData;
            default: return rData;
        }
    }
    private float GetSwingReward()
    {
        Vector3 spreaderPosition = crane.SpreaderPosition;
        Vector3 cabinPosition = crane.CabinPosition;

        float distance = Mathf.Abs(cabinPosition.z - spreaderPosition.z);
        if (distance < 2 && distance != 0) return 1 / GetComponent<Agent>().MaxStep;
        return 0f;
    }


    public RewardData GetRewardTrigger(int level, Collider other)
    {
        RewardData r = new RewardData(0, false);
        switch (level)
        {
            case 0:
                {
                    r = GetTriggerReward(other, r);
                    r.reward += GetSwingReward();
                    return r;
                }

            case -1: return r;
            default: return r;
        }
    }
}
