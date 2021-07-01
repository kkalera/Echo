using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinchMovementSettings : MonoBehaviour
{
    [SerializeField] [Range(0.01f,4f)]public float velocityTarget = 4f;
    [SerializeField] [Range(0.01f, 4f)] public float finalVelocityTarget = 0.05f;
    [SerializeField] [Range(0, 25)] public float spreaderHeight = 25f;
    [SerializeField] [Range(0, 25)] public float finalSpreaderHeight = 17;
    [SerializeField] public float discount = 0.1f;

    public bool FinalDifficulty => Mathf.Approximately(spreaderHeight, finalSpreaderHeight) && Mathf.Approximately(velocityTarget, finalVelocityTarget);

    public void IncreaseDifficulty()
    {
        if(Mathf.Approximately(spreaderHeight, finalSpreaderHeight))
        {
            velocityTarget = Mathf.Max(velocityTarget - discount, finalVelocityTarget);
        }
        else
        {
            spreaderHeight = Mathf.Max(spreaderHeight - discount, finalSpreaderHeight);
        }
    }    
}
