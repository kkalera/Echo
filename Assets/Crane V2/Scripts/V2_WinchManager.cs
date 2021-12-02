using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace echo {
    public class V2_WinchManager : MonoBehaviour
    {
        [SerializeField] V2_CraneSpecs _craneSpecs;

        [SerializeField] private HingeJoint spoolLandRight;
        [SerializeField] private HingeJoint spoolLandLeft;
        [SerializeField] private HingeJoint spoolWaterRight;
        [SerializeField] private HingeJoint spoolWaterLeft;

        [SerializeField] private Filo.Cable cableLandRight;
        [SerializeField] private Filo.Cable cableLandLeft;
        [SerializeField] private Filo.Cable cableWaterRight;
        [SerializeField] private Filo.Cable cableWaterLeft;

        public void MoveWinch(float value)
        {
            value *= _craneSpecs.winchSpeed;

            Filo.Cable.Link linkLandLeft = cableLandLeft.links[0];
            if (linkLandLeft.storedCable < 5 && value < 0) value = 0;


            // Adjust the value since the value provided is the speed in m/s
            // The motor target velocity is in degree/s
            // Since our pulleys have a diameter of 1m we want 1 rotation/pi per m/s requested.
            if (value != 0) value *= 360 / Mathf.PI;

            // Get 2 motors (one side turns clockwise while the other side turns counter-clockwise
            JointMotor motor = spoolWaterLeft.motor;
            JointMotor motor2 = spoolLandLeft.motor;

            float currentVelocity = motor.targetVelocity;
            //float acceleration = 360 * Time.deltaTime * Time.timeScale;
            float acceleration = 360 * Time.deltaTime;

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

            spoolLandLeft.motor = motor;
            spoolLandRight.motor = motor2;
            spoolWaterLeft.motor = motor;
            spoolWaterRight.motor = motor2;
        }
    }
}