using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;

public class BasicAgent : Agent
{
    [SerializeField] InputAction xMovement;
    [SerializeField] InputAction zMovement;
    [SerializeField] Rigidbody agentBody;

    private void Start()
    {
        xMovement.Enable();
        zMovement.Enable();
    }


    private static void AccelerateTo(Rigidbody body, Vector3 targetVelocity, float maxAccel, ForceMode forceMode = ForceMode.Acceleration)
    {
        Vector3 deltaV = targetVelocity - body.velocity;
        Vector3 accel = deltaV / Time.deltaTime;

        if (accel.sqrMagnitude > maxAccel * maxAccel)
            accel = accel.normalized * maxAccel;

        body.AddForce(accel, forceMode);
    }


    public override void OnEpisodeBegin()
    {
        transform.position = new Vector3(Random.Range(-9, 9), 0.1f, 9);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
            
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 a = Vector3.zero;
        a.z = actions.DiscreteActions[0];
        a.x = actions.DiscreteActions[1];

        Utils.ClearLogConsole();
        Debug.Log("action received: " + a);

        AccelerateTo(agentBody, a, 1);

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discrete = actionsOut.DiscreteActions;
        discrete[0] = ((int)zMovement.ReadValue<float>());
        discrete[1] = ((int)xMovement.ReadValue<float>());

    }
}
