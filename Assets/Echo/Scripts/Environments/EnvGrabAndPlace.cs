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
    public class EnvGrabAndPlace : Environment
    {
        public static int katIndex = 0;
        public static int winchIndex = 1;
        [TagsAndLayers.TagDropdown] public string tagDead;
        [TagsAndLayers.TagDropdown] public string tagContainer;
        [TagsAndLayers.TagDropdown] public string tagTarget;
        [SerializeField] private Crane _crane;        
        [SerializeField] private SoCollision collisionManager;
        [SerializeField] private GameObject containerPrefab;
        [SerializeField] private GameObject targetPrefab;
        [SerializeField][Range(0.05f,1)] private float accuracy;

        private Container _container;
        private GameObject _target;
        private bool grabbed;
        
        void Start()
        {
            Crane = _crane;
        }

        public override void InitializeEnvironment()
        {
            // Spawn the container
            var c = Instantiate(containerPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), transform);
            _container = c.GetComponent<Container>();
            _target = Instantiate(targetPrefab, new Vector3(0,0.1f,25), Quaternion.Euler(Vector3.zero), transform);
        }

        public override void OnEpisodeBegin()
        {
            collisionManager.Reset();

            // Reset crane position
            Crane.ResetPosition(new Vector3(0, 15, 25));

            // Reset container position
            _container.ResetPosition(new Vector3(0,0.1f,0));

            // Set the target as the container position
            TargetPosition = _container.transform.position + new Vector3(0, 2.75f, 0);

            grabbed = false;

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
            // Positive reward
            if (collisionManager.collided && collisionManager._collision.collider.CompareTag(tagContainer))
            {
                float distance = Mathf.Abs(_crane.spreader.Position.z - TargetPosition.z);
                if(distance < accuracy)
                {
                    if (grabbed)
                    {
                        return new State(1f, true);
                    }
                    else
                    {
                        _crane.spreader.GrabContainer(_container.transform);
                        TargetPosition = _target.transform.position;
                        grabbed = true;
                        return new State(1f, false);
                    }
                }
                
            }

            // Dead
            if ((collisionManager.collided && collisionManager._collision.collider.CompareTag(tagDead))
                || (collisionManager.triggered && collisionManager.triggered_collider.CompareTag(tagDead))
                || (_crane.spreader.Rotation.eulerAngles.x > 45 && _crane.spreader.Rotation.eulerAngles.x < 315))
            {
                return new State(-1f, true);
            }
            
            return new State(1f / MaxStep, false);
        }
    }
}