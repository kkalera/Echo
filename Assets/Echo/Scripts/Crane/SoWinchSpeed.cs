using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    [CreateAssetMenu(fileName = "WinchSpeed", menuName = "ScriptableObjects/WinchSpeed", order = 1)]
    public class SoWinchSpeed : ScriptableObject
    {
        public float winchSpeed;
        public float maxSpeed;
    }
}