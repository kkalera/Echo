using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

// Output the build size or a failure depending on BuildPlayer.
[ExecuteInEditMode]
public class BuildPlayer : MonoBehaviour
{
    [MenuItem("Build/Build OSX")]
    public static void BuildOSX()
    {
        Build("Build/OSX", BuildTarget.StandaloneOSX);
    }

    [MenuItem("Build/Build Windows")]
    public static void BuildWindows()
    {
        Build("Build/crane.exe", BuildTarget.StandaloneWindows);
    }

    private static void Build(string pathname, BuildTarget target)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        string sceneName = SceneManager.GetActiveScene().name;
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/"+sceneName+".unity" };
        buildPlayerOptions.locationPathName = pathname;
        buildPlayerOptions.target = target;
        buildPlayerOptions.options = BuildOptions.None;

        EnvSpawner spawner = GameObject.Find("Spawner").GetComponent<EnvSpawner>();
        if (spawner != null)
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
    }

}