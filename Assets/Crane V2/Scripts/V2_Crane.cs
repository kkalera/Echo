using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace echo
{
    public class V2_Crane : MonoBehaviour
    {
        [Header("Crane Specs")]
        [SerializeField] V2_SO_CraneSpecs _craneSpecs;
        [Header("Crane parts")]
        [SerializeField] V2_Spreader _spreader;
        [SerializeField] V2_Cabin _cabin;
        [SerializeField] V2_WinchManager _winchManager;
        [Header("Environment")]
        [SerializeField] Transform _environmentPosition;

        public V2_Agent Agent { set => _agent = value; }
        private V2_Agent _agent;
        public V2_Spreader Spreader => _spreader;
        public V2_Cabin Cabin => _cabin;

        private void Update()
        {
            _spreader.environmentPosition = _environmentPosition.position;
            _spreader.CollisionReceiver = _agent;
            _cabin.environmentPosition = _environmentPosition.position;
        }
        private static void AccelerateTo(Rigidbody body, Vector3 targetVelocity, float maxAccel, ForceMode forceMode = ForceMode.Acceleration)
        {
            Vector3 deltaV = targetVelocity - body.velocity;
            Vector3 accel = deltaV / Time.deltaTime;

            if (accel.sqrMagnitude > maxAccel * maxAccel)
                accel = accel.normalized * maxAccel;

            body.AddForce(accel, forceMode);
        }
        public void MoveCabin(float val)
        {
            if (!_craneSpecs.cabinMovementEnabled) return;
            if (_cabin.Position.z > 45 && val > 0) val = 0;
            if (_cabin.Position.z < -30 && val < 0) val = 0;
            AccelerateTo(_cabin.Rbody, new Vector3(0, 0, _craneSpecs.cabinSpeed * val), _craneSpecs.cabinAcceleration);

            var vel = _spreader.Rbody.velocity;
            vel.z = _craneSpecs.cabinSpeed * val;
            AccelerateTo(_spreader.Rbody,vel, _craneSpecs.cabinAcceleration);
        }
        public void MoveCrane(float val)
        {
            if (!_craneSpecs.craneMovementEnabled) return;
            if (transform.localPosition.x > 25 && val > 0) val = 0;
            if (transform.localPosition.x < -25 && val < 0) val = 0;
            AccelerateTo(GetComponent<Rigidbody>(), new Vector3(_craneSpecs.craneSpeed * val, 0, 0), _craneSpecs.craneAcceleration);
        }
        public void MoveWinch(float val)
        {
            if (!_craneSpecs.winchMovementEnabled) return;
            if (val > 0 && _spreader.Position.y > _craneSpecs.maxSpreaderHeight) val = 0;
            if (val < 0 && _spreader.Position.y < _craneSpecs.minSpreaderHeight) val = 0;
            _winchManager.MoveWinch(val);
        }
        public void ResetToPosition(Vector3 position)
        {
            transform.localPosition = new Vector3(position.x, 0, 0f);
            _cabin.transform.localPosition = new Vector3(0, 32, position.z);
            _spreader.transform.localPosition = new Vector3(_cabin.transform.localPosition.x, position.y, _cabin.transform.localPosition.z + 1);

            _spreader.Rbody.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            _spreader.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            _spreader.Rbody.velocity = Vector3.zero;
            _spreader.Rbody.angularVelocity = Vector3.zero;
            _cabin.Rbody.velocity = Vector3.zero;
            _spreader.Rbody.isKinematic = true;

            _winchManager.ResetWinch();
            _spreader.Rbody.isKinematic = false;
        }
        public void GrabContainer(V2_Container container)
        {
            container.transform.parent = _spreader.transform;            
        }
    }
}
