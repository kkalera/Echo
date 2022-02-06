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
            craneSpecs.katWorldPosition = transform.position;            
        }

        void Update()
        {            
            craneSpecs.katWorldPosition = transform.position;            
        }
        private void FixedUpdate()
        {
            ManageKatLimit();
            MoveKat(craneSpecs.katSpeed);
        }

        public void MoveKat(float value)
        {            
            Utils.AccelerateRigidbody_Z_Axis(_rigidBody, value * craneSpecs.katMaxSpeed, craneSpecs.katMaxSpeed, craneSpecs.katAcceleration, Time.fixedDeltaTime);
        }
        private void ManageKatLimit()
        {
            if ((craneSpecs.katWorldPosition.z > 50 && _rigidBody.velocity.z > 0) || (craneSpecs.katWorldPosition.z < -25 && _rigidBody.velocity.z < 0))
            {
                _rigidBody.AddForce(new Vector3(0, 0, -_rigidBody.velocity.z), ForceMode.VelocityChange);
            }            
        }
    }
}