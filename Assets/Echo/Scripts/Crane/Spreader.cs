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


        public void EditSpreaderPosition(Vector3 pos)
        {
            transform.position = pos;
        }
        void Start()
        {
            craneSpecs.spreaderWorldPosition = transform.position;
            craneSpecs.spreaderRotation = transform.rotation.eulerAngles;
        }
        void Update()
        {            
            craneSpecs.spreaderWorldPosition = transform.position - craneSpecs.environmentWorldPosition;
            craneSpecs.spreaderRotation = transform.rotation.eulerAngles;
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