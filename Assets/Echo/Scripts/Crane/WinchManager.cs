using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo {
    [RequireComponent(typeof(HingeJoint))]
    public class WinchManager : MonoBehaviour
    {
        [SerializeField]SoWinchSpeed winchSpeed;
        HingeJoint joint;

        private void Start()
        {
            joint = GetComponent<HingeJoint>();
        }

        void Update()
        {
            MoveWinch(winchSpeed.winchSpeed);

        } public void MoveWinch(float value)
        {
            // Adjust the value since the value provided is the speed in m/s
            // The motor target velocity is in degree/s
            // Since our pulleys have a diameter of 1m we want 1 rotation/pi per m/s requested.
            if (value != 0) value *= 360 / Mathf.PI;

            // Get 2 motors (one side turns clockwise while the other side turns counter-clockwise
            JointMotor motor = joint.motor;

            float currentVelocity = motor.targetVelocity;
            float acceleration = 360 * Time.deltaTime * winchSpeed.maxSpeed;


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
            joint.motor = motor;
        }
    }
}