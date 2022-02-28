using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo {
    [RequireComponent(typeof(Filo.Cable))]
    public class CableManager : MonoBehaviour
    {
        [SerializeField] SoCraneSpecs _craneSpecs;
        Filo.Cable _cable;
        List<Filo.Cable.Link> _links;

        private void Start()
        {
            _cable = GetComponent<Filo.Cable>();
            _links = _cable.links;
        }
        private void Update()
        {
            //AddCable(_craneSpecs.winchSpeed);
        }

        public void AddCable(float val)
        {
            var crane = GetComponentInParent<Crane>();
            if ((crane.spreader.Position.y > 25 && _craneSpecs.winchSpeed > 0) ||
                (crane.spreader.Position.y < 0 && _craneSpecs.winchSpeed < 0) ||
                _craneSpecs.winchCableAmount < 5)
            {
                val = 0;
            }

            float spawnSpeed = _links[0].cableSpawnSpeed;
            //float timeDelta = Time.deltaTime + Time.fixedDeltaTime * Time.timeScale;
            float timeDelta = Time.deltaTime * Time.timeScale;
            float targetVelocity = val * _craneSpecs.winchMaxSpeed;

            float acceleration = _craneSpecs.winchAcceleration * timeDelta;
            float delta = Mathf.Abs(targetVelocity - spawnSpeed);

            if (acceleration > delta) acceleration = delta;

            spawnSpeed += (spawnSpeed < targetVelocity) ? acceleration : -acceleration;
            Mathf.Clamp(spawnSpeed, -_craneSpecs.winchMaxSpeed, _craneSpecs.winchMaxSpeed);

            for(int i = 0; i < _links.Count; i++)
            {
                var l = _links[i];
                if (l.type.Equals(Filo.Cable.Link.LinkType.Attachment))
                {
                    l.cableSpawnSpeed = spawnSpeed;
                    _links[i] = l;
                }                
            }
        }

    }
}

