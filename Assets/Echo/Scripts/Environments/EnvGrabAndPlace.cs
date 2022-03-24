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
        [SerializeField] private bool normalisation;
        [SerializeField] private bool randomPosition;
        [SerializeField] private bool denseRewards;

        private Container _container;
        private GameObject _target;
        private bool grabbed;
        private bool rewarded;

        private bool collided;
        private Collider collision_collider;
        private bool triggered;
        private Collider trigger_collider;
        
        private float startDistance;
        public override bool NormalisedObservations { get => normalisation; set =>normalisation = value; }
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
            if (randomPosition)
            {
                Crane.ResetPosition(new Vector3(0, Random.Range(15, 20), Random.Range(-20, 45)) + transform.position);
                _container.ResetPosition(new Vector3(0, 0.1f, Random.Range(-4, 4)) + transform.position);
                _target.transform.position = new Vector3(0, 0.01f, Random.Range(15, 40)) + transform.position;
            }
            else
            {
                Crane.ResetPosition(new Vector3(0, 15, 20) + transform.position);
                _container.ResetPosition(new Vector3(0, 0.1f, 0) + transform.position);
                _target.transform.position = new Vector3(0, 0.01f, 25) + transform.position;
            }

            // Set the target as the container position
            TargetPosition = _container.transform.position + new Vector3(0,2.75f,0);

            _container.transform.parent = transform;
            grabbed = false;
            rewarded = false;

            startDistance = Vector3.Distance(_crane.spreader.Position, TargetPosition);
            
            float swing = Academy.Instance.EnvironmentParameters.GetWithDefault("swing", _crane._swingLimit);
            if (Mathf.Approximately(swing, -1)) Application.Quit();
            
            _crane._swingLimit = swing;
            Academy.Instance.StatsRecorder.Add("Swing", swing);
        }

        /*
        public override void CollectObservations(VectorSensor sensor)
        {
            // Use normalisation
            if (normalisation)
            {
                Bounds bounds = GetComponent<BoxCollider>().bounds;
                float xmin = bounds.center.x - bounds.extents.x;
                float xmax = bounds.center.x + bounds.extents.x;
                float ymin = bounds.center.y - bounds.extents.y;
                float ymax = bounds.center.y + bounds.extents.y;
                float zmin = bounds.center.z - bounds.extents.z;
                float zmax = bounds.center.z + bounds.extents.z;

                //Vector3 spreaderPos = Crane.spreader.Position - transform.position;
                Vector3 spreaderPos = Crane.spreader.Position;
                spreaderPos.x = Utils.Normalize(spreaderPos.x, xmin, xmax);
                spreaderPos.y = Utils.Normalize(spreaderPos.y, ymin, ymax);
                spreaderPos.z = Utils.Normalize(spreaderPos.z, zmin, zmax);
                sensor.AddObservation(spreaderPos);

                Vector3 spreaderVel = Crane.spreader.Rbody.velocity;
                spreaderVel.x = Utils.Normalize(spreaderVel.x, _crane.craneSpecs.katMaxSpeed * 3, -(_crane.craneSpecs.katMaxSpeed * 3));
                spreaderVel.y = Utils.Normalize(spreaderVel.y, _crane.craneSpecs.winchMaxSpeed * 3, -(_crane.craneSpecs.winchMaxSpeed * 3));
                spreaderVel.z = Utils.Normalize(spreaderVel.z, _crane.craneSpecs.katMaxSpeed * 3, -(_crane.craneSpecs.katMaxSpeed * 3));
                sensor.AddObservation(spreaderVel);

                Vector3 spreaderRotation = Crane.spreader.Rotation.eulerAngles;
                spreaderRotation.x = Utils.Normalize(spreaderRotation.x, 0, 360);
                spreaderRotation.y = Utils.Normalize(spreaderRotation.y, 0, 360);
                spreaderRotation.z = Utils.Normalize(spreaderRotation.z, 0, 360);
                sensor.AddObservation(spreaderRotation);

                float katPosition = Crane.kat.Position;
                katPosition = Utils.Normalize(katPosition, zmin, zmax);
                sensor.AddObservation(katPosition);

                float katVelocity = Crane.kat.Velocity;
                katVelocity = Utils.Normalize(katVelocity, -_crane.craneSpecs.katMaxSpeed, _crane.craneSpecs.katMaxSpeed);
                sensor.AddObservation(katVelocity);

                Vector3 targetPos = TargetPosition;
                targetPos.x = Utils.Normalize(targetPos.x, xmin, xmax);
                targetPos.y = Utils.Normalize(targetPos.y, ymin, ymax);
                targetPos.z = Utils.Normalize(targetPos.z, zmin, zmax);
                sensor.AddObservation(targetPos);


            }
            else
            {
                sensor.AddObservation(Crane.spreader.Position - transform.position);
                sensor.AddObservation(Crane.spreader.Rbody.velocity);
                sensor.AddObservation(Crane.spreader.Rotation.eulerAngles);
                sensor.AddObservation(Crane.kat.Position - transform.position.z);
                sensor.AddObservation(Crane.kat.Velocity);
                sensor.AddObservation(TargetPosition - transform.position);
                
            } 
        }*/
        
        public override Dictionary<string, float> CollectObservations()
        {
            Dictionary<string, float> dict = new();
            if (normalisation)
            {
                Bounds bounds = GetComponent<BoxCollider>().bounds;
                float xmin = bounds.center.x - bounds.extents.x;
                float xmax = bounds.center.x + bounds.extents.x;
                float ymin = bounds.center.y - bounds.extents.y;
                float ymax = bounds.center.y + bounds.extents.y;
                float zmin = bounds.center.z - bounds.extents.z;
                float zmax = bounds.center.z + bounds.extents.z;

                //Vector3 spreaderPos = Crane.spreader.Position - transform.position;
                Vector3 spreaderPos = Crane.spreader.Position;
                spreaderPos.x = Utils.Normalize(spreaderPos.x, xmin, xmax);
                spreaderPos.y = Utils.Normalize(spreaderPos.y, ymin, ymax);
                spreaderPos.z = Utils.Normalize(spreaderPos.z, zmin, zmax);
                dict.Add("spreaderX", spreaderPos.x);
                dict.Add("spreaderY", spreaderPos.y);
                dict.Add("spreaderZ", spreaderPos.z);


                Vector3 spreaderVel = Crane.spreader.Rbody.velocity;
                spreaderVel.x = Utils.Normalize(spreaderVel.x, _crane.craneSpecs.katMaxSpeed * 3, -(_crane.craneSpecs.katMaxSpeed * 3));
                spreaderVel.y = Utils.Normalize(spreaderVel.y, _crane.craneSpecs.winchMaxSpeed * 3, -(_crane.craneSpecs.winchMaxSpeed * 3));
                spreaderVel.z = Utils.Normalize(spreaderVel.z, _crane.craneSpecs.katMaxSpeed * 3, -(_crane.craneSpecs.katMaxSpeed * 3));
                dict.Add("spreaderVelX", spreaderVel.x);
                dict.Add("spreaderVelY", spreaderVel.y);
                dict.Add("spreaderVelZ", spreaderVel.z);

                Vector3 spreaderRotation = Crane.spreader.Rotation.eulerAngles;
                spreaderRotation.x = Utils.Normalize(spreaderRotation.x, 0, 360);
                spreaderRotation.y = Utils.Normalize(spreaderRotation.y, 0, 360);
                spreaderRotation.z = Utils.Normalize(spreaderRotation.z, 0, 360);
                dict.Add("spreaderRotaX", spreaderRotation.x);
                dict.Add("spreaderRotaY", spreaderRotation.y);
                dict.Add("spreaderRotaZ", spreaderRotation.z);

                float katPosition = Crane.kat.Position;
                katPosition = Utils.Normalize(katPosition, zmin, zmax);
                dict.Add("kat", katPosition);

                float katVelocity = Crane.kat.Velocity;
                katVelocity = Utils.Normalize(katVelocity, -_crane.craneSpecs.katMaxSpeed, _crane.craneSpecs.katMaxSpeed);
                dict.Add("katVel", katVelocity);

                Vector3 targetPos = TargetPosition;
                targetPos.x = Utils.Normalize(targetPos.x, xmin, xmax);
                targetPos.y = Utils.Normalize(targetPos.y, ymin, ymax);
                targetPos.z = Utils.Normalize(targetPos.z, zmin, zmax);
                dict.Add("targetX", targetPos.x);
                dict.Add("targetY", targetPos.y);
                dict.Add("targetZ", targetPos.z);
            }
            else
            {
                dict.Add("spreaderX", (Crane.spreader.Position - transform.position).x);
                dict.Add("spreaderY", (Crane.spreader.Position - transform.position).y);
                dict.Add("spreaderZ", (Crane.spreader.Position - transform.position).z);
                dict.Add("spreaderVelX", Crane.spreader.Rbody.velocity.x);
                dict.Add("spreaderVelY", Crane.spreader.Rbody.velocity.y);
                dict.Add("spreaderVelZ", Crane.spreader.Rbody.velocity.z);
                dict.Add("spreaderRotaX", Crane.spreader.Rotation.eulerAngles.x);
                dict.Add("spreaderRotaY", Crane.spreader.Rotation.eulerAngles.y);
                dict.Add("spreaderRotaZ", Crane.spreader.Rotation.eulerAngles.z);
                dict.Add("kat", Crane.kat.Position - transform.position.z);
                dict.Add("katVel", Crane.kat.Velocity);
                dict.Add("targetX", (TargetPosition - transform.position).x);
                dict.Add("targetY", (TargetPosition - transform.position).y);
                dict.Add("targetZ", (TargetPosition - transform.position).z);
            }
            return dict;
        }

        public override void TakeActions(ActionBuffers actions)
        {
            Crane.MoveKat(actions.ContinuousActions[katIndex]);
            Crane.MoveWinch(actions.ContinuousActions[winchIndex]);
        }

        public override State State()
        {
            if (!GetComponent<BoxCollider>().bounds.Contains(_crane.spreader.Position)) return new State(0, true);
            float swingAmount = Mathf.Abs(_crane.spreader.Position.z - _crane.kat.Position);            

            // Positive reward
            if ((collided && (collision_collider.CompareTag(tagContainer) || collision_collider.CompareTag(tagTarget)))
                || triggered && (trigger_collider.CompareTag(tagContainer) || trigger_collider.CompareTag(tagTarget)))
            {
                float distance = Mathf.Abs(_crane.spreader.Position.z - TargetPosition.z);
                
                if(distance < accuracy)
                {
                    if (grabbed && !rewarded) 
                    {
                        rewarded = true;                        
                        return new State(2f, true);
                    }
                    else
                    {
                        OnCollisionExit(null);
                        _crane.spreader.GrabContainer(_container.transform);
                        TargetPosition = _target.transform.position + new Vector3(0,2.75f,0);
                        startDistance = Vector3.Distance(TargetPosition, _crane.spreader.Position);
                        grabbed = true;
                        return new State(2f, false);
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

            // Swing reward
            /*
            float swingMultiplier = Academy.Instance.EnvironmentParameters.GetWithDefault("swingMultiplier", 0);
            float swingLimit = Academy.Instance.EnvironmentParameters.GetWithDefault("swing", _crane._swingLimit);
            Academy.Instance.StatsRecorder.Add("SwingAmount", swingAmount);
            float swingReward = swingMultiplier * Mathf.Pow(swingAmount, 2) * Mathf.Min(swingLimit, 1);*/            

            return new State(-1f / MaxStep, false);
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