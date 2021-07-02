using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinchMovement : CraneLevel
{
    [SerializeField] private WinchMovementSettings _settings;
    [SerializeField] private Transform _environment;
    [SerializeField] private Transform _target;

    private bool _targetReached = false;    
    private bool _episodeComplete = false;
    private bool _randomHeightChosen = false;

    private ICrane _crane;

    public override Vector3 TargetLocation => _target.position - _environment.position;

    public override void OnEpisodeBegin()
    {
        Utils.ReportStat(_settings.velocityTarget, "Environment / WinchMovement / _velocityTarget");
        Utils.ReportStat(_settings.spreaderHeight, "Environment / WinchMovement / _spreaderHeight");

        _crane.CabinMovementDisabled = false;
        _crane.WinchMovementDisabled = false;
        _crane.SwingDisabled = true;
        _targetReached = false;
        _episodeComplete = false;

        _crane.SetWinchLimits(_settings.spreaderHeight - 2f, 27f);

        //_randomHeightChosen = Random.Range(0f, 1f) > 0.5f;
        _randomHeightChosen = true;

        float randomZCrane = Random.Range(-25, 35);
        if (randomZCrane > 4 && randomZCrane < 14) randomZCrane = 14;
        if (randomZCrane < -4 && randomZCrane > -13) randomZCrane = -13;

        if (!_randomHeightChosen) _crane.ResetToPosition(new Vector3(0, _settings.spreaderHeight, randomZCrane));
        if (_randomHeightChosen) _crane.ResetToPosition(new Vector3(0, Random.Range(_settings.spreaderHeight, 25f), randomZCrane));


        float randomZ = Random.Range(-25, 35);       
        if (randomZCrane < -13) randomZ = Random.Range(-4,35);
        if (randomZCrane > 14) randomZ = Random.Range(-25, 4);

        if (randomZ > 4 && randomZ < 14) randomZ = 14;
        if (randomZ < -4 && randomZ > -13) randomZ = -13;
        
        if (!_randomHeightChosen) _target.position = _environment.position + new Vector3(0, _settings.spreaderHeight, randomZ);
        if (_randomHeightChosen) _target.position = _environment.position + new Vector3(0, Random.Range(_settings.spreaderHeight, 25f), randomZ);

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
            if (_settings.FinalDifficulty) rd.reward += .25f;

            rd.endEpisode = true;
            rd.reward += .75f;
            _settings.IncreaseDifficulty();

        }

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
        _episodeComplete = (_targetReached && _crane.SpreaderVelocity.magnitude < _settings.velocityTarget);
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
