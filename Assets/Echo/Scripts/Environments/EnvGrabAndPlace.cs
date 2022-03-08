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
    public class EnvGrabAndPlace : Environment, CollisionReceiver
    {
        public static int katIndex = 0;
        public static int winchIndex = 1;
        [TagsAndLayers.TagDropdown] public string tagDead;
        [TagsAndLayers.TagDropdown] public string tagContainer;
        [TagsAndLayers.TagDropdown] public string tagTarget;
        [SerializeField] private Crane _crane;        
        [SerializeField] private GameObject containerPrefab;
        [SerializeField] private GameObject targetPrefab;
        [SerializeField][Range(0.05f,1)] private float accuracy;

        private Container _container;
        private GameObject _target;
        private bool grabbed;
        private bool rewarded;

        private bool collided;
        private Collider collision_collider;
        private bool triggered;
        private Collider trigger_collider;
        
        void Start()
        {
            Crane = _crane;
        }

        public override void InitializeEnvironment()
        {
            // Spawn the container
            var c = Instantiate(containerPrefab, Vector3.zero + transform.position, Quaternion.Euler(Vector3.zero), transform);
            _container = c.GetComponent<Container>();
            _target = Instantiate(targetPrefab, new Vector3(0,0.1f,-25) + transform.position, Quaternion.Euler(Vector3.zero), transform);
        }

        public override void OnEpisodeBegin()
        {
            OnCollisionExit(null);
            OnTriggerExit(null);

            // Reset crane position
            //Crane.ResetPosition(new Vector3(0, 15, Random.Range(-20,45)));
            Crane.ResetPosition(new Vector3(0, 15, 20) + transform.position);

            // Reset container position
            _container.ResetPosition(new Vector3(0,0.1f, Random.Range(-4,4)) + transform.position);
            //_container.ResetPosition(new Vector3(0, 0.1f, 0));

            // Set the target position
            _target.transform.position = new Vector3(0, 0.1f, Random.Range(15,40)) + transform.position;
            //_target.transform.position = new Vector3(0, 0.1f, 25);
            

            // Set the target as the container position
            TargetPosition = _container.transform.position;

            _container.transform.parent = transform;
            grabbed = false;
            rewarded = false;

        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(Crane.spreader.Position - transform.position);
            sensor.AddObservation(Crane.spreader.Rbody.velocity);
            sensor.AddObservation(Crane.spreader.Rotation);
            sensor.AddObservation(Crane.kat.Position - transform.position.z);            
            sensor.AddObservation(Crane.kat.Velocity);
            sensor.AddObservation(TargetPosition - transform.position);
        }

        public override void TakeActions(ActionBuffers actions)
        {
            Crane.MoveKat(actions.ContinuousActions[katIndex]);
            Crane.MoveWinch(actions.ContinuousActions[winchIndex]);
        }

        public override State State()
        {
            float swingDistance = Mathf.Abs(_crane.spreader.Position.z - _crane.kat.Position);
            float swingReward = 1f - swingDistance;
            float speedZReward = 1 - Mathf.Min(Mathf.Abs(_crane.spreader.Rbody.velocity.z), 1);
            float speedYReward = 1 - Mathf.Min(Mathf.Abs(_crane.spreader.Rbody.velocity.y), 1);
            float reward = (swingReward + speedYReward + speedZReward) / 3;            

            // Positive reward
            if (collided && (collision_collider.CompareTag(tagContainer) || collision_collider.CompareTag(tagTarget)))
            {
                float distance = Mathf.Abs(_crane.spreader.Position.z - TargetPosition.z);
                
                if(distance < accuracy)
                {
                    if (grabbed && !rewarded) 
                    {
                        rewarded = true;                        
                        return new State(1f + reward, true);
                    }
                    else
                    {
                        OnCollisionExit(null);
                        _crane.spreader.GrabContainer(_container.transform);
                        TargetPosition = _target.transform.position;
                        grabbed = true;
                        return new State(1f + reward, false);
                    }
                }
                else
                {
                    return new State(0.1f, true);
                }
            }

            // Dead
            if ((collided && collision_collider.CompareTag(tagDead))
                || (triggered && trigger_collider.CompareTag(tagDead))
                || (_crane.spreader.Rotation.eulerAngles.x > 45 && _crane.spreader.Rotation.eulerAngles.x < 315))
            {
                return new State(-1f, true);
            }

            return new State((-1f + swingReward) / MaxStep, false);
        }

        public void OnCollisionEnter(Collision collision)
        {
            collided = true;
            collision_collider = collision.collider;
        }

        public void OnCollisionStay(Collision collision)
        {
            collided = true;
            collision_collider = collision.collider;
        }

        public void OnCollisionExit(Collision collision)
        {
            collided = false;
            collision_collider = null;
        }

        public void OnTriggerEnter(Collider other)
        {
            triggered = true;
            trigger_collider = other;
        }

        public void OnTriggerStay(Collider other)
        {
            triggered = true;
            trigger_collider = other;
        }

        public void OnTriggerExit(Collider other)
        {
            triggered = false;
            trigger_collider = null;
        }
    }
}