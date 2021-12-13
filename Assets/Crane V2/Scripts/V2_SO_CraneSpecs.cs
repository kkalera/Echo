using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace echo
{
    [CreateAssetMenu(fileName = "CraneSpecs", menuName = "ScriptableObjects/CraneSpecs", order = 1)]
    public class V2_SO_CraneSpecs : ScriptableObject
    {
        
        [SerializeField] [Range(.1f, 10f)] private float _craneSpeed = 0.75f; //Speed in m/s
        [SerializeField] [Range(.1f, 100f)] private float _craneAcceleration = 1f; //Acceleration in m/s²
        [SerializeField] [Range(.1f, 100f)] private float _cabinSpeed = 4; //Speed in m/s
        [SerializeField] [Range(.1f, 100f)] private float _cabinAcceleration = 1f; //Acceleration in m/s²
        [SerializeField] [Range(.1f, 100f)] private float _winchSpeed = 4; //Speed in m/s
        [SerializeField] [Range(.1f, 100f)] private float _winchAcceleration = 4; //Speed in m/s
        [SerializeField] [Range(.1f, 100f)] private float _maxSwing = 4; // Max amount of swing allowed. This is used to keep the spreader still in easier training sessions
        [SerializeField] private float _minSpreaderHeight = 0; // Lowest height the spreader is allowed to reach
        [SerializeField] private float _maxSpreaderHeight = 0; // Maximum height the spreader is allowed to reach
        [SerializeField] private bool _craneMovementEnabled = false; // Wether or not crane movement is allowed, this is left-right movement
        [SerializeField] private bool _cabinMovementEnabled = false; // Wether or not cabin movement is allowed, this is forwards-backwards movement
        [SerializeField] private bool _winchMovementEnabled = false; // Wether or not winch movement is allowed, this is up-down movement


        public float craneSpeed { get => _craneSpeed; set => _craneSpeed = value; }
        public float craneAcceleration{ get => _craneAcceleration; set => _craneAcceleration = value; }
        public float cabinSpeed { get => _cabinSpeed; set => _cabinSpeed = value; }
        public float cabinAcceleration { get => _cabinAcceleration; set => _cabinAcceleration = value; }
        public float winchSpeed { get => _winchSpeed; set => _winchSpeed = value; }
        public float winchAcceleration { get => _winchAcceleration; set => _winchAcceleration = value; }
        public float maxSwing { get => _maxSwing; set => maxSwing = value; }
        public float minSpreaderHeight { get => _minSpreaderHeight; set => _minSpreaderHeight = value; }
        public float maxSpreaderHeight { get => _maxSpreaderHeight; set => _maxSpreaderHeight = value; }
        public bool craneMovementEnabled { get => _craneMovementEnabled; set => _craneMovementEnabled = value; }
        public bool cabinMovementEnabled { get => _cabinMovementEnabled; set => _cabinMovementEnabled = value; }
        public bool winchMovementEnabled { get => _winchMovementEnabled; set => _winchMovementEnabled = value; }
        
    }
}