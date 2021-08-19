using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelManager
{
    void SetEnvironment(int level);
    CraneLevel CurrentLevel { get; }
}
