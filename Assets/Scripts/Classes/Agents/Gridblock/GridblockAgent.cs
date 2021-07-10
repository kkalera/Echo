using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class GridblockAgent : Agent
{
    [SerializeField] private Transform _Target;
    [SerializeField] private Transform _Environment;

    private Rigidbody _agentRB;


    private void Start()
    {
        _agentRB = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if(transform.localPosition.y < 0)
        {
            _agentRB.angularVelocity = Vector3.zero;
            _agentRB.velocity = Vector3.zero;
            transform.localPosition = Vector3.zero;

            _Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_Target.localPosition);
        sensor.AddObservation(_agentRB.velocity.x);
        sensor.AddObservation(_agentRB.velocity.z);
    }

    public float forceMultiplier = 10f;
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        _agentRB.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, _Target.localPosition);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Fell off platform
        else if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }
}
