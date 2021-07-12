using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatTraining : CraneLevel
{
    [SerializeField] private GameObject targetPrefab;
    private GameObject target;

    public override Vector3 TargetLocation => target.transform.localPosition;

    public override void ClearEnvironment(Transform environment)
    {
        Destroy(environment.Find("target"));
    }

    public override void InitializeEnvironment(Transform environment)
    {
        target = Instantiate(targetPrefab, environment);
        target.name = "target";
    }

    public override void ResetEnvironment(Transform environment, ICrane crane)
    {
        target.transform.localPosition = new Vector3(0, 20, Random.Range(-20, 35));        
        crane.ResetToPosition(new Vector3(0, 20, Random.Range(-20, 35)));
    }

    public override ICrane SetCraneRestrictions(ICrane crane)
    {
        crane.CabinMovementDisabled = false;
        crane.CraneMovementDisabled = true;
        crane.WinchMovementDisabled = true;
        return crane;
    }
}
