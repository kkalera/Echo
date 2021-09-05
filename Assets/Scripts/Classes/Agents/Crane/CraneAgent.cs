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
    [SerializeField] bool autoPilot;
    [SerializeField] bool logData;

    private void Start()
    {        
        CheckLevelParameter();
        ICrane crane = GetComponentInChildren<ICrane>();
        levelManager.CurrentLevel.InitializeEnvironment(transform, crane);

        inputActionX.Enable();
        inputActionY.Enable();
        inputActionZ.Enable();
    }

    public override void OnEpisodeBegin()
    {
        
        CheckLevelParameter();
        levelManager.CurrentLevel.SetCraneRestrictions();        
        levelManager.CurrentLevel.ResetEnvironment();
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

        levelManager.CurrentLevel.Crane.MoveCrane(inputX);
        levelManager.CurrentLevel.Crane.MoveWinch(inputY);
        levelManager.CurrentLevel.Crane.MoveCabin(inputZ);

        //////////////////////////////

        /*
        Vector3 ap = AutoPilot.GetInputsSwing(
                levelManager.CurrentLevel.TargetLocation,
                levelManager.CurrentLevel.Crane.SpreaderPosition,
                levelManager.CurrentLevel.Crane.CabinPosition,
                levelManager.CurrentLevel.Crane.SpreaderVelocity,
                levelManager.CurrentLevel.Crane.CabinVelocity,
                levelManager.CurrentLevel.Crane.SpreaderAngularVelocity,
                0.25f);
        Vector3 ap = AutoPilot.GetInputs(
                levelManager.CurrentLevel.TargetLocation,
                levelManager.CurrentLevel.Crane.SpreaderPosition,
                levelManager.CurrentLevel.Crane.SpreaderVelocity,
                0.25f, levelManager.CurrentLevel.Crane.CabinPosition);

        float rx = 1f - Mathf.Abs(ap.x - inputX);
        float rz = 1f - Mathf.Abs(ap.z - inputZ);
        float ry = 1f - Mathf.Abs(ap.y - inputY);

        AddReward(((rx + rz + ry) / 3) / Mathf.Max(MaxStep,1));
        /*-------------------------------------------*/
        
        RewardData rewardData = levelManager.CurrentLevel.GetReward();
        AddReward(rewardData.reward);
        if (rewardData.endEpisode) EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = inputActionX.ReadValue<float>();
        continuousActions[1] = inputActionY.ReadValue<float>();
        continuousActions[2] = inputActionZ.ReadValue<float>();

        if (autoPilot)
        {
            Vector3 actions = Vector3.zero;

            actions = AutoPilot.GetInputs(
                levelManager.CurrentLevel.TargetLocation,
                levelManager.CurrentLevel.Crane.SpreaderPosition,
                levelManager.CurrentLevel.Crane.SpreaderVelocity,
                0.25f, levelManager.CurrentLevel.Crane.CabinPosition);

            continuousActions[0] = actions.x;
            continuousActions[1] = actions.y;
            continuousActions[2] = actions.z;            
        }
                

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(levelManager.CurrentLevel.Crane.CabinPosition);
        sensor.AddObservation(levelManager.CurrentLevel.Crane.CabinVelocity);
                              
        sensor.AddObservation(levelManager.CurrentLevel.Crane.CranePosition);
        sensor.AddObservation(levelManager.CurrentLevel.Crane.CraneVelocity);
                              
        sensor.AddObservation(levelManager.CurrentLevel.Crane.SpreaderPosition);
        sensor.AddObservation(levelManager.CurrentLevel.Crane.SpreaderVelocity);

        sensor.AddObservation(levelManager.CurrentLevel.TargetLocation);
    }


    public void OnCollisionEnter(Collision col)
    {        
        if (col.collider.CompareTag("dead")) EndEpisode();
        if (col.collider.CompareTag("crane")) EndEpisode();
    }

    public void OnCollisionStay(Collision col)
    {
        if (col.collider.CompareTag("dead")) EndEpisode();
        if (col.collider.CompareTag("crane")) EndEpisode();

        ContactPoint[] cps = col.contacts;
        foreach (ContactPoint cp in cps)
        {                
        }
        Utils.ClearLogConsole();
        Debug.Log(col.collider.tag);
        Debug.Log(col.relativeVelocity);     
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dead")) EndEpisode();
        if (other.CompareTag("crane")) EndEpisode();
    }

    public void OnTriggerExit(Collider other)
    {        
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("dead")) EndEpisode();
        if (other.CompareTag("crane")) EndEpisode();
    }
}