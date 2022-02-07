using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementTest : MonoBehaviour
{
    [SerializeField] Echo.SoCraneSpecs craneSpecs;
    private bool started;
    private bool slowdown;
    private float startTime;
    private Rigidbody rbody;
    bool startMovement;
    bool resetEnvironment;
    bool autopilot;
    [SerializeField]Vector3 target;

    private void Start()
    { 
        rbody = GetComponent<Rigidbody>();
        ResetEnvironment();        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || started) 
        {
            //startMovement = true;
            startTime = Time.time;
            autopilot = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && !started)
        {
            resetEnvironment = true;
        }
        
    }

    private void FixedUpdate()
    {
        if (startMovement) StartMovementTest();
        if (resetEnvironment) ResetEnvironment();
        MoveTowardsGoal();
    }

    void ResetEnvironment()
    {
        started = false;
        slowdown = false;
        startTime = 0;        
        rbody.velocity = Vector3.zero;
        transform.position = Vector3.zero;
        autopilot = false;
    }
    void StartMovementTest()
    {

        if(!started) startTime = Time.time;

        started = true;

        Echo.Utils.ClearLogConsole();

        if(!slowdown && !Mathf.Approximately(rbody.velocity.z, craneSpecs.katMaxSpeed))
        {
            Echo.Utils.AccelerateRigidbody_Z_Axis(rbody, craneSpecs.katMaxSpeed, craneSpecs.katMaxSpeed, craneSpecs.katAcceleration, Time.fixedDeltaTime);
        }        

        if(slowdown || Mathf.Approximately(rbody.velocity.z, craneSpecs.katMaxSpeed) && !Mathf.Approximately(rbody.velocity.z, 0f))
        {
            slowdown = true;
            Echo.Utils.AccelerateRigidbody_Z_Axis(rbody, -4, craneSpecs.katMaxSpeed, craneSpecs.katAcceleration, Time.fixedDeltaTime);
        }
        
        if(slowdown && Mathf.Approximately(rbody.velocity.z, -4f))
        {
            Debug.Log("Time to accelerate: " + (Time.time - startTime));
            started = false;
            slowdown = false;
        }
    }
    void MoveTowardsGoal()
    {
        if (autopilot)
        {            
            float distance = Mathf.Abs(transform.position.z - target.z);
            if (Mathf.Approximately(distance, 0)) return;

            float vel = Mathf.Abs(rbody.velocity.z);
            float d = Mathf.Pow(vel, 2) / (2 * craneSpecs.katAcceleration);
            float input = distance - d;

            if (target.z < transform.position.z) input = -input;

            MoveBlock(input);

            if (Mathf.Approximately(transform.position.z, target.z)) 
            {
                autopilot = false;
                Debug.Log("Time to accelerate: " + (Time.time - startTime));
            }
        }
    }
    void MoveBlock(float val)
    {
        Echo.Utils.AccelerateRigidbody_Z_Axis(rbody, val * craneSpecs.katMaxSpeed, craneSpecs.katMaxSpeed, craneSpecs.katAcceleration, Time.fixedDeltaTime);
    }
}
