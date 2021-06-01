using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPoint : CraneLevel
{
    [SerializeField] [Range(0.001f, 1)] float increment = 0.01f;
    [SerializeField] [Range(0.001f, 10)] float _timeTarget = 0.01f;
    [SerializeField] Transform _targetIndicator;
    [SerializeField] TMPro.TextMeshPro _tmpro;

    private bool _targetReached = false;
    private bool _finalTraining = false;
    private bool _episodeComplete = false;

    private float _enterTime = -1f;

    private ICrane _crane;

    public override Vector3 TargetLocation => _targetLocation;
    private Vector3 _targetLocation;

    public override void OnEpisodeBegin()
    {
        // Set the allowed movements for the crane.
        _crane.CabinMovementDisabled = false;
        _crane.WinchMovementDisabled = false;
        _crane.SwingDisabled = true;

        _targetReached = false;
        _episodeComplete = false;

        _crane.ResetToPosition(new Vector3(0, Random.Range(15, 25), Random.Range(-10, 35)));
        _targetIndicator.localPosition = new Vector3(0, Random.Range(15, 25), Random.Range(-10, 35));
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
            if (_finalTraining) rd.reward += 1f;

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
        _targetReached = Vector3.Distance(_crane.SpreaderPosition, _targetLocation) < 1;

        if (_targetReached && _enterTime == -1f) _enterTime = Time.time;
        if (_targetReached && _enterTime != -1f) _episodeComplete = Time.time > _enterTime + _timeTarget;
        if (!_targetReached) _enterTime = -1f;

        _tmpro.text = "" + Vector3.Distance(_crane.SpreaderPosition, _targetLocation);
    }
}
