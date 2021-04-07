using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crane : MonoBehaviour
{
    private HingeJoint spool1;
    private HingeJoint spool2;
    private HingeJoint spool3;
    private HingeJoint spool4;

    public Kat Kat { get; private set; }
    public Spreader Spreader { get; private set; }

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        Kat = GetComponentInChildren<Kat>();
        Spreader = GetComponentInChildren<Spreader>();

        HingeJoint[] joints = GetComponentsInChildren<HingeJoint>();
        spool1 = joints[0];
        spool2 = joints[2];
        spool3 = joints[1];
        spool4 = joints[3];

        Rigidbody rb = Spreader.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        new WaitForSeconds(0.001f);
        rb.isKinematic = false;
    }

    public void Hijs(float value)
    {
        if (value > 0 && Spreader.transform.localPosition.y > Kat.transform.localPosition.y - 5) value = 0;

        // Adjust the value since the value provided is the speed in m/s
        // The motor target velocity is in degree/s
        // Since our pulleys have a diameter of 1m we want 1 rotation/pi per m/s requested.
        value *= 360 / Mathf.PI;

        // Get 2 motors (one side turns clockwise while the other side turns counter-clockwise
        JointMotor motor = spool1.motor;
        JointMotor motor2 = spool2.motor;

        float currentVelocity = motor.targetVelocity;
        float acceleration = 360 * Time.deltaTime;

        if (value > currentVelocity && value != 0)
        {
            value = Mathf.Min(currentVelocity + acceleration, value);
        }
        else if (value < currentVelocity && value != 0)
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
        motor2.targetVelocity = -value;

        spool1.motor = motor;
        spool2.motor = motor;
        spool3.motor = motor2;
        spool4.motor = motor2;
    }

    internal void ResetToRandomPosition()
    {
        Kat.transform.localPosition = new Vector3(Random.Range(-15, 15), 15, Random.Range(-15, 15));
        Spreader.transform.localPosition = new Vector3(Kat.transform.localPosition.x, 7.5f, Kat.transform.localPosition.z + 1.75f);
        Rigidbody rb = Spreader.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        new WaitForSeconds(0.001f);
        rb.isKinematic = false;
    }

    internal void ResetToPosition(Vector3 katPosition, float spreaderHeight)
    {
        Kat.transform.localPosition = katPosition;
        Spreader.transform.localPosition = new Vector3(Kat.transform.localPosition.x, spreaderHeight, Kat.transform.localPosition.z + 1.75f);
        Rigidbody rb = Spreader.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        new WaitForSeconds(0.001f);
        rb.isKinematic = false;
    }
}
