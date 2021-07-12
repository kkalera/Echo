using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;

public class GridblockAgent : Agent
{
    [SerializeField] private InputAction inputUpDown;
    [SerializeField] private InputAction inputLeftRight;
    
    [SerializeField] private Transform _Target;
    [SerializeField] private Transform _Environment;

    private Rigidbody _agentRB;


    private void Start()
    {
        inputLeftRight.Enable();
        inputUpDown.Enable();
        _agentRB = GetComponent<Rigidbody>();
        float i = -5f;
        while (i<5)
        {
            Debug.Log(Utils.Normalize(i, -5, 5));
            i += 0.1f;
        }
    }

    public override void OnEpisodeBegin()
    {        
        _agentRB.angularVelocity = Vector3.zero;
        _agentRB.velocity = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localPosition = new Vector3(0,0.5f,0);
        _Target.localPosition = new Vector3(Random.Range(-5, 5), 1.5f, Random.Range(-5, 5));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(_Target.localPosition);
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);
        sensor.AddObservation(transform.localPosition.z);
        sensor.AddObservation(_Target.localPosition.x);
        sensor.AddObservation(_Target.localPosition.y);
        sensor.AddObservation(_Target.localPosition.z);
        //sensor.AddObservation(Utils.Normalize(transform.localPosition.x, -7, 7));
       // sensor.AddObservation(Utils.Normalize(transform.localPosition.z, -7, 7));
        //sensor.AddObservation(Utils.Normalize(transform.localPosition.y, -5, 5));
       // sensor.AddObservation(Utils.Normalize(_Target.localPosition.x, -7, 7));
       // sensor.AddObservation(Utils.Normalize(_Target.localPosition.z, -7, 7));
       // sensor.AddObservation(Utils.Normalize(_Target.localPosition.y, -5, 5));
        sensor.AddObservation(_agentRB.velocity.x);
        sensor.AddObservation(_agentRB.velocity.z);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = inputUpDown.ReadValue<float>();
        continuousActions[1] = inputLeftRight.ReadValue<float>();
        float rx = Mathf.Min(Mathf.Abs(transform.localPosition.x - _Target.localPosition.x), 1);
        float rz = Mathf.Min(Mathf.Abs(transform.localPosition.z - _Target.localPosition.z), 1);
    }

    public float forceMultiplier = 10f;
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];

        transform.localPosition =transform.localPosition + (controlSignal*Time.deltaTime);

        float rx = Mathf.Min(Mathf.Abs(transform.localPosition.x - _Target.localPosition.x),1) / (MaxStep * 4);
        float rz = Mathf.Min( Mathf.Abs(transform.localPosition.z - _Target.localPosition.z),1) / (MaxStep * 4);
        //AddReward(rx + rz);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, _Target.localPosition);

        // Reached target
        if (distanceToTarget < 2f)
        {
            SetReward(1.0f);
            //AddReward((0.5f /MaxStep)* (MaxStep - StepCount));
            EndEpisode();
        }

        // Fell off platform
        else if (this.transform.localPosition.y < -0.5)
        {
            Utils.ClearLogConsole();
            Debug.Log("dead");
            EndEpisode();
        }
    }
}
