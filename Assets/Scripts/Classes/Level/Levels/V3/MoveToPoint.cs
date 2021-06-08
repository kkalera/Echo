using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPoint : CraneLevel
{
    [SerializeField] [Range(0.001f, 1)] float increment = 0.01f;
    [SerializeField] [Range(0.001f, 10)] float _timeTarget = 0.01f;
    [SerializeField] [Range(0, 25)] float _spreaderMin = 25f;
    [SerializeField] Transform _targetIndicator;
    [SerializeField] TMPro.TextMeshPro _tmpro;

    private bool _targetReached = false;
    private bool _finalTraining = false;
    private bool _episodeComplete = false;
    private bool _winchDisabled = true;

    private float _enterTime = -1f;

    private ICrane _crane;

    public override Vector3 TargetLocation => _targetLocation;
    private Vector3 _targetLocation;

    public override void OnEpisodeBegin()
    {
        if (_timeTarget == 5 && _spreaderMin == 3) _finalTraining = true;
        if (!_winchDisabled && _spreaderMin > 3) _timeTarget = 0.01f;
        if (_timeTarget == 5 && _winchDisabled) _winchDisabled = false;
        if (!_winchDisabled) _crane.SetWinchLimits(_spreaderMin, 30);

        // Set the allowed movements for the crane.
        _crane.CabinMovementDisabled = false;
        _crane.WinchMovementDisabled = _winchDisabled;
        _crane.SwingDisabled = true;

        _targetReached = false;
        _episodeComplete = false;

        _crane.ResetToPosition(new Vector3(0, Random.Range(_spreaderMin, 25), Random.Range(-25, 35)));

        float randomZ = Random.Range(-25, 35);
        if (randomZ > 4 && randomZ < 14) randomZ = 14;
        if (randomZ < -4 && randomZ > -13) randomZ = -13;

        _targetIndicator.localPosition = new Vector3(0, Random.Range(_spreaderMin, 25), randomZ);
        _targetLocation = _targetIndicator.localPosition;

    }

    public override void ResetEnvironment(ICrane crane)
    {
        _crane = crane;
    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        RewardData rd = new RewardData();

        rd.endEpisode = ProcessCollision(col, other);

        if (_episodeComplete)
        {
            if (_finalTraining) rd.reward += 1f;
            if (!_winchDisabled) rd.reward += 1f;

            rd.endEpisode = true;
            rd.reward += 1f;
            _timeTarget = Mathf.Min(_timeTarget * 1.1f, 5);

            if (!_winchDisabled) _spreaderMin = Mathf.Max(_spreaderMin * 0.999f, 3);
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
    }

    private bool ProcessCollision(Collision col = null, Collider other = null)
    {
        if (col != null)
        {
            return col.collider.CompareTag("dead");
        }

        if (other != null)
        {
            return other.CompareTag("dead");
        }

        return false;
    }
}
