using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;
using Unity.MLAgents.Sensors;

public class BasicTrainingAgent : Agent, IAgent
{
    [SerializeField] private Transform crane;
    [SerializeField] private Transform cabin;
    [SerializeField] private Transform spreader;
    [SerializeField] private Transform target;

    [SerializeField] private HingeJoint spool;
    [SerializeField] private Filo.Cable cable;

    [SerializeField] private bool craneMovementEnabled;
    [SerializeField] private bool cabinMovementEnabled;
    [SerializeField] private bool winchMovementEnabled;
    [SerializeField] private bool swingEnabled;

    [SerializeField] private InputAction inputCabin;
    [SerializeField] private InputAction inputCrane;
    [SerializeField] private InputAction inputLift;

    private MovementManager movementManager;

    private Rigidbody rbCrane;
    private Rigidbody rbCabin;
    private Rigidbody rbSpreader;

    private float craneValue;
    private float cabinValue;
    private float winchValue;

    private float enterTime;
    private float stayTime;
    private float maxStayTime;
    private int level;


    private void Start()
    {
        rbCrane = crane.GetComponent<Rigidbody>();
        rbCabin = cabin.GetComponent<Rigidbody>();
        rbSpreader = spreader.GetComponent<Rigidbody>();

        movementManager = GetComponent<MovementManager>();

        inputCrane.Enable();
        inputCabin.Enable();
        inputLift.Enable();
    }
    public override void OnEpisodeBegin()
    {
        level = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("level_parameter", 0);
        if (level == -1) Utils.StopSimulation();
        if (level == 0)
        {
            MoveCraneToPosition(new Vector3(Random.Range(-8, 8), -4.5f, Random.Range(-8, 8)));
            target.localPosition = new Vector3(Random.Range(-8, 8), 1.5f, Random.Range(-8, 8));
            craneMovementEnabled = true;
            cabinMovementEnabled = true;
            swingEnabled = false;
            winchMovementEnabled = false;
        }
        if (level == 1)
        {
            MoveCraneToPosition(new Vector3(Random.Range(-8, 8), -1f, Random.Range(-8, 8)));
            target.localPosition = new Vector3(Random.Range(-8, 8), 1.5f, Random.Range(-8, 8));
            craneMovementEnabled = true;
            cabinMovementEnabled = true;
            swingEnabled = false;
            winchMovementEnabled = true;
        }
        if (level == 2)
        {
            MoveCraneToPosition(new Vector3(Random.Range(-8, 8), -1f, Random.Range(-8, 8)));
            target.localPosition = new Vector3(Random.Range(-8, 8), 1.5f, Random.Range(-8, 8));
            craneMovementEnabled = true;
            cabinMovementEnabled = true;
            swingEnabled = true;
            winchMovementEnabled = true;
        }
        if (level == 3)
        {
            MoveCraneToPosition(new Vector3(Random.Range(-8, 8), -1f, Random.Range(-8, 8)));
            target.localPosition = new Vector3(Random.Range(-8, 8), 1.5f, Random.Range(-8, 8));
            craneMovementEnabled = true;
            cabinMovementEnabled = true;
            swingEnabled = true;
            winchMovementEnabled = true;
        }
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
        craneValue = continousActions[0];
        cabinValue = continousActions[1];
        winchValue = continousActions[2];
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Utils.Normalize(crane.localPosition.x, -10, 10));
        sensor.AddObservation(Utils.Normalize(rbCrane.velocity.x, -10, 10));

        sensor.AddObservation(Utils.Normalize(crane.localPosition.z, -10, 10));
        sensor.AddObservation(Utils.Normalize(rbCrane.velocity.z, -10, 10));

        sensor.AddObservation(Utils.Normalize(spreader.localPosition.x, -10, 10));
        sensor.AddObservation(Utils.Normalize(spreader.localPosition.z, -10, 10));
        sensor.AddObservation(Utils.Normalize(spreader.localPosition.y, 0, 10));

        sensor.AddObservation(Utils.Normalize(rbSpreader.velocity.x, -10, 10));
        sensor.AddObservation(Utils.Normalize(rbSpreader.velocity.z, -10, 10));
        sensor.AddObservation(Utils.Normalize(rbSpreader.velocity.y, -10, 10));

