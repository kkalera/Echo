using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinchMovement : CraneLevel
{
    [SerializeField] [Range(0.001f, 1)] float increment = 0.01f;
    [SerializeField] [Range(0.001f, 10)] float _velocityTarget = 4f;
    [SerializeField] [Range(25, 0)] float _spreaderHeight = 25f;
    [SerializeField] private Transform _environment;
    [SerializeField] private Transform _target;

    private bool _targetReached = false;
    private bool _finalTraining = false;
    private bool _episodeComplete = false;
    private bool _randomHeightChosen = false;

    private float _enterTime = -1f;

    private ICrane _crane;

    public override Vector3 TargetLocation => _target.position - _environment.position;

    public override void OnEpisodeBegin()
    {
        Utils.ReportStat(_velocityTarget, "WinchMovement/_timeTarget");
        Utils.ReportStat(_spreaderHeight, "WinchMovement/_spreaderHeight");

        if (Mathf.Approximately(_velocityTarget, 0f) && Mathf.Approximately(_spreaderHeight, 3f) && !_finalTraining) _finalTraining = true;

        // Set the allowed movements for the crane.
        _crane.CabinMovementDisabled = false;
        _crane.WinchMovementDisabled = false;
        _crane.SwingDisabled = true;

        _targetReached = false;
        _episodeComplete = false;

        _randomHeightChosen = Random.Range(0f, 1f) > 0.5f;

        float randomZCrane = Random.Range(-25, 35);
        if (randomZCrane > 4 && randomZCrane < 14) randomZCrane = 14;
        if (randomZCrane < -4 && randomZCrane > -13) randomZCrane = -13;

        if (!_randomHeightChosen) _crane.ResetToPosition(new Vector3(0, _spreaderHeight, randomZCrane));
        if (_randomHeightChosen) _crane.ResetToPosition(new Vector3(0, Random.Range(_spreaderHeight, 25f), randomZCrane));


        float randomZ = Random.Range(-25, 35);
        if (randomZ > 4 && randomZ < 14) randomZ = 14;
        if (randomZ < -4 && randomZ > -13) randomZ = -13;
        _target.position = _environment.position + new Vector3(0, 25, randomZ);

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

            rd.endEpisode = true;
            rd.reward += 1f;
            //_timeTarget = Mathf.Min(_timeTarget + (increment / 2), 5);
            if (Mathf.Approximately(_spreaderHeight, 3f)) _velocityTarget = Mathf.Clamp(_velocityTarget - increment, 0, 4f);
            if (!_randomHeightChosen) _spreaderHeight = Mathf.Max(_spreaderHeight - increment, 3f);

        }

        //if (_targetReached) rd.reward += 1f / 5000;


        bool dead = ProcessCollision(col, other);
        if (dead)
        {
            rd.endEpisode = dead;
            rd.reward = -1f;
        }

        return rd;
    }


    void Update()
    {
        _targetReached = Vector3.Distance(_crane.SpreaderWorldPosition, _target.position) < 1;
        _episodeComplete = (_targetReached && _crane.SpreaderVelocity.magnitude < _velocityTarget);
        /*
        if (_targetReached && _enterTime == -1f) _enterTime = Time.time;
        if (_targetReached && _enterTime != -1f) _episodeComplete = Time.time > _enterTime + _timeTarget;
        if (!_targetReached) _enterTime = -1f;*/
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
