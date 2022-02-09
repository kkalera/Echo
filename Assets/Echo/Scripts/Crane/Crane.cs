using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class Crane : MonoBehaviour
    {
        [SerializeField] public SoCraneSpecs craneSpecs;
        [SerializeField][Range(0,10)] private float _swingLimit;
        [SerializeField] private List<Filo.Cable> cables;
        [SerializeField] public Rigidbody spreaderBody;
        [SerializeField] public Rigidbody katBody;
        private bool manageSwing;

        private void Start()
        {
            manageSwing = false;
        }
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
            craneSpecs.environmentWorldPosition = transform.position;
            
        }
        void FixedUpdate()
        {
            ManageSwingLimit();
        }
        private void ManageSwingLimit()
        {
            if (Mathf.Abs(craneSpecs.spreaderWorldPosition.z - craneSpecs.katWorldPosition.z) > _swingLimit)
            {
                var spreaderVelocity = spreaderBody.velocity;
                var katVelocity = katBody.velocity;
                var delta = Mathf.Abs(spreaderVelocity.z - katVelocity.z);

                if (craneSpecs.spreaderWorldPosition.z > craneSpecs.katWorldPosition.z)
                {                    
                    spreaderBody.AddForce(new Vector3(0, 0, -delta), ForceMode.VelocityChange);
                }
                else
                {
                    spreaderBody.AddForce(new Vector3(0, 0, delta), ForceMode.VelocityChange);
                }
            }
        }
        public void ResetPosition()
        {
            manageSwing = false;
            spreaderBody.isKinematic = true;
            katBody.isKinematic = true;

            katBody.transform.position = new Vector3(0, 32, 25);
            spreaderBody.transform.position = new Vector3(0, 20, 25);
            spreaderBody.transform.rotation = Quaternion.Euler(Vector3.zero);

            for(int i = 0; i < cables.Count; i++)
            {
                cables[i].Setup();

                for(int ii = 0; ii < cables[i].links.Count; ii++)
                {
                    var link = cables[i].links[ii];
                    if (link.hybridRolling)
                    {
                        link.storedCable = 50;
                    }
                    cables[i].links[ii] = link;
                }
            }

            spreaderBody.isKinematic = false;
            katBody.isKinematic = false;
            manageSwing = true;
        }
        
    }
}