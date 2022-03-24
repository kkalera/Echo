using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class TestAgent : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private bool autoPilot;
    private Rigidbody rbody;
    private float inputX;
    private float inputZ;

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();        
    }

    public override void OnEpisodeBegin()
    {        
        transform.position = new Vector3(-5,0.1f,5) - transform.parent.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rbody.velocity);
        sensor.AddObservation(transform.position - transform.parent.position);
        sensor.AddObservation(target.transform.position - transform.parent.position);
    }

    private void FixedUpdate()
    {
        Echo.Utils.AccelerateRigidbody_X_Axis(rbody, inputX * 5, 5, 5, Time.fixedDeltaTime);
        Echo.Utils.AccelerateRigidbody_Z_Axis(rbody, inputZ * 5, 5, 5, Time.fixedDeltaTime);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        if (!autoPilot)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) actions[0] = 1;
            if (Input.GetKey(KeyCode.RightArrow)) actions[0] = -1;
            if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)) actions[0] = 0;

            if (Input.GetKey(KeyCode.UpArrow)) actions[1] = 1;
            if (Input.GetKey(KeyCode.DownArrow)) actions[1] = -1;
            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) actions[1] = 0;
        }
        else
        {
            var inputs = GetInputs(target.position - transform.parent.position, transform.position - transform.parent.position, rbody.velocity, new Vector3(5, 5, 5));
            actions[0] = inputs.z;
            actions[1] = inputs.x;
        }
        
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        var cActions = actions.ContinuousActions;
        inputZ = cActions[0];
        inputX = cActions[1];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Target"))
        {
            AddReward(1);
            EndEpisode();
        }
    }
    public static Vector3 GetInputs(Vector3 targetPosition, Vector3 spreaderPosition, Vector3 currentSpeed, Vector3 acceleration)
    {
        Vector3 inputs = new Vector3(0, 0, 0);
        targetPosition = GetNextPosition(spreaderPosition, targetPosition);

        ///// Z movement
        float distanceZ = Mathf.Abs(spreaderPosition.z - targetPosition.z);
        if (!Mathf.Approximately(distanceZ, 0))
        {
            float vel = Mathf.Abs(currentSpeed.z);
            float d = Mathf.Pow(vel, 2) / (2 * acceleration.z);
            inputs.z = distanceZ - d;

            if (targetPosition.z < spreaderPosition.z) inputs.z = -inputs.z;
            inputs.z = Mathf.Clamp(inputs.z, -1, 1);
        }
        /////

        ///// Y movement
        float distanceY = Mathf.Abs(spreaderPosition.x - targetPosition.x);
        if (!Mathf.Approximately(distanceY, 0))
        {
            float vel = Mathf.Abs(currentSpeed.x);
            float d = Mathf.Pow(vel, 2) / (2 * acceleration.x);
            inputs.x = distanceY - d;
            if (targetPosition.x < spreaderPosition.x) inputs.x = -inputs.x;
            inputs.x = Mathf.Clamp(inputs.x, -1, 1);
        }
        /////
        return inputs;

    }
    private static Vector3 GetNextPosition(Vector3 spreaderPosition, Vector3 targetPosition)
    {

        if (spreaderPosition.x < 2 && spreaderPosition.z > -7)
        {
            targetPosition.z = -8;
            targetPosition.x = 2;
        }
        if(spreaderPosition.x <=2 && spreaderPosition.z > -7) targetPosition.z = -8;

        if (spreaderPosition.z <= -7 && spreaderPosition.x >= 2 && !Mathf.Approximately(spreaderPosition.x,6.5f))
        {
            targetPosition.z = -8;
            targetPosition.x = 6.5f;
        }        
        
        
        return targetPosition;
    }
}
