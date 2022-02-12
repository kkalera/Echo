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
        [SerializeField] private List<HingeJoint> winches;

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
        private void FixedUpdate()
        {
            ManageSwingLimit();
        }
        private void ManageSwingLimit()
        {
            if (spreaderBody.isKinematic || Time.timeScale == 0) return;

            if (Mathf.Abs(craneSpecs.spreaderWorldPosition.z - craneSpecs.katWorldPosition.z) > _swingLimit)
            {
                var spreaderVelocity = spreaderBody.velocity;
                var katVelocity = katBody.velocity;
                //var delta = Mathf.Abs(spreaderVelocity.z - katVelocity.z);

                if (craneSpecs.spreaderWorldPosition.z > craneSpecs.katWorldPosition.z)
                {
                    //spreaderBody.velocity += new Vector3(0, 0, -delta);
                    spreaderVelocity.z = katVelocity.z * 1.1f;
                    
                }
                else
                {
                    //spreaderBody.velocity += new Vector3(0, 0, delta);
                    spreaderVelocity.z = katVelocity.z * 0.9f;
                }
                spreaderBody.velocity = spreaderVelocity;
                
            }
        }
        public void ResetPosition(Vector3 position)
        {
            craneSpecs.winchSpeed = 0;
            craneSpecs.katSpeed = 0;
            
            
            spreaderBody.isKinematic = true;
            katBody.isKinematic = true;

            katBody.transform.position = new Vector3(0, 32, position.z);
            spreaderBody.transform.position = new Vector3(0, position.y, position.z);
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
            for (int w = 0; w < winches.Count; w++)
            {
                var m = winches[w].motor;
                m.targetVelocity = 0;
                winches[w].motor = m;
            }

            spreaderBody.isKinematic = false;
            katBody.isKinematic = false;            
        }
        
    }
}