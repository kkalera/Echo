using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class WinchTesting : MonoBehaviour
    {
        [SerializeField] private Crane crane;
        
        void Start()
        {
            crane.ResetPosition(new Vector3(0,10f,0));
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.DownArrow)) crane.MoveWinch(-1);
            if (Input.GetKey(KeyCode.UpArrow)) crane.MoveWinch(1);
            if (!Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow)) crane.MoveWinch(0);
        }
    }
}