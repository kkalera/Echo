using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinchManager : MonoBehaviour
{
    [SerializeField] public HingeJoint spoolLeft;
    [SerializeField] public HingeJoint spoolRight;
    [SerializeField] public Filo.Cable cableLeft;
    [SerializeField] public Filo.Cable cableRight;

    void Update()
    {
        MoveWinch(0);
        if (Input.GetKey(KeyCode.DownArrow)) MoveWinch(-1);
        if (Input.GetKey(KeyCode.UpArrow)) MoveWinch(1);
        
    }public void MoveWinch(float value)
    {
        // Adjust the value since the value provided is the speed in m/s
        // The motor target velocity is in degree/s
        // Since our pulleys have a diameter of 1m we want 1 rotation/pi per m/s requested.
        if (value != 0) value *= 360 / Mathf.PI;

        // Get 2 motors (one side turns clockwise while the other side turns counter-clockwise
        JointMotor motor = spoolLeft.motor;
        JointMotor motor2 = spoolRight.motor;

        float currentVelocity = motor.targetVelocity;
        //float acceleration = 360 * Time.deltaTime * Time.timeScale;
        float acceleration = 360 * Time.deltaTime * 100;

        if (value != 0 && value > currentVelocity)
        {
            value = Mathf.Min(currentVelocity + acceleration, value);
        }
        else if (value != 0 && value < currentVelocity)
        {
            value = Mathf.Max(currentVelocity - acceleration, value);
        }

        if (Mathf.Approximately(value, 0) && currentVelocity > 0)
        {
            value = Mathf.Max(currentVelocity - acceleration, value);
        }
        else if (Mathf.Approximately(value, 0) && currentVelocity < 0)
        {
            value = Mathf.Min(currentVelocity + acceleration, value);
        }

        motor.targetVelocity = value;
        motor2.targetVelocity = -value;

        spoolLeft.motor = motor;
        spoolRight.motor = motor;
    }
}
