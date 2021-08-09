using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAndPlace : CraneLevel
{    
    [SerializeField] private GameObject containerPrefab;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private int gravityMultiplier;
    public bool enableSwing;
    private Transform container;
    private Transform target;
    private Transform environment;
    private Vector3 currentTarget;
    private ICrane _crane;
    public override ICrane Crane { get => _crane; set => _crane = value; }

    public override Vector3 TargetLocation => currentTarget;  

    public override void ClearEnvironment()
    {
        Destroy(environment.Find("container"));
        Destroy(environment.Find("targetPlane"));
    }

    public override RewardData GetReward()
    {
        if (_crane.ContainerGrabbed && Vector3.Distance(container.position, target.position) < 2.5)
        {
            _crane.ReleaseContainer(environment);
            return new RewardData(1f, true);
        }
        return new RewardData();
    }

    public override void IncreaseDifficulty(){}

    public override void InitializeEnvironment(Transform environment, ICrane crane)
    {
        _crane = crane;
        this.environment = environment;

        GameObject containerobj = Instantiate(containerPrefab, this.environment);
        containerobj.name = "container";
        container = containerobj.transform;

        GameObject targetobj = Instantiate(targetPrefab, this.environment);
        targetobj.name = "targetPlane";
        target = targetobj.transform;
        
    }

    private void Update()
    {
        if(!_crane.ContainerGrabbed && Vector3.Distance(_crane.SpreaderWorldPosition, container.position + new Vector3(0,2.85f,0)) < 0.2)
        {
            _crane.GrabContainer(container);
            currentTarget = target.position + new Vector3(0, 2.85f,0);
        }        
    }

    public override void ResetEnvironment()
    {
        _crane.ReleaseContainer(environment);
        container.transform.rotation = Quaternion.Euler(Vector3.zero);
        //container.transform.localPosition = new Vector3(0, 0, Random.Range(-4,4));
        container.transform.localPosition = new Vector3(0, 0, -25);

        target.transform.localPosition = new Vector3(0, 0, Random.Range(16, 40));
        
        _crane.ResetToPosition(new Vector3(0, 25, 40)); /*------------------*/
        //_crane.ResetToPosition(new Vector3(0, 25, -25));

        currentTarget = container.transform.position + new Vector3(0,2.85f,0);
                
    }

    public override void SetCraneRestrictions()
    {        
        _crane.CraneMovementDisabled = true;
        _crane.CabinMovementDisabled = false;        
        _crane.WinchMovementDisabled = true;
        _crane.SwingDisabled = !enableSwing;        
        _crane.SetWinchLimits(0, 30);
    }
}
