using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EnvSpawner : MonoBehaviour
{
    [SerializeField] int numEnvironments = 1;
    [SerializeField] GameObject environmentPrefab;
    [SerializeField] int envSizeX;
    [SerializeField] int envSizeZ;
    public bool building = false;

    private GameObject[] spawnedPrefabs;
#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.update += Update;

    }

    private void OnDisable()
    {
        EditorApplication.update -= Update;
    }
#endif

    private void Awake()
    { 
     
        spawnedPrefabs = null;

        //Transform result = transform.Find("Environment");
        /*while (result)
        {
            spawnedPrefabs.Add(result);
        }*/

        spawnedPrefabs = GameObject.FindGameObjectsWithTag("Environment");
        //if (spawnedPrefabs != null && spawnedPrefabs.Length != numEnvironments) SpawnEnvironments();
        
    }
    
    private void Update()
    {
        //if (spawnedPrefabs != null && spawnedPrefabs.Length != numEnvironments) SpawnEnvironments();
        if (!building)
        {
            environmentPrefab.SetActive(true);
            spawnedPrefabs = GameObject.FindGameObjectsWithTag("Environment");
            for (int i = 0; i < spawnedPrefabs.Length; i++)
            {
                DestroyImmediate(spawnedPrefabs[i]);
            }
            spawnedPrefabs = null;
        }
    }

    public void SpawnEnvironments()
    {
        environmentPrefab.SetActive(true);
        spawnedPrefabs = GameObject.FindGameObjectsWithTag("Environment");
        for (int i = 0; i < spawnedPrefabs.Length; i++)
        {
            DestroyImmediate(spawnedPrefabs[i]);
        }
        spawnedPrefabs = null;

        // Spawn new prefabs
        for (int i = 0; i < numEnvironments; i++)
        {
            GameObject env = Instantiate(environmentPrefab, transform);
            env.name = "Environment";
            env.tag = "Environment";
        }


        // Move the prefabs
        float sqrt = Mathf.Sqrt(numEnvironments);
        int floorSqrt = Mathf.FloorToInt(sqrt);
        int spawned = 0;
        spawnedPrefabs = GameObject.FindGameObjectsWithTag("Environment");

        for (int i = 0; i < floorSqrt; i++)
        {
            for (int j = 0; j < floorSqrt; j++)
            {
                float spawnX = -envSizeX * j;
                float spawnZ = -envSizeZ * i;
                //transform.Find("Environment" + spawned).transform.localPosition = new Vector3(spawnX, 0, spawnZ);
                spawnedPrefabs[spawned].transform.localPosition = new Vector3(spawnX, 0, spawnZ);
                spawned++;
            }
        }

        while (spawned < numEnvironments)
        {
            float spawnZ = -envSizeZ * (floorSqrt);
            float spawnX = -envSizeX * (numEnvironments - spawned - 1);
            spawnedPrefabs[spawned].transform.localPosition = new Vector3(spawnX, 0, spawnZ);
            //transform.Find("Environment" + spawned).transform.localPosition = new Vector3(spawnX, 0, spawnZ);
            spawned++;
        }
        environmentPrefab.SetActive(false);
    }
}
