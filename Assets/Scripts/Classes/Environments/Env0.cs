using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Env0 : MonoBehaviour, IEnvironment
{
    [SerializeField]
    private float _EnvSizeX;
    public float EnvSizeX { get => _EnvSizeX; set => _EnvSizeX = value; }

    [SerializeField]
    private float _EnvSizeY;
    public float EnvSizeY { get => _EnvSizeY; set => _EnvSizeY = value; }

    [SerializeField]
    private float _EnvSizeZ;
    public float EnvSizeZ { get => _EnvSizeZ; set => _EnvSizeZ = value; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
