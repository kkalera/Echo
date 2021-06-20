using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class CraneAgentV2 : Agent, IAgent
{
    [Header("Objects")]
    [Space(10)]
    [SerializeField] private GameObject craneObject;
    [Header("Inputs")]
    [Space(10)]
    [SerializeField] private bool _autopilot = true;
    [SerializeField] private InputAction inputCabin;
    [SerializeField] private InputAction inputCrane;
    [SerializeField] private InputAction inputLift;
    public bool testmode;
    public int testlevel = 1;

    private ICrane crane;
    private LevelManager3 levelManager;
    private int level;

    private float cabinValue;
    private float craneValue;
    private float winchValue;

    void Start()
    {
        inputCabin.Enable();
        inputCrane.Enable();
        inputLift.Enable();
        crane = craneObject.GetComponentInChildren<ICrane>();
        levelManager = GetComponent<LevelManager3>();
    }

    private void Update()
    {
        crane.MoveCrane(craneValue);
        crane.MoveCabin(cabinValue);
        crane.MoveWinch(winchValue);
    }
    /*
    private void FixedUpdate()
    {
        crane.MoveCrane(craneValue);
        crane.MoveCabin(cabinValue);
        crane.MoveWinch(winchValue);
    }*/

    public override void OnEpisodeBegin()
    {
        level = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("level_parameter", 0);
        if (testmode) level = testlevel;
        levelManager.Crane = crane;
        levelManager.SetLevel(level);
        levelManager.OnEpisodeBegin();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Utils.Normalize(crane.CranePosition.x, -25, 25));
        sensor.AddObservation(Utils.Normalize(crane.CraneVelocity.x, -10, 10));

        sensor.AddObservation(Utils.Normalize(crane.CabinPosition.z, -25, 50));
        sensor.AddObservation(Utils.Normalize(crane.CabinVelocity.z, -10, 10));

        sensor.AddObservation(Utils.Normalize(crane.SpreaderPosition.x, -25, 25));
        sensor.AddObservation(Utils.Normalize(crane.SpreaderPosition.z, -25, 50));
        sensor.AddObservation(Utils.Normalize(crane.SpreaderPosition.y, 0, 30));

        sensor.AddObservation(Utils.Normalize(crane.SpreaderVelocity.x, -10, 10));
        sensor.AddObservation(Utils.Normalize(crane.SpreaderVelocity.z, -10, 10));
        sensor.AddObservation(Utils.Normalize(crane.SpreaderVelocity.y, -10, 10));

        sensor.AddObservation(Utils.Normalize(levelManager.TargetPosition.x, -25, 25));
        sensor.AddObservation(Utils.Normalize(levelManager.TargetPosition.z, -25, 50));
        sensor.AddObservation(Utils.Normalize(levelManager.TargetPosition.y, 0, 30));

        sensor.AddObservation(craneValue);
        sensor.AddObservation(cabinValue);
        sensor.AddObservation(winchValue);

        sensor.AddObservation(crane.ContainerGrabbed);
        sensor.AddObservation(false);
        sensor.AddObservation(false);
        sensor.AddObservation(false);

        //Debug.Log("Spreader: " + crane.SpreaderVelocity.z + "  ||  cabin: " + crane.CabinVelocity.z);
        //Add observations for:
        // Spreader locked
        // Flippers down

        /*Debug.Log(
            "Crane position normalized: " + Utils.Normalize(crane.CranePosition.x, -25, 25) +
            " | " + Utils.Normalize(crane.CabinPosition.z, -25, 50) +
            " | " + Utils.Normalize(crane.SpreaderPosition.x, -25, 25) +
            " | " + Utils.Normalize(crane.SpreaderPosition.z, -25, 50) +
            " | " + Utils.Normalize(crane.SpreaderPosition.y, 0, 30) +
            "  |||  Target position normalized: " + Utils.Normalize(levelManager.TargetPosition.x, -25, 25) +
            " | " + Utils.Normalize(levelManager.TargetPosition.z, -25, 50) +
            " | " + Utils.Normalize(levelManager.TargetPosition.y, 0, 30)
            );*/
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        if (!_autopilot)
        {

            continuousActions[0] = inputCrane.ReadValue<float>();
            continuousActions[1] = inputCabin.ReadValue<float>();
            continuousActions[2] = inputLift.ReadValue<float>();
        }
        else
        {
            Vector3 inputs = AutoPilot.GetInputs(levelManager.TargetPosition, crane.SpreaderPosition, 4f, 0.5f);
            continuousActions[0] = inputs.x;
            continuousActions[1] = inputs.z;
            continuousActions[2] = inputs.y;
        }

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<float> continousActions = actions.ContinuousActions;
        craneValue = continousActions[0];
        cabinValue = continousActions[1];
        winchValue = continousActions[2];

        RewardData rewardData = levelManager.Step();
        AddReward(rewardData.reward);
        AddReward(GetInputReward(continousActions) / MaxStep);
        AddReward(-1f / MaxStep);

        if (rewardData.endEpisode) EndEpisode();
    }

    private float GetInputReward(ActionSegment<float> continousActions)
    {
        Vector3 inputs = AutoPilot.GetInputs(levelManager.TargetPosition, crane.SpreaderPosition, 4f, 0.5f);
        float xVal = Mathf.Clamp(1 - Mathf.Abs(continousActions[0] - inputs.x), -1, 1);
        float zVal = Mathf.Clamp(1 - Mathf.Abs(continousActions[1] - inputs.z), -1, 1);
        float yVal = Mathf.Clamp(1 - Mathf.Abs(continousActions[2] - inputs.y), -1, 1);

        return (xVal + zVal + yVal) / 3;
    }

    public void OnCollisionEnter(Collision col)
    {
        RewardData rewardData = levelManager.Step(col);
        AddReward(rewardData.reward);
        if (rewardData.endEpisode) EndEpisode();
    }

    public void OnCollisionStay(Collision col)
    {
        RewardData rewardData = levelManager.Step(col);
        AddReward(rewardData.reward);
        if (rewardData.endEpisode) EndEpisode();
    }

    public void OnTriggerEnter(Collider other)
    {
        RewardData rewardData = levelManager.Step(null, other);
        AddReward(rewardData.reward);
        if (rewardData.endEpisode) EndEpisode();
    }

    public void OnTriggerStay(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public void OnTriggerExit(Collider other)
    {
        throw new System.NotImplementedException();
    }
}
