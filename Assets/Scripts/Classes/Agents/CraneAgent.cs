using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor;


public class CraneAgent : Agent
{
    [Header("Testing")]
    [Space(10)]
    [SerializeField] bool testMode = false;
    [Header("Level")]
    [Space(10)]
    [SerializeField] int level = 0;
    [SerializeField] public int planeSize = 30;
    [SerializeField] public bool cableSwingDisabled = false;
    [SerializeField] public bool winchDisabled = false;
    [SerializeField] public Transform plane;

    [Header("Variables")]
    [Space(10)]
    [SerializeField] private Environment environment;
    [SerializeField] public GameObject cranePrefab;
    [SerializeField] public GameObject containerPrefab;
    [SerializeField] [Range(.1f, 10f)] private float craneSpeed = 0.75f;
    [SerializeField] [Range(.1f, 10f)] private float katSpeed = 4;
    [SerializeField] [Range(1f, 100f)] private float liftSpeed = 100;


    [Header("Inputs")]
    [Space(10)]
    [SerializeField] private InputAction inputUp;
    [SerializeField] private InputAction inputDown;
    [SerializeField] private InputAction inputLeft;
    [SerializeField] private InputAction inputRight;
    [SerializeField] private InputAction inputLift;

    [HideInInspector] public Crane crane;
    [HideInInspector] public Container container;
    private GameObject craneGameObject;
    private RewardManager rewardManager;
    private LevelManager levelManager;
    private float winchVal = 0;
    private float katVal = 0;
    private float craneVal = 0;

    private void Awake()
    {
        crane = GetComponentInChildren<Crane>();
    }

    private void Start()
    {
        inputUp.Enable();
        inputDown.Enable();
        inputLeft.Enable();
        inputRight.Enable();
        inputLift.Enable();

        rewardManager = GetComponent<RewardManager>();
        levelManager = GetComponent<LevelManager>();
        rewardManager.level = level;
        container = GetComponentInChildren<Container>();
    }

    private void FixedUpdate()
    {
        float z = crane.Spreader.transform.localPosition.z;
        float x = crane.Spreader.transform.localPosition.z;
        if (z > levelManager.PlaneHalfSize || z < -levelManager.PlaneHalfSize || x > levelManager.PlaneHalfSize || x < -levelManager.PlaneHalfSize)
        {
            AddReward(-1f);
            EndEpisode();
        }
        if (crane != null && crane.Spreader != null && crane.Kat != null &&
            crane.Spreader.transform.localPosition.y > crane.Kat.transform.localPosition.y - 1)
        {
            Debug.Log("Spreader Y: " + crane.Spreader.transform.localPosition.y + " || kat Y: " + crane.Kat.transform.localPosition.y);
            AddReward(-1f); EndEpisode();
        }

    }

    private void Update()
    {
        if (!winchDisabled) crane.Hijs(winchVal);

        Vector3 movementVector = new Vector3(craneVal, 0, katVal);
        crane.Kat.MoveTransform(movementVector * Time.deltaTime);

        if (cableSwingDisabled && crane != null)
        {
            crane.Spreader.transform.localPosition = new Vector3(crane.Kat.transform.localPosition.x, crane.Spreader.transform.localPosition.y, crane.Kat.transform.localPosition.z + 1.75f);
        }

    }

    public override void OnEpisodeBegin()
    {
        if (!testMode) level = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("level_parameter", 0);
        rewardManager.level = level;

        if (level == 4) EditorApplication.isPlaying = false; //EditorApplication.ExecuteMenuItem("Edit/Play");

        // Destroy the current crane and container
        if (crane != null) Destroy(crane.gameObject);
        if (container != null) Destroy(container.gameObject);

        craneGameObject = Instantiate(cranePrefab, transform, false);
        //GameObject containerGameObject = Instantiate(containerPrefab, transform, false);
        //container = containerGameObject.GetComponent<Container>();

        crane = craneGameObject.GetComponent<Crane>();
        crane.Init();

        //crane.ResetToRandomPosition();
        crane.Spreader.agent = this;

        levelManager.SetEnvironment(level);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = inputUp.ReadValue<float>();
        continuousActions[1] = inputDown.ReadValue<float>();
        continuousActions[2] = inputLeft.ReadValue<float>();
        continuousActions[3] = inputRight.ReadValue<float>();
        continuousActions[4] = inputLift.ReadValue<float>();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<float> continousActions = actions.ContinuousActions;
        winchVal = continousActions[4] * liftSpeed;
        craneVal = (continousActions[3] - continousActions[2]) * craneSpeed;
        katVal = (continousActions[0] - continousActions[1]) * katSpeed;


        RewardData rewardData = rewardManager.GetReward(null);
        AddReward(rewardData.reward);
        if (rewardData.endEpisode) EndEpisode();
    }

    public void OnCollisionEnter(Collision collision)
    {
        RewardData rewardData = rewardManager.GetReward(collision);
        AddReward(rewardData.reward);
        if (rewardData.endEpisode) EndEpisode();
    }

    public void OnCollisionStay(Collision collision)
    {
        RewardData rewardData = rewardManager.GetReward(collision);
        AddReward(rewardData.reward);
        if (rewardData.endEpisode) EndEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Utils.Normalize(crane.Kat.transform.localPosition.x, -levelManager.PlaneHalfSize, levelManager.PlaneHalfSize));
        sensor.AddObservation(Utils.Normalize(crane.Kat.transform.localPosition.z, -levelManager.PlaneHalfSize, levelManager.PlaneHalfSize));
        sensor.AddObservation(Utils.Normalize(crane.Kat.transform.localPosition.y, 0, 50));

        sensor.AddObservation(Utils.Normalize(crane.Spreader.transform.localPosition.x, -levelManager.PlaneHalfSize, levelManager.PlaneHalfSize));
        sensor.AddObservation(Utils.Normalize(crane.Spreader.transform.localPosition.z, -levelManager.PlaneHalfSize, levelManager.PlaneHalfSize));
        sensor.AddObservation(Utils.Normalize(crane.Spreader.transform.localPosition.y, 0, 50));

        if (container != null)
        {
            sensor.AddObservation(Utils.Normalize(container.transform.localPosition.x, -levelManager.PlaneHalfSize, levelManager.PlaneHalfSize));
            sensor.AddObservation(Utils.Normalize(container.transform.localPosition.z, -levelManager.PlaneHalfSize, levelManager.PlaneHalfSize));
            sensor.AddObservation(Utils.Normalize(container.transform.localPosition.y, 0, 50));
        }
        else
        {
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
        }


        sensor.AddObservation(false); // Spreader locked or not
        sensor.AddObservation(false); // Spreader white lights        

        AddReward(-0.1f / MaxStep);
    }

    public void ReportZwier(float zwier)
    {
        //Academy.Instance.EnvironmentParameters.GetWithDefault("my_environment_parameter", 1.0f);
        Academy.Instance.StatsRecorder.Add("zwier", zwier);
    }
}
