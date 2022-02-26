using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Echo
{
    [RequireComponent(typeof(Rigidbody))]
    public class Spreader : MonoBehaviour
    {
        [SerializeField] private SoCraneSpecs craneSpecs;
        [SerializeField] private SoCollision collisionManager;
        [SerializeField] private bool noCable;

        public Vector3 Position { get => transform.position; }
        public Quaternion Rotation { get => transform.rotation; }
        public Rigidbody Rbody { get => GetComponent<Rigidbody>(); }

        private void FixedUpdate()
        {
            if (noCable)
            {
                Utils.AccelerateRigidbody_Y_Axis(Rbody, craneSpecs.winchSpeed * craneSpecs.winchMaxSpeed,
                    craneSpecs.winchMaxSpeed, craneSpecs.winchAcceleration, Time.fixedDeltaTime + 0.02f);
            }
        }
        public void GrabContainer(Transform container)
        {
            if(container.TryGetComponent<Rigidbody>(out Rigidbody rb)){
                rb.isKinematic = true;
            }
            container.parent = transform;
            container.transform.rotation = Quaternion.Euler(Vector3.zero);
            container.transform.position = new Vector3(transform.position.x, container.transform.position.y, transform.position.z);
            collisionManager.collided = false;
            collisionManager._collision = null;
        }
        private void OnCollisionEnter(Collision collision)
        {
            collisionManager.collided = true;
            collisionManager._collision = collision;
        }

        private void OnCollisionStay(Collision collision)
        {
            collisionManager.collided = true;
            collisionManager._collision = collision;
        }
        private void OnCollisionExit(Collision collision)
        {
            collisionManager.collided = false;
            collisionManager._collision = null;
        }
        private void OnTriggerEnter(Collider other)
        {          
            collisionManager.triggered = true;
            collisionManager.triggered_collider = other;
        }
        private void OnTriggerStay(Collider other)
        {
            collisionManager.triggered = true;
            collisionManager.triggered_collider = other;
        }
        private void OnTriggerExit(Collider other)
        {
            collisionManager.triggered = false;
            collisionManager.triggered_collider = null;
        }
    }
}