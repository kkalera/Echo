using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V3GrabAndPlace : CraneLevel
{
    [SerializeField] [Range(0.001f, 1)] float increment = 0.01f;
    [SerializeField] [Range(0.001f, 10)] float _timeTarget = 0.01f;
    [SerializeField] Transform _targetIndicator;
    [SerializeField] TMPro.TextMeshPro _tmpro;
    [SerializeField] Transform _container;
    [SerializeField] Transform _targetPlane;

    private bool _targetReached = false;
    private bool _containerGrabbed = false;
    private bool _episodeComplete = false;

    private ICrane _crane;

    public override Vector3 TargetLocation => _targetLocation;
    private Vector3 _targetLocation;

    private void Start()
    {
        _targetIndicator.GetComponent<MeshRenderer>().enabled = false;
    }

    public override void OnEpisodeBegin()
    {
        // Set the allowed movements for the crane.
        _crane.CabinMovementDisabled = false;
        _crane.WinchMovementDisabled = false;
        _crane.SwingDisabled = true;

        _targetReached = false;
        _containerGrabbed = false;
        _episodeComplete = false;

        _crane.ResetToPosition(new Vector3(0, Random.Range(15, 25), Random.Range(-10, 35)));
        _crane.ReleaseContainer(_crane.Transform);
        //_container.localPosition = new Vector3(0, -2.85f, 0);        

        _targetIndicator.localPosition = new Vector3(0, 3, Random.Range(-4, 4));
        _targetLocation = _targetIndicator.localPosition;

    }

    public override void ResetEnvironment(ICrane crane)
    {
        _crane = crane;
    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        RewardData rd = new RewardData();

        if (_episodeComplete)
        {
            rd.endEpisode = true;
            rd.reward += 1f;
        }

        if (_targetReached)
        {
            rd.reward += 1f / 5000;
        }

        return rd;
    }


    void Update()
    {
        _targetReached = Vector3.Distance(_crane.SpreaderPosition, _targetLocation) < 0.2;

        if (_targetReached && _crane.SpreaderVelocity.magnitude < 0.5)
        {
            if (!_containerGrabbed)
            {
                _crane.GrabContainer(_targetIndicator);
                _containerGrabbed = true;
                _targetLocation = new Vector3(0, Random.Range(0, 10), Random.Range(16, 35));
                _targetPlane.localPosition = _targetLocation - new Vector3(0, 3, 0);
            }
            else
            {
                _episodeComplete = true;
            }

        }



        _tmpro.text = "" + Vector3.Distance(_crane.SpreaderPosition, _targetLocation);
    }
}
