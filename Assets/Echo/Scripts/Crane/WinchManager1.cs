using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo {    
    public class WinchManager1 : MonoBehaviour
    {
        [SerializeField] SoCraneSpecs _craneSpecs;
        [SerializeField] Crane _crane;
        
        private void FixedUpdate()
        {
            MoveWinch(_crane.winchSpeed);
            //EnforceLimits();
            //SyncSpools();
        }        
        public void MoveWinch(float value)
        {
            //value = CheckLimits(value);
            // Adjust the value since the value provided is the speed in m/s
            // The motor target velocity is in degree/s
            // Every pulley has a diameter of 1 meter.
            // This means that for every rotation, 3.14m of cable is added
            // So 1 degree = 0.00872m of cable released of 1m/s of cable = +-114.6 degree/s
            // Spools have a radius of 2 in the disc settings, this seems to be a bug in Filo cables
            float diameter = 0.5f;
            float degreeToM = 360 / (Mathf.PI * diameter);
            value *= degreeToM * _craneSpecs.winchMaxSpeed;

            JointMotor motor = _crane.winches[0].motor;

            float timeDelta = Time.fixedDeltaTime + 0.02f;
            float accel = (_craneSpecs.winchAcceleration) * degreeToM * timeDelta;
            float deltaV = Mathf.Abs(value - motor.targetVelocity);
            if (accel > deltaV) accel = deltaV;

            if (motor.targetVelocity < value) motor.targetVelocity += accel;
            if (motor.targetVelocity > value) motor.targetVelocity -= accel;

            motor.targetVelocity = Mathf.Clamp(motor.targetVelocity, -_craneSpecs.winchMaxSpeed * degreeToM, _craneSpecs.winchMaxSpeed * degreeToM);

            for (int i = 0; i < _crane.winches.Count; i++)
            {
                JointMotor m = _crane.winches[i].motor;
                m.targetVelocity = motor.targetVelocity;
                _crane.winches[i].motor = m;
            }
        }
        private float CheckLimits(float value)
        {
            if(value > 0)
            {
                float distanceY = _craneSpecs.maxSpreaderHeight - _crane.spreader.Position.y;
                if (!Mathf.Approximately(distanceY, 0))
                {
                    float vel = Mathf.Abs(_crane.spreader.Rbody.velocity.y);
                    float d = Mathf.Pow(vel, 2) / (2 * _craneSpecs.winchAcceleration);
                    value = distanceY - d;
                }
                if(_crane.spreader.Position.y >= _craneSpecs.maxSpreaderHeight)
                {
                    value = 0;
                }
            }else if(value < 0)
            {
                float distanceY = Mathf.Abs(_crane.spreader.Position.y - _craneSpecs.minSpreaderHeight);
                if (!Mathf.Approximately(distanceY, 0))
                {
                    float vel = Mathf.Abs(_crane.spreader.Rbody.velocity.y);
                    float d = Mathf.Pow(vel, 2) / (2 * _craneSpecs.winchAcceleration);
                    value = -distanceY + d;
                    value = Mathf.Clamp(value, -1, 0);
                }
            }

            return value;
        }
        private void EnforceLimits()
        {
            bool stopWinches;

            stopWinches = _crane.winchSpeed > 0 && _crane.spreader.Position.y > _craneSpecs.maxSpreaderHeight;
            if(!stopWinches)stopWinches = _crane.winchSpeed < 0 && _crane.spreader.Position.y < _craneSpecs.minSpreaderHeight;
            if(!stopWinches)stopWinches = _crane.cables[0].links[0].storedCable <= 1 && _crane.winchSpeed < 0;            

            if (stopWinches)
            {
                for (int i = 0; i < _crane.winches.Count; i++)
                {
                    JointMotor motor = _crane.winches[i].motor;
                    motor.targetVelocity = 0;
                    _crane.winches[i].motor = motor;
                }
            }

        }
    }
}