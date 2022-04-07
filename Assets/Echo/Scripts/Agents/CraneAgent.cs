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
        [SerializeField] private bool heuristicController;
        [SerializeField] private bool useAutopilotRewards;

        private static readonly int katIndex = 0;
        private static readonly int winchIndex = 1;

        private float autoPilotKat = 0;
        private float autoPilotWinch = 0;

        private void Start()
        {
            env.InitializeEnvironment();
            env.MaxStep = Mathf.Max(1,MaxStep);
        }
        public override void OnEpisodeBegin()
        {            
            env.OnEpisodeBegin();
            useAutopilotRewards = Academy.Instance.EnvironmentParameters.GetWithDefault("useAutopilotRewards", -1) > 0;

        }
        public override void Heuristic(in ActionBuffers actionsOut)
        {            
            var Actions = actionsOut.ContinuousActions;

            if (!autoPilot)
            {
                if (Input.GetKey(KeyCode.Z)) Actions[katIndex] = 1;
                if (Input.GetKey(KeyCode.S)) Actions[katIndex] = -1;
                if (!Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.S)) Actions[katIndex] = 0;

                if (Input.GetKey(KeyCode.UpArrow)) Actions[winchIndex] = 1;
                if (Input.GetKey(KeyCode.DownArrow)) Actions[winchIndex] = -1;
                if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) Actions[winchIndex] = 0;
            }


            Dictionary<string, float> obs = env.CollectObservations();

            Vector3 inputs = Vector3.zero;
            if (!env.NormalisedObservations)
            {
                inputs = GetInputs
                (
                new Vector3(obs["targetX"], obs["targetY"], obs["targetZ"]),
                new Vector3(obs["spreaderX"], obs["spreaderY"], obs["spreaderZ"]),
                new Vector3(obs["spreaderVelX"], obs["spreaderVelY"], obs["katVel"]),
                new Vector3(0, env.Crane.craneSpecs.winchAcceleration, env.Crane.craneSpecs.katAcceleration)
                );                
            }
            else
            {
                Bounds bounds = env.transform.GetComponent<BoxCollider>().bounds;
                float xmin = bounds.center.x - bounds.extents.x;
                float xmax = bounds.center.x + bounds.extents.x;
                float ymin = bounds.center.y - bounds.extents.y;
                float ymax = bounds.center.y + bounds.extents.y;
                float zmin = bounds.center.z - bounds.extents.z;
                float zmax = bounds.center.z + bounds.extents.z;

                inputs = GetInputs
                (
                new Vector3(Utils.DeNormalize(obs["targetX"], xmin, xmax) - transform.parent.position.x, 
                            Utils.DeNormalize(obs["targetY"], ymin, ymax) - transform.parent.position.y, 
                            Utils.DeNormalize(obs["targetZ"], zmin, zmax) - transform.parent.position.z),

                new Vector3(Utils.DeNormalize(obs["spreaderX"], xmin, xmax) - transform.parent.position.x,
                            Utils.DeNormalize(obs["spreaderY"], ymin, ymax) - transform.parent.position.y, 
                            Utils.DeNormalize(obs["spreaderZ"], zmin, zmax) - transform.parent.position.z),

                new Vector3(0, env.Crane.spreader.Rbody.velocity.y, env.Crane.kat.Velocity),
                new Vector3(0, env.Crane.craneSpecs.winchAcceleration, env.Crane.craneSpecs.katAcceleration)
                );
            }

            // Used for autopilot reward system
            autoPilotKat = inputs.z;
            autoPilotWinch = inputs.y;

            if (autoPilot)
            {
                Actions[katIndex] = inputs.z;
                Actions[winchIndex] = inputs.y;                
            }            
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float katAction = Mathf.Clamp(actions.ContinuousActions[katIndex], -1, 1);
            env.Crane.MoveKat(katAction);

            float winchAction = Mathf.Clamp(actions.ContinuousActions[winchIndex], -1, 1);
            env.Crane.MoveWinch(winchAction);

            // Get the state after interaction
            State state = env.State();
            AddReward(state.reward);
            if (useAutopilotRewards) AddReward((1-Mathf.Abs(actions.ContinuousActions[katIndex] - autoPilotKat)) / MaxStep);
            if (useAutopilotRewards) AddReward((1-Mathf.Abs(actions.ContinuousActions[winchIndex] - autoPilotWinch)) / MaxStep);
            if (state.dead) EndEpisode();

        }

        public override void CollectObservations(VectorSensor sensor)
        {
            var obs = env.CollectObservations();
            sensor.AddObservation(obs["spreaderX"]);
            sensor.AddObservation(obs["spreaderY"]);
            sensor.AddObservation(obs["spreaderZ"]);
            sensor.AddObservation(obs["spreaderVelX"]);
            sensor.AddObservation(obs["spreaderVelY"]);
            sensor.AddObservation(obs["spreaderVelZ"]);
            sensor.AddObservation(obs["spreaderRotaX"]);
            sensor.AddObservation(obs["spreaderRotaY"]);
            sensor.AddObservation(obs["spreaderRotaZ"]);
            sensor.AddObservation(obs["kat"]);
            sensor.AddObservation(obs["katVel"]);
            sensor.AddObservation(obs["targetX"]);
            sensor.AddObservation(obs["targetY"]);
            sensor.AddObservation(obs["targetZ"]);
        }

        private Vector3 GetInputs(Vector3 targetPosition, Vector3 spreaderPosition, Vector3 currentSpeed, Vector3 acceleration)
        {
            
            Vector3 inputs = Vector3.zero;
            
            targetPosition = env.GetNextPosition(spreaderPosition, targetPosition);

            ///// Z movement
            float distanceZ = Mathf.Abs(spreaderPosition.z - targetPosition.z);
            if (!Mathf.Approximately(distanceZ, 0))
            {
                float vel = Mathf.Abs(currentSpeed.z);
                float d = Mathf.Pow(vel, 2) / (2 * acceleration.z);
                inputs.z = distanceZ - d;

                if (targetPosition.z < spreaderPosition.z)
                {
                    inputs.z = Mathf.Clamp(-inputs.z,-1,0);
                }
                else
                {
                    inputs.z = Mathf.Clamp(inputs.z, 0, 1);
                }
                
                //inputs.z = Mathf.Clamp(inputs.z, -1, 1);
            }
            /////

            ///// Y movement
            float distanceY = Mathf.Abs(spreaderPosition.y - targetPosition.y);
            if (!Mathf.Approximately(distanceY, 0))
            {
                float vel = Mathf.Abs(currentSpeed.y);
                float d = Mathf.Pow(vel, 2) / (2 * acceleration.y);
                inputs.y = distanceY - d;                
                if (targetPosition.y < spreaderPosition.y)
                {
                    inputs.y = Mathf.Clamp(-inputs.y, -1, 0);
                }
                else
                {
                    inputs.y = Mathf.Clamp(inputs.y, 0, 1);
                }
                
            }
            /////            
            return inputs;
        }
    }
}