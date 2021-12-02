using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace echo
{
    public class V2_Crane : MonoBehaviour
    {
        [Header("Crane Specs")]
        [SerializeField] V2_CraneSpecs _craneSpecs;
        [Header("Crane parts")]
        [SerializeField] V2_Spreader _spreader;
        [SerializeField] V2_Cabin _cabin;
        [SerializeField] V2_WinchManager _winchManager;
        [Header("Environment")]
        [SerializeField] Transform _environmentPosition;

        private Vector3 _Position()
        {
            return transform.position - _environmentPosition.position;
        }

        private void OnEnable()
        {
            _spreader.environmentPosition = _environmentPosition.position;
            _cabin.environmentPosition = _environmentPosition.position;
        }

        private void Update()
        {
            
        }
        private static void AccelerateTo(Rigidbody body, Vector3 targetVelocity, float maxAccel, ForceMode forceMode = ForceMode.Acceleration)
        {
            Vector3 deltaV = targetVelocity - body.velocity;
            Vector3 accel = deltaV / Time.deltaTime;

            if (accel.sqrMagnitude > maxAccel * maxAccel)
                accel = accel.normalized * maxAccel;

            body.AddForce(accel, forceMode);
        }
        private void MoveCabin(float val)
        {
            if (!_craneSpecs.cabinMovementEnabled) return;
            if (_cabin.Position.z > 45 && val > 0) val = 0;
            if (_cabin.Position.z < -30 && val < 0) val = 0;
            AccelerateTo(_cabin.Rbody, new Vector3(0, 0, _craneSpecs.cabinSpeed * val), _craneSpecs.cabinAcceleration);
        }
        private void MoveCrane(float val)
        {
            if (!_craneSpecs.craneMovementEnabled) return;
            if (transform.localPosition.x > 25 && val > 0) val = 0;
            if (transform.localPosition.x < -25 && val < 0) val = 0;
            AccelerateTo(_cabin.Rbody, new Vector3(_craneSpecs.craneSpeed * val, 0, 0), _craneSpecs.craneAcceleration);
        }
        private void MoveWinch(float val)
        {
            if (!_craneSpecs.winchMovementEnabled) return;
            if (val > 0 && _spreader.Position.y > _craneSpecs.maxSpreaderHeight) val = 0;
            if (val < 0 && _spreader.Position.y < _craneSpecs.minSpreaderHeight) val = 0;
            _winchManager.MoveWinch(val);
        }
    }
}
