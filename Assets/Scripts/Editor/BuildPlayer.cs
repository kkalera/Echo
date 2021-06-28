using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;

// Output the build size or a failure depending on BuildPlayer.
[ExecuteInEditMode]
public class BuildPlayer : MonoBehaviour
{
    [MenuItem("Build/Build OSX")]
    public static void MyBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Crane Training.unity" };
        buildPlayerOptions.locationPathName = "OSXBuild";
        buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        buildPlayerOptions.options = BuildOptions.None;

        EnvSpawner spawner = GameObject.Find("Spawner").GetComponent<EnvSpawner>();
        if(spawner != null)
        {
            spawner.building = true;
            spawner.SpawnEnvironments();

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
                GameObject.Find("Spawner").GetComponent<EnvSpawner>().building = false;
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
                GameObject.Find("Spawner").GetComponent<EnvSpawner>().building = false;
            }
            
        }

        /*BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }*/
    }
}