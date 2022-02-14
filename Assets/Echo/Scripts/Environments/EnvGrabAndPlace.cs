using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

namespace Echo
{
    public class EnvGrabAndPlace : Environment
    {
        [TagsAndLayers.TagDropdown] public string tagDead;
        [TagsAndLayers.TagDropdown] public string tagContainer;
        [TagsAndLayers.TagDropdown] public string tagGoal;

        [SerializeField] GameObject containerPrefab;
        [SerializeField] GameObject targetPrefab;

        [SerializeField] SoCollision collisionManager;
        [SerializeField] SoCraneSpecs craneSpecs;
        
        [SerializeField][Range(0.01f,1)] public float accuracy = 0.2f;
        [SerializeField] private bool useSwingReward = false;
        [SerializeField] private Crane crane;

        private GameObject container;
        private GameObject target;
        private bool grabbed = false;


        
        public override void InitializeEnvironment()
        {
            container = Instantiate(containerPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), transform);
            target = Instantiate(targetPrefab, new Vector3(0, 0.01f, 25), Quaternion.Euler(Vector3.zero), transform);
        }
        public override void OnEpisodeBegin()
        {
            collisionManager.Reset();
            container.transform.parent = transform;
            container.transform.rotation = Quaternion.Euler(Vector3.zero);
            container.transform.position = Vector3.zero;
            UpdateTargetWorldPosition(container.transform.position);

            if(container.TryGetComponent<Rigidbody>(out Rigidbody rb)){
                rb.isKinematic = true;
            }

            grabbed = false;
        }
        public override State Step()
        {
            if (collisionManager.collided && collisionManager._collision.collider.CompareTag(tagDead) 
                || collisionManager.triggered && collisionManager.triggered_collider.CompareTag(tagDead)
                || (craneSpecs.spreaderRotation.x > 45 && craneSpecs.spreaderRotation.x < 315)) return new State(-1f, true);
            
            if(grabbed && collisionManager.collided && collisionManager.triggered_collider.CompareTag(tagGoal))
            {
                if(Mathf.Abs(craneSpecs.spreaderWorldPosition.z - TargetWorldPosition.z) <= accuracy)
                {                    
                    return new State(1f, true);                    
                }
            }

            if (!grabbed && collisionManager.collided && collisionManager._collision.collider.CompareTag(tagContainer))
            {
                if (Mathf.Abs(craneSpecs.spreaderWorldPosition.z - TargetWorldPosition.z) <= accuracy)
                {
                    crane.spreader.GrabContainer(container.transform);
                    UpdateTargetWorldPosition(target.transform.position);
                    grabbed = true;
                    return new State(1f, false);
                }
            }

            if (useSwingReward)
            {
                float swing = Mathf.Abs(craneSpecs.katWorldPosition.z - craneSpecs.spreaderWorldPosition.z);
                float swingReward = (1 - (swing *2)) / MaxStep;
                return new State(swingReward, false);
            }
            return new State(-1/MaxStep,false);
        }
        public override void UpdateTargetWorldPosition(Vector3 position)
        {
            base.UpdateTargetWorldPosition(position);
        }
        public override Vector3 CraneStartLocation()
        {
            Vector3 loc = Vector3.zero;
            loc.y = Random.Range(5, 25);
            loc.z = Random.Range(10, 25);
            return loc;
        }
    }
}