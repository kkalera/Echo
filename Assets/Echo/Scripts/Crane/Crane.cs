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
        [SerializeField] private List<HingeJoint> winches;
        
        [SerializeField] public Spreader spreader;
        [SerializeField] public Kat kat;
        
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
            //ManageSwingLimit();
        }
        private void FixedUpdate()
        {
            ManageSwingLimit();
        }

        private void ManageSwingLimit()
        {
            if (spreader.Rbody.isKinematic || Mathf.Approximately(Time.timeScale, 0)) return;

            var delta = Mathf.Abs(spreader.transform.position.z - kat.transform.position.z);
            if (delta > _swingLimit)
            {
                Vector3 sp = spreader.transform.position;

                spreader.transform.position = sp.z > kat.transform.position.z ? new Vector3(sp.x, sp.y, kat.transform.position.z + _swingLimit) : new Vector3(sp.x, sp.y, kat.transform.position.z - _swingLimit);    
            }
        }
        public void ResetPosition(Vector3 position)
        {
            craneSpecs.winchSpeed = 0;
            craneSpecs.katSpeed = 0;            
            
            spreader.Rbody.isKinematic = true;
            kat.Rbody.isKinematic = true;

            kat.Rbody.transform.position = new Vector3(position.x, 32, position.z);
            spreader.Rbody.transform.position = new Vector3(position.x, position.y, position.z);
            spreader.Rbody.transform.rotation = Quaternion.Euler(Vector3.zero);

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

            spreader.Rbody.isKinematic = false;
            kat.Rbody.isKinematic = false;            
        }
        
    }
}