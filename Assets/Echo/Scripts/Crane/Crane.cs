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
        [SerializeField] private Transform spreaderTransform;

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
            //ManageSwingLimit();
        }
        private void FixedUpdate()
        {
            
        }
        private void ManageSwingLimit()
        {
            if (spreaderBody.isKinematic || Time.timeScale == 0 || Mathf.Approximately(craneSpecs.spreaderVelocity.z,0)) return;

            var delta = Mathf.Abs(craneSpecs.spreaderWorldPosition.z - craneSpecs.katWorldPosition.z);
            if (delta > _swingLimit)
            {
                var t = spreaderTransform.position;

                if (craneSpecs.spreaderWorldPosition.z > craneSpecs.katWorldPosition.z)
                {
                    t.z -= delta - _swingLimit - 0.001f;
                    //spreaderVelocity.z = katVelocity.z * 1.1f;
                }
                else
                {
                    t.z += delta - _swingLimit - 0.001f;
                    //spreaderVelocity.z = katVelocity.z * .9f;
                }
                spreaderTransform.position = t;
                //spreaderBody.velocity = spreaderVelocity;
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