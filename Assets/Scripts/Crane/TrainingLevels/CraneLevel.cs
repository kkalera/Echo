using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraneLevel : MonoBehaviour
{
    public abstract ICrane SetCraneRestrictions(ICrane crane);
}
