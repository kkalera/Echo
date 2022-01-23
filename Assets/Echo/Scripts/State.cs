using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class State
    {
        public float reward;
        public bool dead;

        public State()
        {
            this.reward = 0;
            this.dead = false;
        }
        public State(float reward, bool dead)
        {
            this.reward = reward;
            this.dead = dead;
        }               
    }
}