using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;

namespace Echo
{
    public static class Utils
    {

        private static MethodInfo _clearConsoleMethod;
        private static MethodInfo clearConsoleMethod
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
        public static void AccelerateRigidbody(Rigidbody body, Vector3 targetVelocity, float maxAccel, ForceMode forceMode = ForceMode.Acceleration)
        {
            Vector3 deltaV = targetVelocity - body.velocity;
            Vector3 accel = deltaV / Time.deltaTime;
            
            if (accel.sqrMagnitude > maxAccel * maxAccel)
                accel = accel.normalized * maxAccel;

            body.AddForce(accel, forceMode);
        }
        public static void AccelerateRigidbodyT(Rigidbody body, Vector3 targetVelocity, Vector3 maxAccel, float timeDelta, ForceMode forceMode = ForceMode.Acceleration)
        {
            Vector3 accel = maxAccel * timeDelta;
            if(accel.sqrMagnitude > targetVelocity.sqrMagnitude)
            {
                accel = targetVelocity;
            }
            body.velocity += accel;
        }
    }
}