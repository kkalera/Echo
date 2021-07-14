using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;

public class CraneAgent : Agent, IAgent
{    
    [SerializeField] public CraneLevelManager levelManager;
    [SerializeField] InputAction inputActionX;
    [SerializeField] InputAction inputActionY;
    [SerializeField] InputAction inputActionZ;

    private ICrane crane;

    private void Start()
    {
        crane = GetComponentInChildren<ICrane>();
        CheckLevelParameter();
        levelManager.CurrentLevel.InitializeEnvironment(transform.parent);

        inputActionX.Enable();
        inputActionY.Enable();
        inputActionZ.Enable();
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

    public override void OnActionReceived(ActionBuffers actions)
    {
        float inputX = actions.ContinuousActions[0];
        float inputY = actions.ContinuousActions[1];
        float inputZ = actions.ContinuousActions[2];

        crane.MoveCrane(inputX);
        crane.MoveWinch(inputY);
        crane.MoveCabin(inputZ);

        RewardData rewardData = levelManager.CurrentLevel.GetReward(crane);
        SetReward(rewardData.reward);
        if (rewardData.endEpisode) EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = inputActionX.ReadValue<float>();
        continuousActions[1] = inputActionY.ReadValue<float>();
        continuousActions[2] = inputActionZ.ReadValue<float>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(crane.CabinPosition);
        sensor.AddObservation(crane.CabinVelocity);

        sensor.AddObservation(crane.CranePosition);
        sensor.AddObservation(crane.CraneVelocity);
            
        sensor.AddObservation(crane.SpreaderPosition);
        sensor.AddObservation(crane.SpreaderVelocity);

        sensor.AddObservation(levelManager.CurrentLevel.TargetLocation);

        Utils.ClearLogConsole();
        Debug.Log(crane.CabinPosition);
        Debug.Log(crane.CranePosition);
        Debug.Log(crane.SpreaderPosition);
        Debug.Log(levelManager.CurrentLevel.TargetLocation);
    }

    public void OnCollisionEnter(Collision col)
    {
    }

    public void OnCollisionStay(Collision col)
    {
    }

    public void OnTriggerEnter(Collider other)
    {
    }

    public void OnTriggerExit(Collider other)
    {
    }

    public void OnTriggerStay(Collider other)
    {
    }
}