        sensor.AddObservation(Utils.Normalize(target.localPosition.x, -10, 10));
        sensor.AddObservation(Utils.Normalize(target.localPosition.z, -10, 10));
        sensor.AddObservation(Utils.Normalize(target.localPosition.y, 0, 10));

        sensor.AddObservation(craneValue);
        sensor.AddObservation(cabinValue);
        sensor.AddObservation(winchValue);

    }
    private void FixedUpdate()
    {
        MoveCabin(cabinValue);
        MoveCrane(craneValue);
        MoveWinch(winchValue);
    }
    private void Update()
    {
        if (!swingEnabled) spreader.localPosition = new Vector3(0, spreader.localPosition.y, 0);
        if (swingEnabled)
        {
            if (rbCrane.velocity.x == 0 && rbCrane.velocity.z == 0) return;

            float swingDistance = Vector2.Distance(
                new Vector2(spreader.localPosition.x, spreader.localPosition.z),
                new Vector2(crane.localPosition.x, crane.localPosition.z));

            if (swingDistance < 1)
            {
                AddReward(1f / MaxStep);
            }
        }
    }
    private void MoveCabin(float val)
    {
        if (!cabinMovementEnabled) return;
        float targetSpeed = movementManager.GetNextSpeed(val, rbCabin.velocity.z, 4f * 0.25f * Time.fixedDeltaTime, 4f);
        Vector3 newVelocity = rbCabin.velocity;
        newVelocity.z = targetSpeed;
        rbCabin.velocity = newVelocity;
    }
    private void MoveCrane(float val)
    {
        if (!craneMovementEnabled) return;
        float targetSpeed = movementManager.GetNextSpeed(val, rbCrane.velocity.x, 0.75f * 0.25f * Time.fixedDeltaTime, 0.75f);
        //targetSpeed = 0.75f;
        Vector3 newVelocity = rbCrane.velocity;
        newVelocity.x = targetSpeed;
        rbCrane.velocity = newVelocity;
    }
    private void MoveWinch(float value)
    {
        if (!winchMovementEnabled) return;
        if (value > 0 && spreader.localPosition.y > -1f) value = 0;
        if (value < 0 && spreader.localPosition.y < - 20) value = 0;

        // Adjust the value since the value provided is the speed in m/s
        // The motor target velocity is in degree/s
        // Since our pulleys have a diameter of 1m we want 1 rotation/pi per m/s requested.
        if (value != 0) value *= 360 / Mathf.PI;

        // Get 2 motors (one side turns clockwise while the other side turns counter-clockwise
        JointMotor motor = spool.motor;

        float currentVelocity = motor.targetVelocity;
        float acceleration = 360 * Time.fixedDeltaTime;

        if (value != 0 && value > currentVelocity)
        {
            value = Mathf.Min(currentVelocity + acceleration, value);
        }
        else if (value != 0 && value < currentVelocity)
        {
            value = Mathf.Max(currentVelocity - acceleration, value);
        }

        if (value == 0 && currentVelocity > 0)
        {
            value = Mathf.Max(currentVelocity - acceleration, value);
        }
        else if (value == 0 && currentVelocity < 0)
        {
            value = Mathf.Min(currentVelocity + acceleration, value);
        }

        motor.targetVelocity = value;

        spool.motor = motor;
    }
    private void MoveCraneToPosition(Vector3 position)
    {
        crane.localPosition = new Vector3(position.x, 6, position.z);
        spreader.localPosition = new Vector3(0, position.y, 0);

        rbCabin.velocity = Vector3.zero;
        rbSpreader.velocity = Vector3.zero;


        cable.Setup();

        Filo.Cable.Link link = cable.links[0];
        link.orientation = false;
        link.storedCable = 50;
        cable.links[0] = link;
    }
    public void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("dead"))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
    public void OnCollisionStay(Collision col)
    {
        throw new System.NotImplementedException();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (level == 0 && other.CompareTag("target"))
        {
            AddReward(+1f);
            EndEpisode();
        }
        if (level == 3) enterTime = Time.time;
    }
    public void OnTriggerStay(Collider other)
    {
        if (level == 3)
        {
            if (Time.time - enterTime > stayTime)
            {
                stayTime += 0.01f;
                enterTime = 0;
                AddReward(1f);
                EndEpisode();
            }
            else
            {
                AddReward(1f / MaxStep);
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "dead")
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

}