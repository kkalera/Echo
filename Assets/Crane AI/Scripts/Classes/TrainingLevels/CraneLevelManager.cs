using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneLevelManager : MonoBehaviour, ILevelManager
{
    [SerializeField] List<CraneLevel> levels;

    private CraneLevel _craneLevel; 
    public CraneLevel CurrentLevel => _craneLevel;

    public void SetEnvironment(int level)
    {
        if(levels.Count > level && level >= 0)
        {
            _craneLevel = levels[level];
        }
    }
}
