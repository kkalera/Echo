using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPoint : CraneLevel
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
    [SerializeField] private bool _winchDisabled = true;
    [SerializeField] private bool _lowTimeTraining = false;
    private bool _heightTraining = false;


    private float _enterTime = -1f;

    private ICrane _crane;

    public override Vector3 TargetLocation => _target.position - _environment.position;

    public override void OnEpisodeBegin()
    {
        Utils.ReportStat(_timeTarget, "MoveToPoint/_timeTarget");
        Utils.ReportStat(_spreaderMin, "MoveToPoint/_spreaderMin");

        if (_timeTarget == 5 && _spreaderMin == 3 && !_finalTraining) _finalTraining = true;
        if (!_winchDisabled && _spreaderMin > 20f) _timeTarget = 0.01f;
        if (!_winchDisabled && Mathf.Approximately(_spreaderMin, 3f)) _lowTimeTraining = true;
        if ((_timeTarget == 5 || _lowTimeTraining) && _winchDisabled) _winchDisabled = false;
        if (!_winchDisabled) { _crane.SetWinchLimits(_spreaderMin - 2f, 30); }

        // Set the allowed movements for the crane.
        _crane.CabinMovementDisabled = false;
        _crane.WinchMovementDisabled = _winchDisabled;
        _crane.SwingDisabled = true;

        _targetReached = false;
        _episodeComplete = false;



        float randomZCrane = Random.Range(-25, 35);
        if (randomZCrane > 4 && randomZCrane < 14) randomZCrane = 14;
        if (randomZCrane < -4 && randomZCrane > -13) randomZCrane = -13;

        int craneStart = 0;
        if (randomZCrane >= -4 && randomZCrane <= 4) craneStart = 1;
        if (randomZCrane > 14) craneStart = 2;

        _crane.ResetToPosition(new Vector3(0, _spreaderMin, randomZCrane));

        float randomZ = Random.Range(-25, 35);
        if (randomZ > 4 && randomZ < 14) randomZ = 14;
        if (randomZ < -4 && randomZ > -13) randomZ = -13;

        int targetStart = 0;
        if (randomZ >= -4 && randomZ <= 4) targetStart = 1;
        if (randomZ > 14) targetStart = 2;

        _heightTraining = craneStart != targetStart;

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
            if (_finalTraining) rd.reward += .25f;
            if (!_winchDisabled) rd.reward += .25f;

            rd.endEpisode = true;
            rd.reward += .5f;
            _timeTarget = Mathf.Min(_timeTarget * 1.01f, 5);

            if (!_winchDisabled && _heightTraining) _spreaderMin = Mathf.Max(_spreaderMin - .1f, 3);
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
            if (!_winchDisabled && !_finalTraining && !_lowTimeTraining) _spreaderMin = Mathf.Min(_spreaderMin + .1f, 25);
        }

        return rd;
    }


    void Update()
    {
        if(_crane != null)
        {
            _targetReached = Vector3.Distance(_crane.SpreaderWorldPosition, _target.position) < 1;

            if (_targetReached && _enterTime == -1f) _enterTime = Time.time;
            if (_targetReached && _enterTime != -1f) _episodeComplete = Time.time > _enterTime + _timeTarget;
            if (!_targetReached) _enterTime = -1f;
        }
        
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
