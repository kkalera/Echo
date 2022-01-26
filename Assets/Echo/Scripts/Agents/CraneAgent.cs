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
        [SerializeField] Crane crane;
        [SerializeField] private bool autoPilot;

        private static readonly int katIndex = 0;
        private static readonly int katForwardValue = 1;
        private static readonly int katBackwardsValue = 2;
        private static readonly int winchIndex = 1;
        private static readonly int winchUpValue = 1;
        private static readonly int winchDownValue = 2;

        private void Start()
        {
            env.InitializeEnvironment();
        }
        public override void OnEpisodeBegin()
        {
            env.OnEpisodeBegin();
            crane.ResetPosition();
        }
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            /*var disActions = actionsOut.DiscreteActions;
            if (Input.GetKey(KeyCode.Z)) disActions[katIndex] = katForwardValue;
            if (Input.GetKey(KeyCode.S)) disActions[katIndex] = katBackwardsValue;
            if (Input.GetKey(KeyCode.UpArrow)) disActions[winchIndex] = winchUpValue;
            if (Input.GetKey(KeyCode.DownArrow)) disActions[winchIndex] = winchDownValue;*/
            var conActions = actionsOut.ContinuousActions;
            if (Input.GetKey(KeyCode.Z)) conActions[katIndex] = 1;
            if (Input.GetKey(KeyCode.S)) conActions[katIndex] = -1;
            if (!Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.S)) conActions[katIndex] = 0;

            if (Input.GetKey(KeyCode.UpArrow)) conActions[winchIndex] = 1;
            if (Input.GetKey(KeyCode.DownArrow)) conActions[winchIndex] = -1;
            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) conActions[winchIndex] = 0;

            if (autoPilot)
            {
                var inputs = GetInputs(env.TargetWorldPosition, crane.craneSpecs.spreaderWorldPosition, crane.katBody.velocity, crane.craneSpecs.katAcceleration / crane.craneSpecs.katMaxSpeed);                
                conActions[katIndex] = inputs.z;
                conActions[winchIndex] = inputs.y;
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            /*
            // Kat movement
            int katAction = actions.DiscreteActions[katIndex];
            switch (katAction)
            {
                case 1:
                    crane.MoveKat(1);
                    break;
                case 2:
                    crane.MoveKat(-1);
                    break;
                default:
                    crane.MoveKat(0);
                    break;
            }

            // Winch Movement
            int winchAction = actions.DiscreteActions[winchIndex];
            switch (winchAction)
            {
                case 1:
                    crane.MoveWinch(1);
                    break;
                case 2:
                    crane.MoveWinch(-1);
                    break;
                default:
                    crane.MoveWinch(0);
                    break;
            }
            */

            float katAction = actions.ContinuousActions[katIndex];
            crane.MoveKat(katAction);

            float winchAction = actions.ContinuousActions[winchIndex];
            crane.MoveWinch(winchAction);

            // Get the state after interaction
            State state = env.Step();
            AddReward(state.reward);
            if (state.dead) EndEpisode();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(crane.craneSpecs.katWorldPosition);
            sensor.AddObservation(crane.craneSpecs.spreaderWorldPosition);
            sensor.AddObservation(env.TargetWorldPosition);
        }

        public static Vector3 GetInputs(Vector3 targetPosition, Vector3 spreaderPosition, Vector3 currentSpeed, float acceleration)
        {

            Vector3 inputs = new Vector3(0, 0, 0);
            targetPosition = GetNextPosition(spreaderPosition, targetPosition);

            float distanceToTravelY = Mathf.Abs(targetPosition.y - spreaderPosition.y);
            float inputY = distanceToTravelY / (Mathf.Max(Mathf.Abs(currentSpeed.y), 0.05f) / acceleration);

            float distanceToTravelZ = Mathf.Abs(targetPosition.z - spreaderPosition.z);
            float inputZ = distanceToTravelZ / (Mathf.Max(Mathf.Abs(currentSpeed.z), 0.05f) / acceleration);

            if (targetPosition.y < spreaderPosition.y) inputY = -inputY;
            if (targetPosition.z < spreaderPosition.z) inputZ = -inputZ;

            if (float.IsNaN(inputY)) inputY = 0;
            if (float.IsNaN(inputZ)) inputZ = 0;

            inputs.y = Mathf.Clamp(inputY, -1, 1);
            inputs.z = Mathf.Clamp(inputZ, -1, 1);

            return inputs;

        }
        private static Vector3 GetNextPosition(Vector3 spreaderPosition, Vector3 targetPosition)
        {
            bool hasToCrossLeg = spreaderPosition.z > 10.5f && targetPosition.z < 10.5f;
            if (!hasToCrossLeg) hasToCrossLeg = spreaderPosition.z > -10.5f && targetPosition.z < -10.5f;
            if (!hasToCrossLeg) hasToCrossLeg = ((spreaderPosition.z > -10.5f && spreaderPosition.z < 10.5f) &&
                    (targetPosition.z > 10.5f || targetPosition.z < -10.5f));


            // Check if we're to far from the target to lower the spreader        
            float r = (spreaderPosition.y * 0.2f) + 1;
            if (spreaderPosition.y < 17 && Mathf.Abs(spreaderPosition.z - targetPosition.z) > r && hasToCrossLeg)
            {
                //targetPosition = new Vector3(0, 25f, spreaderPosition.z);
                targetPosition = new Vector3(0, 25f, 4);
            }
            else if (spreaderPosition.y >= 17 && Mathf.Abs(spreaderPosition.z - targetPosition.z) > r)
            {
                targetPosition = new Vector3(0, spreaderPosition.y, targetPosition.z);
            }


            if (Mathf.Abs(spreaderPosition.y - targetPosition.y) > 1 && spreaderPosition.z > 4 && Mathf.Abs(spreaderPosition.z - targetPosition.z) < r) targetPosition.z -= 0.75f;
            if (Mathf.Abs(spreaderPosition.z - targetPosition.z) > r &&
                Mathf.Abs(spreaderPosition.y - targetPosition.y) < 2 &&
                spreaderPosition.y < 19) targetPosition.y += 0.5f;

            return targetPosition;
        }
    }
}