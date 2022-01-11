using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.DownArrow)) transform.position = transform.position + Vector3.down * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow)) transform.position = transform.position + Vector3.up * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow)) transform.position = transform.position + Vector3.left * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow)) transform.position = transform.position + Vector3.right * Time.deltaTime;
    }

}
