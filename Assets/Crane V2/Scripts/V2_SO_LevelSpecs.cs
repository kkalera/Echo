using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace echo
{
    [CreateAssetMenu(fileName = "LevelSpecs", menuName = "ScriptableObjects/LevelSpecs", order = 1)]
    public class V2_SO_LevelSpecs : ScriptableObject
    {
        [SerializeField] GameObject _containerPrefab;
        [SerializeField] GameObject _targetPrefab;
        [SerializeField] V2_ILevel level;
        public GameObject containerPrefab => _containerPrefab;
        public GameObject targetPrefab => _targetPrefab;
    }
}