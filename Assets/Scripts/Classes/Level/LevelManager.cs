using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    private CraneAgent agent;
    public float PlaneHalfSize { get => (agent.planeSize * agent.plane.localScale.x) / 2; }
    public float PlaneFullSize { get => (agent.planeSize * agent.plane.localScale.x); }

    void Start()
    {
        agent = GetComponent<CraneAgent>();
    }

    public void SetEnvironment(int level)
    {
        switch (level)
        {
            case 0:
                SetLevel0();
                break;
            case 1:
                SetLevel1();
                break;
            case 2:
                SetLevel2();
                break;
            case 3:
                SetLevel3();
                break;
            case 4:
                SetLevel3();
                break;
            default:
                SetLevel0();
                break;

        }
    }
    /// <summary>
    /// Very basic level requiring only kat and crane movement to hit the target
    /// </summary>
    private void SetLevel0()
    {
        agent.plane.localScale = new Vector3(1f, 1, 1f);
        agent.cableSwingDisabled = true;
        agent.winchDisabled = true;
        agent.container.transform.localPosition = new Vector3(Random.Range(-PlaneHalfSize + 7, PlaneHalfSize - 7), 0.01f, Random.Range(-PlaneHalfSize + 2, PlaneHalfSize - 2));
        Vector3 katPosition = new Vector3(Random.Range(-PlaneHalfSize + 7, PlaneHalfSize - 7), 10, Random.Range(-PlaneHalfSize + 2, PlaneHalfSize - 2));
        agent.crane.ResetToPosition(katPosition, 3f);
    }

    /// <summary>
    /// A basic level requiring only kat and crane movement, twice the size of the first level
    /// </summary>
    private void SetLevel1()
    {
        agent.plane.localScale = new Vector3(2f, 1, 2f);
        agent.cableSwingDisabled = true;
        agent.winchDisabled = true;
        agent.container.transform.localPosition = new Vector3(Random.Range(-PlaneHalfSize + 7, PlaneHalfSize - 7), 0.01f, Random.Range(-PlaneHalfSize + 2, PlaneHalfSize - 2));
        Vector3 katPosition = new Vector3(Random.Range(-PlaneHalfSize + 7, PlaneHalfSize - 7), 10, Random.Range(-PlaneHalfSize + 2, PlaneHalfSize - 2));
        agent.crane.ResetToPosition(katPosition, 3f);
    }

    /// <summary>
    /// A basic level requiring only kat and crane movement, but adding winch actions
    /// </summary>
    private void SetLevel2()
    {
        agent.plane.localScale = new Vector3(2, 1, 2);
        agent.cableSwingDisabled = true;
        agent.winchDisabled = false;
        agent.container.transform.localPosition = new Vector3(Random.Range(-PlaneHalfSize + 7, PlaneHalfSize - 7), 0.01f, Random.Range(-PlaneHalfSize + 2, PlaneHalfSize - 2));
        Vector3 katPosition = new Vector3(Random.Range(-PlaneHalfSize + 7, PlaneHalfSize - 7), 15, Random.Range(-PlaneHalfSize + 2, PlaneHalfSize - 2));
        agent.crane.ResetToPosition(katPosition, 8);
    }

    /// <summary>
    /// A level requiring winch, kat and crane movement while activating cable swing
    /// </summary>
    private void SetLevel3()
    {
        agent.plane.localScale = new Vector3(2, 1, 2);
        agent.cableSwingDisabled = false;
        agent.winchDisabled = false;
        agent.container.transform.localPosition = new Vector3(Random.Range(-PlaneHalfSize + 7, PlaneHalfSize - 7), 0.01f, Random.Range(-PlaneHalfSize + 2, PlaneHalfSize - 2));
        Vector3 katPosition = new Vector3(Random.Range(-PlaneHalfSize + 7, PlaneHalfSize - 7), 15, Random.Range(-PlaneHalfSize + 2, PlaneHalfSize - 2));
        agent.crane.ResetToPosition(katPosition, 8);
    }



}
