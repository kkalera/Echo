using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace echo
{
    public class GrabPlace : MonoBehaviour, V2_ILevel
    {
        [SerializeField] V2_SO_LevelSpecs _levelSpecs;
        private Transform _environment;
        private V2_Crane _crane;
        private Transform _target;
        private Transform _container;
        private bool _dead = false;
        private bool _containerGrabbed = false;
        public bool Dead => _dead;

        public void InitializeEnvironment(Transform environment, V2_Crane crane)
        {
            _environment = environment;
            _crane = crane;
            GameObject c = Instantiate(_levelSpecs.containerPrefab, new Vector3(0,2.75f,0), Quaternion.Euler(Vector3.zero), _environment);
            _container = c.transform;
            GameObject t = Instantiate(_levelSpecs.targetPrefab, new Vector3(0, 0, 27f), Quaternion.Euler(Vector3.zero), _environment);
            _target = t.transform;
            _dead = false;
        }
        public void ResetEnvironment()
        {
            _container.parent = _environment;
            _crane.ResetToPosition(new Vector3(0, 25, Random.Range(-15, 30)));
            _container.position = _environment.position + new Vector3(0, 2.75f, 0);
            _container.rotation = Quaternion.Euler(Vector3.zero);
            _dead = false;
            _containerGrabbed = false;
        }

        public Vector3 TargetPosition(Vector3 environmentPosition)
        {
            if(_containerGrabbed) return _target.position - environmentPosition;
            return _container.position - environmentPosition;
        }
        public V2_CollisionResponse OnCollision(Collision col)
        {
            Debug.Log(col);
            var response = new V2_CollisionResponse();
            if (col.collider.tag.Equals("container") && Mathf.Abs(_crane.Spreader.Position.z - TargetPosition(_environment.position).z) < 0.1f)
            {
                if (!_containerGrabbed)
                {
                    var container = col.collider.GetComponent<V2_Container>();
                    _crane.GrabContainer(container);
                    response.Reward = 1;
                    _containerGrabbed = true;
                }
            }
            else if (col.collider.tag.Equals("shipFloor") && Mathf.Abs(_crane.Spreader.Position.z - TargetPosition(_environment.position).z) < 0.5f)
            {
                response.Reward = 1f;
                response.EndEpisode = true;
            }
            else if (col.collider.tag.Equals("crane"))
            {
                response.Reward = -1f;
                response.EndEpisode = true;
            }
            return response;
        }
    }
}
