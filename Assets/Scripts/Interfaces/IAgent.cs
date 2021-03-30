using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgent
{
    IEnvironment Environment { get; set; }
}
