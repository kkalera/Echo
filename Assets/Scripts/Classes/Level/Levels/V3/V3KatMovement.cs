using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V3KatMovement : CraneLevel
{
    [SerializeField] private Transform _environment;
    [SerializeField] private Transform _target;
    [SerializeField] private TMPro.TextMeshPro _tmpro;

    private ICrane _crane;
    public override Vector3 TargetLocation => _target.position - _environment.position;

    private bool _endEpisode;
    private float _timeTarget;
    private float _enterTime;
    private static float maxStayTime = 5f;

    private void Start()
    {
        _timeTarget = 0.01f;
    }
    public override void OnEpisodeBegin()
    {
        _crane.WinchMovementDisabled = true;
        _crane.CraneMovementDisabled = true;
        _crane.CabinMovementDisabled = false;

        _endEpisode = false;
        _enterTime = -1f;

        _crane.ResetToPosition(new Vector3(0, 20, Random.Range(-15, 35)));
        _target.position = _environment.position + new Vector3(0, 20, Random.Range(-15, 35));
    }

    public override void ResetEnvironment(ICrane crane)
    {
        _crane = crane;
    }

    private void Update()
    {
        _tmpro.text = "" + TargetLocation;
        if (Vector3.Distance(_target.position, _crane.SpreaderWorldPosition) < 1)
        {
            if (_enterTime == -1f) _enterTime = Time.time;
            if (_enterTime != -1f && _enterTime + _timeTarget < Time.time) _endEpisode = true;
        }



    }

    public override RewardData Step(Collision col = null, Collider other = null)
    {
        RewardData rd = new RewardData();
        rd.endEpisode = _endEpisode;
        if (_endEpisode) _timeTarget = Mathf.Min(_timeTarget * 1.01f, maxStayTime);
        return rd;
    }
}
