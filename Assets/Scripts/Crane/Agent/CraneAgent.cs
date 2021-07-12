using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class CraneAgent : Agent, IAgent
{    
    [SerializeField] public CraneLevelManager levelManager;
    private ICrane crane;

    private void Start()
    {
        crane = GetComponentInChildren<ICrane>();
        CheckLevelParameter();
        levelManager.CurrentLevel.InitializeEnvironment(transform.parent);
    }

    public override void OnEpisodeBegin()
    {
        CheckLevelParameter();
        crane = levelManager.CurrentLevel.SetCraneRestrictions(crane);
        levelManager.CurrentLevel.ResetEnvironment(transform.parent, crane);
    }

    private void CheckLevelParameter()
    {
        int level = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("level_parameter", 0);
        levelManager.SetEnvironment(level);
    }

    public void OnCollisionEnter(Collision col)
    {
        throw new System.NotImplementedException();
    }

    public void OnCollisionStay(Collision col)
    {
        throw new System.NotImplementedException();
    }

    public void OnTriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public void OnTriggerExit(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public void OnTriggerStay(Collider other)
    {
        throw new System.NotImplementedException();
    }
}
