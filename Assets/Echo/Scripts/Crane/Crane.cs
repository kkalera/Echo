using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class Crane : MonoBehaviour
    {
        [SerializeField] public SoCraneSpecs craneSpecs;
        [SerializeField] private bool UseSwingLimit;
        [SerializeField][Range(0,10)] public float _swingLimit;
        [SerializeField] [Range(0, 1)] private float _limitBounce;

        [SerializeField] public List<Filo.Cable> cables;
        [SerializeField] public List<HingeJoint> winches;
        
        [SerializeField] public Spreader spreader;
        [SerializeField] public Kat kat;

        public float winchSpeed;
        public float katSpeed;

        public void MoveWinch(float value)
        {
            winchSpeed = value;
        }
        public void MoveKat(float value)
        {
            katSpeed = value;
        }
        private void FixedUpdate()
        {
            if (UseSwingLimit) ManageSwingLimit();
        }
        private void ManageSwingLimit()
        {
            if (spreader.Rbody.isKinematic || Mathf.Approximately(Time.timeScale, 0)) return;

            var delta = Mathf.Abs(spreader.transform.position.z - kat.transform.position.z);
            if (delta > _swingLimit)
            {                                
                var spreaderVelocity = spreader.Rbody.velocity;

                if (spreader.transform.position.z > kat.transform.position.z)
                {
                    //spreaderVelocity.z = kat.Rbody.velocity.z < 0 ? kat.Rbody.velocity.z * 1.1f : -spreaderVelocity.z*_limitBounce;
                    spreaderVelocity.z = kat.Rbody.velocity.z < 0 ? kat.Rbody.velocity.z * 1.1f : 0;
                }
                else
                {
                    //spreaderVelocity.z = kat.Rbody.velocity.z < 0 ? -spreaderVelocity.z*_limitBounce : kat.Rbody.velocity.z * 1.1f;
                    spreaderVelocity.z = kat.Rbody.velocity.z < 0 ? 0 : kat.Rbody.velocity.z * 1.1f;
                }
                spreader.Rbody.velocity = spreaderVelocity;
            }
            
        }
        public void ResetPosition(Vector3 position)
        {
            winchSpeed = 0;
            katSpeed = 0;            
            
            spreader.Rbody.isKinematic = true;
            kat.Rbody.isKinematic = true;

            kat.Rbody.transform.position = new Vector3(position.x, 32, position.z);
            spreader.Rbody.transform.position = new Vector3(position.x, position.y, position.z);
            spreader.Rbody.transform.rotation = Quaternion.Euler(Vector3.zero);            

            for(int i = 0; i < cables.Count; i++)
            {
                
                //cables[i].links[0].orientation = false;
                //cables[i].links[0].storedCable = 50;
                //cables[i].links[2].orientation = true;
                //cables[i].links[2].storedCable = 50;

                for (int ii = 0; ii < cables[i].links.Count; ii++)
                {
                    var link = cables[i].links[ii];
                    if (link.hybridRolling)
                    {
                        link.storedCable = 50;
                    }
                    cables[i].links[ii] = link;
                }
                cables[i].Setup();

            }
            for (int w = 0; w < winches.Count; w++)
            {
                var m = winches[w].motor;
                m.targetVelocity = 0;
                winches[w].motor = m;

                Transform t = winches[w].transform;
                Vector3 rotation = t.rotation.eulerAngles;
                rotation.z = 0;
                t.rotation = Quaternion.Euler(rotation);
            }

            spreader.Rbody.isKinematic = false;
            kat.Rbody.isKinematic = false;            
        }
        
    }
}