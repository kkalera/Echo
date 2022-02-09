using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    [CreateAssetMenu(fileName = "CraneSpecs", menuName = "ScriptableObjects/CraneSpecs", order = 1)]
    public class SoCraneSpecs : ScriptableObject
    {
        public float winchSpeed = 0;
        public float winchMaxSpeed = 1;
        public float winchAcceleration = 1;

        public float katSpeed = 0;
        public float katMaxSpeed = 1;
        public float katAcceleration = 1;

        public float skewSpeed = 0;
        public float skewMaxSpeed = 1;

        public Vector3 spreaderWorldPosition = Vector3.zero;
        //public Rigidbody spreaderBody;

        public Vector3 katWorldPosition = Vector3.zero;
        //public Rigidbody katBody;
        public Vector3 environmentWorldPosition = Vector3.zero;
    }
}