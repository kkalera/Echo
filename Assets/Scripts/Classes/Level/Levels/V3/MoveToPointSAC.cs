using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPointSAC : CraneLevel
{
    [SerializeField] [Range(0.001f, 1)] float increment = 0.01f;
    [SerializeField] [Range(0.001f, 10)] float _timeTarget = 0.01f;
    [SerializeField] [Range(0, 25)] float _spreaderMin = 25f;
    [SerializeField] TMPro.TextMeshPro _tmpro;
    [SerializeField] private Transform _environment;
    [SerializeField] private Transform _target;


    private bool _targetReached = false;
    private bool _finalTraining = false;
    private bool _episodeComplete = false;

    private float _enterTime = -1f;

    private ICrane _crane;

    public override Vector3 TargetLocation => _target.position - _environment.position;

    public override void OnEpisodeBegin()
    {
        Utils.ReportStat(_timeTarget, "MoveToPoint/_timeTarget");
        Utils.ReportStat(_spreaderMin, "MoveToPoint/_spreaderMin");
        if (_timeTarget == 5 && _spreaderMin == 3) _finalTraining = true;

        // Set the allowed movements for the crane.
        _crane.CabinMovementDisabled = false;
        _crane.WinchMovementDisabled = false;
        _crane.SwingDisabled = true;

        _targetReached = false;
        _episodeComplete = false;



        float randomZCrane = Random.Range(-25, 35);
        if (randomZCrane > 4 && randomZCrane < 14) randomZCrane = 14;
        if (randomZCrane < -4 && randomZCrane > -13) randomZCrane = -13;

        _crane.ResetToPosition(new Vector3(0, _spreaderMin, randomZCrane));

        float randomZ = Random.Range(-25, 35);
        if (randomZ > 4 && randomZ < 14) randomZ = 14;
        if (randomZ < -4 && randomZ > -13) randomZ = -13;

        _target.position = _environment.position + new Vector3(0, _spreaderMin, randomZ);
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
            _timeTarget = Mathf.Min(_timeTarget * 1.1f, 5);

            if(Mathf.Approximately(_timeTarget, 5f))_spreaderMin = Mathf.Max(_spreaderMin * 0.99f, 3);
        }

        if (_targetReached)
        {
            rd.reward += 1f / 5000;
        }

        bool dead = ProcessCollision(col, other);
        if (dead)
        {
            rd.endEpisode = dead;
            rd.reward = -1f;
            _spreaderMin = Mathf.Min(_spreaderMin * 1.001f, 25);
        }

        return rd;
    }


    void Update()
    {
        _targetReached = Vector3.Distance(_crane.SpreaderWorldPosition, _target.position) < 1;

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
