using UnityEditor;
using System.Reflection;
using System;
using Unity.MLAgents;
using UnityEngine;

public static class Utils
{
    static MethodInfo _clearConsoleMethod;
    static MethodInfo clearConsoleMethod
    {
        get
        {
            if (_clearConsoleMethod == null)
            {
#if UNITY_EDITOR
                Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
                Type logEntries = assembly.GetType("UnityEditor.LogEntries");
                _clearConsoleMethod = logEntries.GetMethod("Clear");
#endif
            }
            return _clearConsoleMethod;
        }
    }

    public static void ClearLogConsole()
    {
        clearConsoleMethod.Invoke(new object(), null);
    }

    public static float Normalize(float val, float min, float max)
    {
        return Mathf.Clamp(((val - min) / (max - min)), 0, 1);
    }

    public static void ReportStat(float value, string name)
    {
        var statsRecorder = Academy.Instance.StatsRecorder;
        statsRecorder.Add(name, value);
    }
    public static void StopSimulation()
    {
        //EditorApplication.isPlaying = false; //EditorApplication.ExecuteMenuItem("Edit/Play");
        Application.Quit();
    }
}