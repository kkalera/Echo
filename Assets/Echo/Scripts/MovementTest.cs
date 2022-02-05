using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementTest : MonoBehaviour
{
    [SerializeField] Echo.SoCraneSpecs craneSpecs;
    private bool started;
    private float startTime;
    private Rigidbody rbody;

    private void Start()
    { 
        rbody = GetComponent<Rigidbody>();
        ResetEnvironment();
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || started)
        {
            StartMovementTest();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && !started)
        {
            ResetEnvironment();
        }
    }

    void ResetEnvironment()
    {
        started = false;
        startTime = 0;        
        rbody.velocity = Vector3.zero;
        transform.position = Vector3.zero;
    }
    void StartMovementTest()
    {
        if(!started) startTime = Time.time;

        started = true;

        Echo.Utils.ClearLogConsole();        

        if(!Mathf.Approximately(rbody.velocity.z, craneSpecs.katMaxSpeed))
        {
            Echo.Utils.AccelerateRigidbodyT(rbody, new Vector3(0,0,craneSpecs.katMaxSpeed), new Vector3(0,0,craneSpecs.katAcceleration), Time.fixedDeltaTime);
        }
        else
        {
            Debug.Log("Time to accelerate: " + (Time.time - startTime));
            started = false;
        }
        
    }
}
