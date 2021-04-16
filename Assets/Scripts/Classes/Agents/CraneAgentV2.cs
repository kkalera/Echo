using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CraneAgentV2 : Agent, IAgent
{
    [Header("Objects")]
    [Space(10)]
    [SerializeField] private GameObject craneObject;
    [Header("Inputs")]
    [Space(10)]
    [SerializeField] private InputAction inputCabin;
    [SerializeField] private InputAction inputCrane;
    [SerializeField] private InputAction inputLift;
    public bool testmode;
    public int testlevel = 1;

    private ICrane crane;
    private LevelManager3 levelManager;
    private int level;

    void Start()
    {
        inputCabin.Enable();
        inputCrane.Enable();
        inputLift.Enable();
        crane = craneObject.GetComponentInChildren<ICrane>();
        levelManager = GetComponent<LevelManager3>();
    }

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
        sensor.AddObservation(crane.CraneVelocity.x);

        sensor.AddObservation(Utils.Normalize(crane.CabinPosition.z, -25, 50));
        sensor.AddObservation(crane.CabinVelocity.z);

        sensor.AddObservation(Utils.Normalize(crane.SpreaderPosition.x, -25, 25));
        sensor.AddObservation(Utils.Normalize(crane.SpreaderPosition.z, -25, 50));
        sensor.AddObservation(Utils.Normalize(crane.SpreaderPosition.y, 0, 30));

        sensor.AddObservation(crane.SpreaderVelocity.x);
        sensor.AddObservation(crane.SpreaderVelocity.z);
        sensor.AddObservation(crane.SpreaderVelocity.y);

        sensor.AddObservation(Utils.Normalize(levelManager.TargetPosition.x, -25, 25));
        sensor.AddObservation(Utils.Normalize(levelManager.TargetPosition.z, -25, 50));
        sensor.AddObservation(Utils.Normalize(levelManager.TargetPosition.y, 0, 30));

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
        continuousActions[0] = inputCrane.ReadValue<float>();
        continuousActions[1] = inputCabin.ReadValue<float>();
        continuousActions[2] = inputLift.ReadValue<float>();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<float> continousActions = actions.ContinuousActions;
        crane.MoveCrane(continousActions[0]);
        crane.MoveCabin(continousActions[1]);
        crane.MoveWinch(continousActions[2]);
        RewardData rewardData = levelManager.Step();
        AddReward(rewardData.reward);
        if (rewardData.endEpisode) EndEpisode();
    }

    public void OnCollisionEnter(Collision col)
    {
        RewardData rewardData = levelManager.Step(col);
        AddReward(rewardData.reward);
        if (rewardData.endEpisode) EndEpisode();
    }

    public void OnCollisionStay(Collision col)
    {
        throw new System.NotImplementedException();
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
}
