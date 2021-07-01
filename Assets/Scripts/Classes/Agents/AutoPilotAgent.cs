using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AutoPilotAgent : Agent, IAgent
{
    [Header("Training options")]
    [Space(10)]
    [SerializeField] public bool testmode;
    [SerializeField] public int testlevel = 1;
    //[SerializeField] public bool NegativeRewardsEnabled;
    [Header("Objects")]
    [Space(10)]
    [SerializeField] private GameObject craneObject;
    [Header("Inputs")]
    [SerializeField] private bool _autopilot = true;
    [SerializeField] private InputAction inputCabin;
    [SerializeField] private InputAction inputCrane;
    [SerializeField] private InputAction inputLift;


    private Vector3 _spreaderPosition;
    private Vector3 _targetPosition;
    private float cabinVelocity;
    private float spreaderVelocity;
    private float cabinPosition;    

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

    public override void OnEpisodeBegin()
    {
        cabinVelocity = Random.Range(0f, 4f);
        spreaderVelocity = Random.Range(0f, 4f);
        float spreaderHeight = Random.Range(1f, 25f);
        cabinPosition = Random.Range(-25f, 35f);

        if (spreaderHeight < 15f)
        {
            if (cabinPosition > -13 && cabinPosition < -4) cabinPosition = -4f;
            if (cabinPosition < 14 && cabinPosition > 4) cabinPosition = 14f;
        }

        _spreaderPosition = new Vector3(0, spreaderHeight, cabinPosition);

        Vector3 _targetPosition = new Vector3(0, Random.Range(3f, 25f), Random.Range(-25f, 35f));
        if (_targetPosition.z > 13 && _targetPosition.z < -4) _targetPosition.z = -4f;
        if (_targetPosition.z < 14 && _targetPosition.z > 4) _targetPosition.z = 14f;

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        /*sensor.AddObservation(Utils.Normalize(crane.CranePosition.x, -25, 25));
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
        sensor.AddObservation(false);*/

        
        
        sensor.AddObservation(Utils.Normalize(0, -25, 25));
        sensor.AddObservation(Utils.Normalize(0, -10, 10));

        sensor.AddObservation(Utils.Normalize(cabinPosition, -25, 50));
        sensor.AddObservation(Utils.Normalize(cabinVelocity, -10, 10));

        sensor.AddObservation(0f);
        sensor.AddObservation(Utils.Normalize(cabinPosition, -25, 50));
        sensor.AddObservation(Utils.Normalize(_spreaderPosition.y,0,30));

        sensor.AddObservation(Utils.Normalize(0, -10, 10));
        sensor.AddObservation(Utils.Normalize(0, -10, 10));
        sensor.AddObservation(Utils.Normalize(spreaderVelocity, -10, 10));

        sensor.AddObservation(Utils.Normalize(_targetPosition.x, -25, 25));
        sensor.AddObservation(Utils.Normalize(_targetPosition.z, -25, 50));
        sensor.AddObservation(Utils.Normalize(_targetPosition.y, 0, 30));

        sensor.AddObservation(false);
        sensor.AddObservation(false);
        sensor.AddObservation(false);
        sensor.AddObservation(false);
        sensor.AddObservation(false);
        sensor.AddObservation(false);
        sensor.AddObservation(false);

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
            Vector3 inputs = AutoPilot.GetInputs(levelManager.TargetPosition, crane.SpreaderPosition, new Vector3(0, crane.SpreaderVelocity.y, crane.CabinVelocity.z), 0.25f);
            continuousActions[0] = inputs.x;
            continuousActions[1] = inputs.z;
            continuousActions[2] = inputs.y;
        }        

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<float> continuousActions = actions.ContinuousActions;
        craneValue = continuousActions[0];
        cabinValue = continuousActions[1];
        winchValue = continuousActions[2];
        
        AddReward(GetInputRewards(continuousActions).z);
        AddReward(GetInputRewards(continuousActions).y);

    }

    private Vector3 GetInputRewards(ActionSegment<float> continousActions)
    {

        Vector3 inputs = AutoPilot.GetInputs(levelManager.TargetPosition, crane.SpreaderPosition, new Vector3(0, crane.SpreaderVelocity.y, crane.CabinVelocity.z), 0.25f);

        float xVal = Mathf.Clamp(1f - Mathf.Abs(continousActions[0] - inputs.x), -1f, 1f);
        float zVal = Mathf.Clamp(1f - Mathf.Abs(continousActions[1] - inputs.z), -1f, 1f);
        float yVal = Mathf.Clamp(1f - Mathf.Abs(continousActions[2] - inputs.y), -1f, 1f);

        return new Vector3(xVal,yVal, zVal);
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
