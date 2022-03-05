using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    [CreateAssetMenu(fileName = "CraneSpecs", menuName = "ScriptableObjects/CraneSpecs", order = 1)]
    public class SoCraneSpecs : ScriptableObject
    {
        public float winchMaxSpeed = 1;
        public float winchAcceleration = 1;
        public float winchCableAmount = 50;

        public float katMaxSpeed = 1;
        public float katAcceleration = 1;

        public float maxSpreaderHeight = 25;
        public float minSpreaderHeight = 0;
    }
}