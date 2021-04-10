using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgent
{
    void OnCollisionEnter(Collision col);
    void OnCollisionStay(Collision col);
}
