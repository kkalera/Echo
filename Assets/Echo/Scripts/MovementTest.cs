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

    private void Start()
    { 
        rbody = GetComponent<Rigidbody>();
        ResetEnvironment();
    }

    void Update()
    {
        startMovement = (Input.GetKeyDown(KeyCode.UpArrow) || started) ? true : false;
        resetEnvironment = (Input.GetKeyDown(KeyCode.DownArrow) && !started) ? true: false ;
        
    }

    private void FixedUpdate()
    {
        if (startMovement) StartMovementTest();
        if (resetEnvironment) ResetEnvironment();
    }

    void ResetEnvironment()
    {
        started = false;
        slowdown = false;
        startTime = 0;        
        rbody.velocity = Vector3.zero;
        transform.position = Vector3.zero;
    }
    void StartMovementTest()
    {
        if(!started) startTime = Time.time;

        started = true;

        Echo.Utils.ClearLogConsole();

        if(!slowdown && !Mathf.Approximately(rbody.velocity.z, craneSpecs.katMaxSpeed))
        {
            Echo.Utils.AccelerateRigidbodyT(rbody, new Vector3(0,0,craneSpecs.katMaxSpeed), new Vector3(0,0,craneSpecs.katAcceleration), Time.fixedDeltaTime);
            //Echo.Utils.AccelerateRigidbodyT(rbody, new Vector3(0, 0, 1), new Vector3(0, 0, craneSpecs.katAcceleration), Time.fixedDeltaTime);
        }        

        if(slowdown || Mathf.Approximately(rbody.velocity.z, craneSpecs.katMaxSpeed) && !Mathf.Approximately(rbody.velocity.z, 0f))
        {
            slowdown = true;
            Echo.Utils.AccelerateRigidbodyT(rbody, new Vector3(0, 0, -4), new Vector3(0, 0, craneSpecs.katAcceleration), Time.fixedDeltaTime);
        }
        
        if(slowdown && Mathf.Approximately(rbody.velocity.z, -4f))
        {
            Debug.Log("Time to accelerate: " + (Time.time - startTime));
            started = false;
            slowdown = false;
        }
    }
}
