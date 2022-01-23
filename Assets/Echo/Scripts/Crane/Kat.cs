using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    [RequireComponent(typeof(Rigidbody))]
    public class Kat : MonoBehaviour
    {
        [SerializeField] private SoCraneSpecs craneSpecs;
        private Rigidbody _rigidBody;

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
            craneSpecs.katBody = _rigidBody;
            craneSpecs.katWorldPosition = transform.position;            
        }

        void Update()
        {
            MoveKat(craneSpecs.katSpeed);
            ManageKatLimit();
            craneSpecs.katWorldPosition = transform.position;            
        }
        
        public void MoveKat(float value)
        {
            Vector3 targetVelocity = new Vector3(0, 0, value * craneSpecs.katMaxSpeed);
            Utils.AccelerateRigidbody(_rigidBody, targetVelocity, craneSpecs.katAcceleration);
        }
        private void ManageKatLimit()
        {
            if ((craneSpecs.katWorldPosition.z > 50 && craneSpecs.katBody.velocity.z > 0) || (craneSpecs.katWorldPosition.z < -25 && craneSpecs.katBody.velocity.z < 0))
            {
                craneSpecs.katBody.AddForce(new Vector3(0, 0, -craneSpecs.katBody.velocity.z), ForceMode.VelocityChange);
            }            
        }
    }
}