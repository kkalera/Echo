using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

namespace Echo
{
    public class EnvGrabContainer : Environment
    {
        [SerializeField] SoCollision collisionManager;
        [SerializeField] SoCraneSpecs craneSpecs;
        [SerializeField] GameObject containerPrefab;
        [TagsAndLayers.TagDropdown] public string tagDead;
        [TagsAndLayers.TagDropdown] public string tagContainer;
        private GameObject container;
        
        public override void InitializeEnvironment()
        {
            container = Instantiate(containerPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), transform);
        }
        public override void OnEpisodeBegin()
        {
            collisionManager.Reset();
            container.transform.parent = transform;
            container.transform.rotation = Quaternion.Euler(Vector3.zero);
            container.transform.position = Vector3.zero;
            
        }
        public override State Step()
        {
            if (collisionManager.collided && collisionManager._collision.collider.CompareTag(tagDead)) return new State(-1f, true);
            if (collisionManager.collided && collisionManager._collision.collider.CompareTag(tagContainer))
            {                
                if(Mathf.Abs(craneSpecs.spreaderWorldPosition.z - TargetWorldPosition.z) < 0.3f)
                {
                    return new State(1f, true);
                }
            }
            return new State();
        }
        public override void UpdateTargetWorldPosition(Vector3 position)
        {
            base.UpdateTargetWorldPosition(position);
        }
    }
}