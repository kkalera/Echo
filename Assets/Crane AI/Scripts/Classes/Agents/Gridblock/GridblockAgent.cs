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
    [SerializeField] private Transform _Floor;

    [SerializeField] private float envScale = 1;

    private Rigidbody _agentRB;


    private void Start()
    {
        inputLeftRight.Enable();
        inputUpDown.Enable();
        _agentRB = GetComponent<Rigidbody>();

        _Floor.localScale = new Vector3(envScale, 1, envScale);
    }

    public override void OnEpisodeBegin()
    {        
        _agentRB.angularVelocity = Vector3.zero;
        _agentRB.velocity = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localPosition = new Vector3(0,0.5f,0);
        _Target.localPosition = new Vector3(Random.Range(-5*envScale, 5*envScale), 1.5f, Random.Range(-5*envScale, 5*envScale));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_Target.localPosition);
        sensor.AddObservation(_agentRB.velocity.z);
        sensor.AddObservation(_agentRB.velocity.x);
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
