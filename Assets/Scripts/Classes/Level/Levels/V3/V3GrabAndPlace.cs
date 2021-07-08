using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V3GrabAndPlace : CraneLevel
{
    [SerializeField] TMPro.TextMeshPro _tmpro;
    [SerializeField] GameObject containerPrefab;
    [SerializeField] GameObject targetPlanePrefab;
    [SerializeField] Transform _environment;
    [SerializeField] GameObject _sphere;

    private Transform _targetPlane;
    private Transform _container;
    private Transform _target;
    private bool _targetReached = false;
    private bool _containerGrabbed = false;
    private bool _episodeComplete = false;
    private bool _grabRewarded = false;

    private ICrane _crane;

    public override Vector3 TargetLocation => (_target.position + new Vector3(0, 3, 0)) - _environment.position;

    public override void OnEpisodeBegin()
    {
        _sphere.SetActive(false);

        if (_container == null)
        {
            GameObject cp = Instantiate(containerPrefab, _environment);
            _container = cp.transform;
            _container.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }

        if (_targetPlane == null)
        {
            GameObject cp = Instantiate(targetPlanePrefab, _environment);
            _targetPlane = cp.transform;
        }

        // Set the allowed movements for the crane.
        _crane.CabinMovementDisabled = false;
        _crane.WinchMovementDisabled = false;
        _crane.SwingDisabled = true;
        _crane.SetWinchLimits(0, 25);

        _targetReached = false;
        _containerGrabbed = false;
        _grabRewarded = false;
        _episodeComplete = false;


        /*float randomZCrane = Random.Range(-25, 35);
        if (randomZCrane > 4 && randomZCrane < 14) randomZCrane = 14;
        if (randomZCrane < -4 && randomZCrane > -13) randomZCrane = -13;*/

        //_crane.ResetToPosition(new Vector3(0, Random.Range(15, 25), randomZCrane));
        _crane.ResetToPosition(new Vector3(0, 25, -20));
        _crane.ReleaseContainer(_environment);

        /*float randomZContainer = Random.Range(-25, 4);
        if (randomZContainer < -4 && randomZContainer > -13) randomZContainer = -14;
        _container.localPosition = new Vector3(0, 0, randomZContainer);*/
        _container.localPosition = new Vector3(0, 0, 0);
        _container.localRotation = Quaternion.Euler(0, 90, 0);


        //_targetPlane.localPosition = new Vector3(0, Random.Range(0, 10), Random.Range(14, 40));
        _targetPlane.localPosition = new Vector3(0, 0, 25);

        _target = _container;
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
            rd.reward += .75f;
        }

        if (_containerGrabbed && !_grabRewarded)
        {
            rd.reward += .5f;
            _grabRewarded = true;
        }

        bool dead = ProcessCollision(col, other);
        if (dead)
        {
            rd.endEpisode = true;
            rd.reward = -1f;
        }

        return rd;
    }


    void Update()
    {
        _targetReached = Vector3.Distance(_crane.SpreaderWorldPosition + new Vector3(0, -3, 0), _target.position) < 4;


        if (_targetReached && _crane.SpreaderVelocity.magnitude < 0.5)
        {
            if (!_containerGrabbed)
            {
                _crane.GrabContainer(_container);
                _containerGrabbed = true;
                _target = _targetPlane;
            }
            else
            {
                _episodeComplete = true;
            }
        }
    }

    private bool ProcessCollision(Collision col = null, Collider other = null)
    {
        if (col != null)
        {
            if (col.collider.CompareTag("dead")) return true;
            if (col.collider.CompareTag("crane")) return true;
            return false;
        }

        if (other != null)
        {
            return other.CompareTag("dead");
        }

        return false;
    }
}
