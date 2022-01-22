using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    [RequireComponent(typeof(Rigidbody))]
    public class Kat : MonoBehaviour
    {
        [SerializeField] SoCraneSpecs _craneSpecs;
        Rigidbody _rigidBody;

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            MoveKat(_craneSpecs.katSpeed);
        }
        
        public void MoveKat(float value)
        {
            Vector3 targetVelocity = new Vector3(0, 0, value * _craneSpecs.katMaxSpeed);
            Utils.AccelerateRigidbody(_rigidBody, targetVelocity, _craneSpecs.katAcceleration);
        }
    }
}