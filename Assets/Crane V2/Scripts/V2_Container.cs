using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace echo
{
    public class V2_Container : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (transform.parent.TryGetComponent<V2_Spreader>(out V2_Spreader spreader))
            {
                Debug.Log("working");
            }
        }
    }
}