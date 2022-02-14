using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{    
    public void ResetPosition(Vector3 position)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
