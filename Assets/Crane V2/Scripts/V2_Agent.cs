using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace echo
{
    public class V2_Agent : Agent, V2_I_CollisionReceiver
    {
        [SerializeField] V2_Crane crane;
        [SerializeField] public GameObject levelObject;
        private V2_ILevel level;
        [SerializeField] bool autoPilot;

        [Header("Cabin input")]        
        [SerializeField] InputAction inputCabin;
        private int _inputCabin;
        
        [Header("Winch input")]
        [SerializeField] InputAction inputWinch;
        private int _inputWinch;



        private void Start()
        {
            inputCabin.Enable();
            inputWinch.Enable();
            crane.Agent = this;
            level = levelObject.GetComponent<V2_ILevel>();
            level.InitializeEnvironment(transform, crane);            
        }

        private void Update()
        {
            _inputCabin = (int)inputCabin.ReadValue<float>();
            _inputWinch = (int)inputWinch.ReadValue<float>();
        }

        public override void OnEpisodeBegin()
        {
            level.ResetEnvironment();

        }
        public override void CollectObservations(VectorSensor sensor)
        {
            AddReward(-1 / MaxStep);
            sensor.AddObservation(crane.Spreader.Position);            
            sensor.AddObservation(level.TargetPosition(transform.position));
        }
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> dActions = actionsOut.DiscreteActions;

            if (_inputCabin > 0){ dActions[0] = 0; }
            else if(_inputCabin < 0){ dActions[0] = 2; }
            else { dActions[0] = 1; }

            if (_inputWinch > 0){ dActions[1] = 0; }
            else if (_inputWinch < 0){ dActions[1] = 2; }
            else { dActions[1] = 1; }

            if (autoPilot)
            {
                Utils.ClearLogConsole();
                
                Vector3 input = AutoPilot.GetInputs(level.TargetPosition(transform.position), crane.Spreader.Position, crane.Cabin.Rbody.velocity, 0.5f, crane.Cabin.Position);
                _inputCabin = (int)input.z;
                _inputWinch = (int)input.y;

                if (_inputCabin > 0) { dActions[0] = 0; }
                else if (_inputCabin < 0) { dActions[0] = 2; }
                else { dActions[0] = 1; }

                if (_inputWinch > 0) { dActions[1] = 0; }
                else if (_inputWinch < 0) { dActions[1] = 2; }
                else { dActions[1] = 1; }
                
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            ActionSegment<int> dActions = actions.DiscreteActions;
            crane.MoveCabin( 1 - dActions[0]);
            crane.MoveWinch( 1 - dActions[1]);
        }

        public void OnCollision(Collision col)
        {            
            var response = level.OnCollision(col);
            AddReward(response.Reward);
            if (response.EndEpisode) EndEpisode();

        }
    }
}