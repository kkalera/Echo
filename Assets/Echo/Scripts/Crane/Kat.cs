using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    [RequireComponent(typeof(Rigidbody))]
    public class Kat : MonoBehaviour
    {
        [SerializeField] private SoCraneSpecs craneSpecs;
        public float Position { get => transform.position.z; }
        public float Velocity { get => Rbody.velocity.z; }
        public Rigidbody Rbody { get => GetComponent<Rigidbody>(); }

        private void FixedUpdate()
        {
            ManageKatLimit();
            MoveKat(craneSpecs.katSpeed);
        }

        public void MoveKat(float value)
        {            
            Utils.AccelerateRigidbody_Z_Axis(Rbody, value * craneSpecs.katMaxSpeed, craneSpecs.katMaxSpeed, craneSpecs.katAcceleration, Time.fixedDeltaTime+0.02f);
        }
        private void ManageKatLimit()
        {
            if ((Position > 50 && Velocity > 0) || (Position < -25 && Velocity < 0))
            {
                Rbody.AddForce(new Vector3(0, 0, -Velocity), ForceMode.VelocityChange);
            }            
        }
    }
}