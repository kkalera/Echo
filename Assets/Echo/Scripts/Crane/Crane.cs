using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class Crane : MonoBehaviour
    {
        [SerializeField] public SoCraneSpecs craneSpecs;
        [SerializeField][Range(0,10)] private float _swingLimit;

        public void MoveWinch(float value)
        {
            craneSpecs.winchSpeed = value;
        }

        public void MoveKat(float value)
        {
            craneSpecs.katSpeed = value;
        }
        private void Update()
        {
            ManageSwingLimit();
        }
        private void ManageSwingLimit()
        {
            if (Mathf.Abs(craneSpecs.spreaderWorldPosition.z - craneSpecs.katWorldPosition.z) > _swingLimit)
            {
                var spreaderVelocity = craneSpecs.spreaderBody.velocity;
                var katVelocity = craneSpecs.katBody.velocity;
                var delta = Mathf.Abs(spreaderVelocity.z - katVelocity.z);

                if (craneSpecs.spreaderWorldPosition.z > craneSpecs.katWorldPosition.z)
                {                    
                    craneSpecs.spreaderBody.AddForce(new Vector3(0, 0, -delta), ForceMode.VelocityChange);
                }
                else
                {
                    craneSpecs.spreaderBody.AddForce(new Vector3(0, 0, delta), ForceMode.VelocityChange);
                }
            }
        }
        
    }
}