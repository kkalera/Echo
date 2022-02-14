using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace Echo
{
    /// <summary>
    /// Environment that spawns a single container, which the agent needs to grab
    /// </summary>
    public class EnvGrabContainer : Environment
    {
        public static int katIndex = 0;
        public static int winchIndex = 1;
        [TagsAndLayers.TagDropdown] public string tagDead;
        [TagsAndLayers.TagDropdown] public string tagContainer;
        [SerializeField] private Crane _crane;
        [SerializeField] private GameObject containerPrefab;
        [SerializeField] private SoCollision collisionManager;
        private Container _container;
        
        void Start()
        {
            Crane = _crane;
        }

        public override void InitializeEnvironment()
        {
            // Spawn the container
            var c = Instantiate(containerPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), transform);
            _container = c.GetComponent<Container>();
        }

        public override void OnEpisodeBegin()
        {
            collisionManager.Reset();

            // Reset crane position
            Crane.ResetPosition(new Vector3(0, 15, 25));

            // Reset container position
            _container.ResetPosition(Vector3.zero);

            // Set the target as the container position
            TargetPosition = _container.transform.position + new Vector3(0, 2.75f, 0); 

        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(Crane.spreader.Position);
            sensor.AddObservation(Crane.spreader.Rbody.velocity);
            sensor.AddObservation(Crane.spreader.Rotation);
            sensor.AddObservation(Crane.kat.Position);
            sensor.AddObservation(Crane.kat.Velocity);
            sensor.AddObservation(TargetPosition);
        }

        public override void TakeActions(ActionBuffers actions)
        {
            Crane.MoveKat(actions.ContinuousActions[katIndex]);
            Crane.MoveWinch(actions.ContinuousActions[winchIndex]);
        }

        public override State State()
        {
            if(collisionManager.collided && collisionManager._collision.collider.CompareTag(tagContainer)) return new State(1f, true);

            if ((collisionManager.collided && collisionManager._collision.collider.CompareTag(tagDead))
                || (collisionManager.triggered && collisionManager.triggered_collider.CompareTag(tagDead)))
            {
                return new State(-1f, true);
            }
            
            return new State(1f / MaxStep, false);
        }
    }
}