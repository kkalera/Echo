using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kat : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) transform.position = transform.position + Vector3.forward * Time.deltaTime * 5;
        if (Input.GetKey(KeyCode.RightArrow)) transform.position = transform.position + Vector3.back * Time.deltaTime * 5;

    }
}
