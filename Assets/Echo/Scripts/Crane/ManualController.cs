using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class ManualController : MonoBehaviour
    {
        private Crane _crane;
        // Start is called before the first frame update
        void Start()
        {
            _crane = GetComponent<Crane>();
        }

        // Update is called once per frame
        void Update()
        {            
            if (Input.GetKey(KeyCode.Z)) _crane.MoveKat(1);
            if (Input.GetKey(KeyCode.S)) _crane.MoveKat(-1);
            if (!Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.S)) _crane.MoveKat(0);

            if (Input.GetKey(KeyCode.UpArrow)) _crane.MoveWinch(1);
            if (Input.GetKey(KeyCode.DownArrow)) _crane.MoveWinch(-1);
            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) _crane.MoveWinch(0);
        }
    }
}

