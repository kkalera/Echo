using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace Echo
{
    public class CraneAgent : Agent
    {
        [SerializeField] Environment env;
        [SerializeField] private bool autoPilot;        

        private static readonly int katIndex = 0;
        private static readonly int winchIndex = 1;        

        private void Start()
        {
            env.InitializeEnvironment();
            env.MaxStep = Mathf.Max(1,MaxStep);
        }
        public override void OnEpisodeBegin()
        {
            env.OnEpisodeBegin();
            env.MaxStep = Mathf.Max(1, MaxStep);
        }        
        public override void Heuristic(in ActionBuffers actionsOut)
        {   
            var conActions = actionsOut.ContinuousActions;
            if (Input.GetKey(KeyCode.Z)) conActions[katIndex] = 1;
            if (Input.GetKey(KeyCode.S)) conActions[katIndex] = -1;
            if (!Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.S)) conActions[katIndex] = 0;

            if (Input.GetKey(KeyCode.UpArrow)) conActions[winchIndex] = 1;
            if (Input.GetKey(KeyCode.DownArrow)) conActions[winchIndex] = -1;
            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) conActions[winchIndex] = 0;            
            
            if (autoPilot)
            {
                var inputs = GetInputs(env.TargetPosition,
                    env.Crane.spreader.Position,
                    new Vector3(0,env.Crane.spreader.Rbody.velocity.y, env.Crane.kat.Velocity),
                    new Vector3(0, env.Crane.craneSpecs.winchAcceleration,
                    env.Crane.craneSpecs.katAcceleration));

                conActions[katIndex] = inputs.z;
                conActions[winchIndex] = inputs.y;
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float katAction = actions.ContinuousActions[katIndex];
            env.Crane.MoveKat(katAction);

            float winchAction = actions.ContinuousActions[winchIndex];
            env.Crane.MoveWinch(winchAction);

            // Get the state after interaction
            State state = env.State();
            AddReward(state.reward);
            //if (state.dead) Debug.Log(StepCount);
            if (state.dead) EndEpisode();
            //Utils.ClearLogConsole();
            //Debug.Log(GetCumulativeReward());
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            env.CollectObservations(sensor);
        }

        public Vector3 GetInputs(Vector3 targetPosition, Vector3 spreaderPosition, Vector3 currentSpeed, Vector3 acceleration)
        {

            Vector3 inputs = new Vector3(0, 0, 0);
            targetPosition = GetNextPosition(spreaderPosition, targetPosition);

            ///// Z movement
            float distanceZ = Mathf.Abs(spreaderPosition.z - targetPosition.z);
            if (!Mathf.Approximately(distanceZ, 0))
            {
                float vel = Mathf.Abs(currentSpeed.z);
                float d = Mathf.Pow(vel, 2) / (2 * Mathf.Abs(acceleration.z));
                inputs.z = distanceZ - d;

                if (targetPosition.z < spreaderPosition.z) inputs.z = -inputs.z;
                inputs.z = Mathf.Clamp(inputs.z, -1, 1);
            }
            /////

            ///// Y movement
            float distanceY = Mathf.Abs(spreaderPosition.y - targetPosition.y);
            if (!Mathf.Approximately(distanceY, 0))
            {
                float vel = Mathf.Abs(currentSpeed.y);
                float d = Mathf.Pow(vel, 2) / (2 * acceleration.y);
                inputs.y = distanceY - d;
                
                if (targetPosition.y < spreaderPosition.y) inputs.y = -inputs.y;
                inputs.y = Mathf.Clamp(inputs.y, -1, 1);
            }
            /////

            return inputs;

        }
        public static Vector3 GetInputsSwing(Vector3 targetPosition,Vector3 spreaderVelocity,
            Vector3 spreaderPosition, Vector3 katVelocity, Vector3 katPosition, Vector3 acceleration)
        {

            Vector3 inputs = new Vector3(0, 0, 0);
            targetPosition = GetNextPosition(spreaderPosition, targetPosition);

            ///// Z movement
            float distanceZ = Mathf.Abs(spreaderPosition.z - targetPosition.z);
            if (!Mathf.Approximately(distanceZ, 0))
            {
                // Swing management
                // Acceleration: limit the acceleration so that the swing created does not exceed the
                // distance the kat can travel in the same time

                // Calculate the distance that the pendulum (spreader) will swing based on current speed and position
                // Formula for the height a pendulum will reach: h = L - L * cos(a)
                // h: height
                // L: length of the pendulum
                // a: angle from vertical
                float sa = Mathf.Abs(spreaderPosition.y - katPosition.y);
                float sb = Mathf.Abs(spreaderPosition.z - katPosition.z);
                float a = Mathf.Atan(sb / sa) * Mathf.Rad2Deg;
                float l = Vector3.Distance(katPosition, spreaderPosition);
                float h = l - l * Mathf.Cos(a);
                
                
            }
            /////

            ///// Y movement
            
            /////

            return inputs;

        }
        private static Vector3 GetNextPosition(Vector3 spreaderPosition, Vector3 targetPosition)
        {
            float craneZLegs = 13;
            float legThickness = 8;
            float clearancHeight = 17;
            int whereToCross = 0;

            // Above ship and target between legs or behind legs
            whereToCross = (spreaderPosition.z > craneZLegs-legThickness && targetPosition.z < craneZLegs) ? 1 : whereToCross;
            // Behind legs and target in front or between legs
            whereToCross = (spreaderPosition.z < -craneZLegs+legThickness && targetPosition.z > -craneZLegs) ? 2 : whereToCross;
            // Between legs and target is in front
            whereToCross = ((spreaderPosition.z > -craneZLegs + legThickness && spreaderPosition.z < craneZLegs - legThickness))
                && targetPosition.z > craneZLegs ? 3 : whereToCross;
            // Between legs and target is behind
            whereToCross = ((spreaderPosition.z > -craneZLegs + legThickness && spreaderPosition.z < craneZLegs - legThickness))
                && targetPosition.z < -craneZLegs ? 4 : whereToCross;
                    
            
            
            // Check if we're to far from the target to lower the spreader        
            float r = (spreaderPosition.y * 0.2f) + 1;
            bool insideR = Mathf.Abs(spreaderPosition.z - targetPosition.z) < r;
            if (!insideR)
            {
                if (spreaderPosition.y > clearancHeight)
                {
                    targetPosition = new Vector3(targetPosition.x, spreaderPosition.y, targetPosition.z);
                }
                else
                {
                    float distanceToClearance = Mathf.Abs(spreaderPosition.y - clearancHeight) * 0.01f;                    
                    switch (whereToCross)
                    {
                        case 1: targetPosition = new Vector3(targetPosition.x, clearancHeight + 3, Mathf.Max(craneZLegs, spreaderPosition.z - distanceToClearance)); break;
                        case 2: targetPosition = new Vector3(targetPosition.x, clearancHeight + 3, Mathf.Min(-craneZLegs, spreaderPosition.z + distanceToClearance)); break;
                        case 3: targetPosition = new Vector3(targetPosition.x, clearancHeight + 3, Mathf.Min(craneZLegs - legThickness, spreaderPosition.z + distanceToClearance)); break;
                        case 4: targetPosition = new Vector3(targetPosition.x, clearancHeight + 3, Mathf.Max(-craneZLegs + legThickness, spreaderPosition.z - distanceToClearance)); break;
                        default: break;
                    }
                }
            }
            return targetPosition;
        }
    }
}