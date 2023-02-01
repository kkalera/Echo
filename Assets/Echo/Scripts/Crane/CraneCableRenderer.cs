using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneCableRenderer : MonoBehaviour
{
    [SerializeField] LineRenderer cable_RL_W;
    [SerializeField] LineRenderer cable_RL_L;

    [SerializeField] LineRenderer cable_LL_W;
    [SerializeField] LineRenderer cable_LL_L;

    [SerializeField] LineRenderer cable_RW_W;
    [SerializeField] LineRenderer cable_RW_L;

    [SerializeField] LineRenderer cable_LW_W;
    [SerializeField] LineRenderer cable_LW_L;

    [SerializeField] GameObject kat;
    [SerializeField] GameObject spreader;


    // Update is called once per frame
    void Update()
    {
        float z_kat = kat.transform.localPosition.z;
        float y_kat = kat.transform.localPosition.y;
        float x_kat = kat.transform.localPosition.x;
        float y_spreader = spreader.transform.localPosition.y;
        float z_spreader = spreader.transform.localPosition.z;
        float x_spreader = spreader.transform.localPosition.x;

        cable_RL_W.SetPositions(new Vector3[] { new Vector3(x: 2.6f, y: 32f, z: z_kat - .05f), new Vector3(x: 2.6f, y: 32f, z: z_kat - .05f) });
        cable_RL_L.SetPositions(new Vector3[] { new Vector3(x: 2.6f, y: 32f, z: z_kat - .95f), new Vector3(x: 2.6f, y: 32f, z: z_kat - .95f) });
    }
}
