using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Env0 : MonoBehaviour, IEnvironment
{
    [SerializeField]
    private float _EnvSizeX;
    public float EnvironmentSizeX { get => _EnvSizeX; set => _EnvSizeX = value; }

    [SerializeField]
    private float _EnvSizeY;
    public float EnvironmentSizeY { get => _EnvSizeY; set => _EnvSizeY = value; }

    [SerializeField]
    private float _EnvSizeZ;
    public float EnvironmentSizeZ { get => _EnvSizeZ; set => _EnvSizeZ = value; }

    /// <summary>
    /// Method to adjust the scale of the environment
    /// </summary>
    /// <param name="scale"> The scale to be applied </param>
    public void SetEnvironmentScale(Vector3 scale)
    {
        transform.localScale = scale;
    }

    /// <summary>
    /// Return the scale of the environment
    /// </summary>
    /// <returns></returns>
    Vector3 IEnvironment.GetEnvironmentScale()
    {
        return transform.localScale;
    }
}
