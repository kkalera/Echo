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
                var inputs = GetInputs(env.TargetPosition - env.transform.position + new Vector3(0,2.75f,0),
                    env.Crane.spreader.Position - env.transform.position,
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
            if (state.dead) EndEpisode();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            env.CollectObservations(sensor);       
        }

        public static Vector3 GetInputs(Vector3 targetPosition, Vector3 spreaderPosition, Vector3 currentSpeed, Vector3 acceleration)
        {
            Vector3 inputs = new Vector3(0, 0, 0);
            targetPosition = GetNextPosition(spreaderPosition, targetPosition);

            ///// Z movement
            float distanceZ = Mathf.Abs(spreaderPosition.z - targetPosition.z);
            if (!Mathf.Approximately(distanceZ, 0))
            {
                float vel = Mathf.Abs(currentSpeed.z);
                float d = Mathf.Pow(vel, 2) / (2 * acceleration.z);
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
            
            float craneZLegs = 12;
            bool hasToCrossLeg = spreaderPosition.z > craneZLegs && targetPosition.z < craneZLegs;

            if (!hasToCrossLeg) hasToCrossLeg = spreaderPosition.z > -craneZLegs && targetPosition.z < -craneZLegs;
            if (!hasToCrossLeg) hasToCrossLeg = ((spreaderPosition.z > -craneZLegs && spreaderPosition.z < craneZLegs) &&
                    (targetPosition.z > craneZLegs || targetPosition.z < -craneZLegs));
            
            // Check if we're to far from the target to lower the spreader        
            float r = (spreaderPosition.y * 0.2f) + 1;
            
            if (spreaderPosition.y < 17 && hasToCrossLeg)
            {                
                targetPosition = new Vector3(0, 25f, spreaderPosition.z);
            }
            else if (spreaderPosition.y >= 17 && Mathf.Abs(spreaderPosition.z - targetPosition.z) > r)
            {
                targetPosition = new Vector3(0, spreaderPosition.y, targetPosition.z);
            }


            //if (Mathf.Abs(spreaderPosition.y - targetPosition.y) > 1 && spreaderPosition.z > 4 && Mathf.Abs(spreaderPosition.z - targetPosition.z) < r) targetPosition.z -= 0.75f;
            //if (Mathf.Abs(spreaderPosition.z - targetPosition.z) > r && Mathf.Abs(spreaderPosition.y - targetPosition.y) < 2 && spreaderPosition.y < 19) targetPosition.y += 0.5f;

            return targetPosition;
        }
    }
}