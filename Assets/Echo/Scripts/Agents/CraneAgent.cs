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
            var disActions = actionsOut.DiscreteActions;
            if (Input.GetKey(KeyCode.Z)) disActions[katIndex] = katForwardValue;
            if (Input.GetKey(KeyCode.S)) disActions[katIndex] = katBackwardsValue;
            if (Input.GetKey(KeyCode.UpArrow)) disActions[winchIndex] = winchUpValue;
            if (Input.GetKey(KeyCode.DownArrow)) disActions[winchIndex] = winchDownValue;
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
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
    }
}