using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public virtual void Step() { }
    public virtual void InitializeEnvironment() { }
    public virtual void OnEpisodeBegin() { }
}
