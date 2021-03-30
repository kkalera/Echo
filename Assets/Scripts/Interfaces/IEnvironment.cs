using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnvironment
{
    float EnvironmentSizeX { get; set; }
    float EnvironmentSizeY { get; set; }
    float EnvironmentSizeZ { get; set; }

    /// <summary>
    /// Method to adjust the scale of the environment
    /// </summary>
    /// <param name="scale"> The scale to be applied </param>
    void SetEnvironmentScale(Vector3 scale);

    /// <summary>
    /// Return the scale of the environment
    /// </summary>
    /// <returns></returns>
    Vector3 GetEnvironmentScale();
}
