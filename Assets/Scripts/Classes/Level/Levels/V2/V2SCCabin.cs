using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V2SCCabin : CraneLevel
{
    [Header("Height")]
    [SerializeField][Range(1,25)] float startSpreaderHeight = 25;
    [SerializeField][Range(1, 25)] float endSpreaderHeight = 10;
    [SerializeField][Range(-0.0001f,1)] float heightDiscount = 0.01f;

    [Header("Swing % allowed")]
    [SerializeField] [Range(0, 100)] float startSwingAllowed = 100;
    [SerializeField][Range(0, 100)] float endSwingAllowed = 25;
    [SerializeField][Range(-0.0001f, 1)] float swingDiscount = 0.1f;

    private ICrane crane;
    private Vector3 _targetLocation;

    public override Vector3 TargetLocation => _targetLocation;

    public override void ResetEnvironment(ICrane crane)
    {
        this.crane = crane;
    }

    public override void OnEpisodeBegin()
    {
        throw new System.NotImplementedException();
    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        throw new System.NotImplementedException();
    }
}
