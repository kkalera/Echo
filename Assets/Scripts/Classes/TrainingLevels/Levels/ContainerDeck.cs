using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerDeck : CraneLevel
{    
    [SerializeField] private GameObject containerPrefab;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform firstContainerOnDeck;
    private Transform container;
    private Transform target;
    private Transform environment;
    private Vector3 currentTarget;
    private ICrane _crane;
    private bool grabRewarded = false;
    private int containersPlaced = 0;
    private bool dead = false;

    public override ICrane Crane { get => _crane; set => _crane = value; }
    public override bool IsDead { get => dead; set => dead = value; }

    public override Vector3 TargetLocation => currentTarget + new Vector3(0,2.85f,0);  

    public override void ClearEnvironment()
    {
        GameObject[] containers = GameObject.FindGameObjectsWithTag("container");
        foreach (GameObject c in containers)
        {
            Destroy(c);
        }

        GameObject[] targets = GameObject.FindGameObjectsWithTag("targetPlane");
        foreach (GameObject c in targets)
        {            
            Destroy(c);                        
        }
    }

    public override RewardData GetReward()
    {
        if (dead) return new RewardData(0, true);

        if (!_crane.ContainerGrabbed && containersPlaced > 0)
        {
            _crane.ReleaseContainer(environment);
            return new RewardData(1f, true);
        }
        if(_crane.ContainerGrabbed && !grabRewarded)
        {
            grabRewarded = true;
            return new RewardData(1f, false);
        }
        return new RewardData();
    }

    public override void IncreaseDifficulty(){}

    public override void InitializeEnvironment(Transform environment, ICrane crane)
    {
        _crane = crane;
        this.environment = environment;
        dead = false;

        GameObject containerobj = Instantiate(containerPrefab, this.environment);
        containerobj.tag = "container";
        container = containerobj.transform;

        GameObject targetobj = Instantiate(targetPrefab, this.environment);
        targetobj.name = "targetPlane";
        targetobj.tag = "targetPlane";
        target = targetobj.transform;
        
    }

    private void Update()
    {
        if(!_crane.ContainerGrabbed && Vector3.Distance(_crane.SpreaderWorldPosition, container.position + new Vector3(0,2.85f,0)) < 0.1f)
        {
            _crane.GrabContainer(container);
            currentTarget = target.position;
        }

        if(_crane.ContainerGrabbed &&
            Mathf.Abs(container.position.y - currentTarget.y) < 0.01f &&
            Mathf.Abs(container.position.z - currentTarget.z) < 0.1f)
        {
            _crane.ReleaseContainer(environment);
            containersPlaced += 1;
            grabRewarded = false;
            target.position = container.position - new Vector3(0, 0, 2.55f);

            GameObject containerobj = Instantiate(containerPrefab, this.environment);
            containerobj.tag = "container";
            container = containerobj.transform;
            container.transform.localPosition = new Vector3(0, 0, Random.Range(-4, 4));

            currentTarget = container.position;
        }

        dead = Mathf.Abs(_crane.SpreaderPosition.z - (_crane.CabinPosition.z + 1)) > 1 + _crane.SwingLimit;
    }

    public override void ResetEnvironment()
    {
        ClearEnvironment();
        InitializeEnvironment(environment, _crane);

        _crane.ReleaseContainer(environment);
        container.transform.rotation = Quaternion.Euler(Vector3.zero);
        container.transform.localPosition = new Vector3(0, 0, Random.Range(-4,4));

        target.transform.position = firstContainerOnDeck.position - new Vector3(0, 0, 2.65f);
        _crane.ResetToPosition(new Vector3(0, 25, -10));
        currentTarget = container.transform.position;

        grabRewarded = false;
        containersPlaced = 0;
    }

    public override void SetCraneRestrictions()
    {        
        _crane.CraneMovementDisabled = true;
        _crane.CabinMovementDisabled = false;        
        _crane.WinchMovementDisabled = false;
        _crane.SwingLimit = 0.01f;
        _crane.SetWinchLimits(0, 30);
    }

}
