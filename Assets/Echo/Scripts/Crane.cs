using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class Crane : MonoBehaviour
    {
        [SerializeField] SoCraneSpecs _craneSpecs;

        public void MoveWinch(float value)
        {
            _craneSpecs.winchSpeed = value;
        }

        public void MoveKat(float value)
        {
            _craneSpecs.katSpeed = value;
        }

        private void Update()
        {
            MoveWinch(0);
            if (Input.GetKey(KeyCode.UpArrow)) MoveWinch(1);
            if (Input.GetKey(KeyCode.DownArrow)) MoveWinch(-1);

            MoveKat(0);
            if (Input.GetKey(KeyCode.LeftArrow)) MoveKat(1);
            if (Input.GetKey(KeyCode.RightArrow)) MoveKat(-1);
        }
    }
}