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

        public void InitializeEnvironment(Transform environment, V2_Crane crane)
        {
            _environment = environment;
            _crane = crane;
            GameObject c = Instantiate(_levelSpecs.containerPrefab, new Vector3(0,2.75f,0), Quaternion.Euler(Vector3.zero), _environment);
            _target = c.transform;
        }

        public void ResetEnvironment()
        {
            _crane.ResetToPosition(new Vector3(0, 25, Random.Range(-15, 30)));
            _target.position = _environment.position + new Vector3(0, 2.75f, 0);
        }

        public Vector3 TargetPosition(Vector3 environmentPosition)
        {
            return _target.position - environmentPosition;
        }
    }
}
