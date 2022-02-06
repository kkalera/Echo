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
        public static void AccelerateRigidbodyT(Rigidbody body, Vector3 targetVelocity, Vector3 maxAccel, float timeDelta)
        {
            Vector3 accel = maxAccel * timeDelta;
            Vector3 delta = targetVelocity - body.velocity;
            if (delta.z < 0) accel *= -1;
            
            if (accel.sqrMagnitude > delta.sqrMagnitude) accel = delta;

            Vector3 vel = body.velocity;
            vel += accel;

            /*vel.x = Mathf.Clamp(vel.x, targetVelocity.x, -targetVelocity.x);
            vel.x = Mathf.Clamp(vel.z, targetVelocity.z, -targetVelocity.z);
            vel.x = Mathf.Clamp(vel.y, targetVelocity.y, -targetVelocity.y);*/


            body.velocity = vel;
        }
    }
}