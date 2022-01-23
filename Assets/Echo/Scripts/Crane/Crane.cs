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
        public void ResetPosition()
        {
            craneSpecs.spreaderBody.isKinematic = true;
            craneSpecs.katBody.isKinematic = true;

            craneSpecs.katBody.transform.position = new Vector3(0, 32, 15);
            craneSpecs.spreaderBody.transform.position = new Vector3(0, 20, 15);
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

            craneSpecs.spreaderBody.isKinematic = false;
            craneSpecs.katBody.isKinematic = false;
        }
        
    }
}