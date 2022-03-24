using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace Echo
{
    public class Environment : MonoBehaviour
    {        
        public virtual void InitializeEnvironment() { }
        public virtual void OnEpisodeBegin() { }
        public virtual void TakeActions(ActionBuffers actions) { }
        //public virtual void CollectObservations(VectorSensor sensor) { }
        public virtual Dictionary<string,float> CollectObservations() { return new Dictionary<string, float>(); }
        public virtual State State() { return new State(); }
        public virtual Crane Crane { get; set; }
        public virtual int MaxStep { get; set; }
        public virtual Vector3 TargetPosition { get; set; }
        public virtual bool NormalisedObservations { get; set; }
    }
}