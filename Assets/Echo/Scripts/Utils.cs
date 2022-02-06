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
        /// <summary>
        /// DEPRECATED
        /// </summary>
        /// <param name="body"></param>
        /// <param name="targetVelocity"></param>
        /// <param name="maxAccel"></param>
        /// <param name="forceMode"></param>
        private static void AccelerateRigidbody(Rigidbody body, Vector3 targetVelocity, float maxAccel, ForceMode forceMode = ForceMode.Acceleration)
        {
            Vector3 deltaV = targetVelocity - body.velocity;
            Vector3 accel = deltaV / Time.deltaTime;
            
            if (accel.sqrMagnitude > maxAccel * maxAccel)
                accel = accel.normalized * maxAccel;

            body.AddForce(accel, forceMode);
        }
        /// <summary>
        /// DEPRECATED
        /// </summary>
        /// <param name="body"></param>
        /// <param name="targetVelocity"></param>
        /// <param name="maxAccel"></param>
        /// <param name="timeDelta"></param>
        private static void AccelerateRigidbodyT(Rigidbody body, Vector3 targetVelocity, Vector3 maxAccel, float timeDelta)
        {            
            Vector3 accel = maxAccel * timeDelta;
            Vector3 delta = targetVelocity - body.velocity;
            if (delta.z < 0) accel *= -1;
            
            if (accel.sqrMagnitude > delta.sqrMagnitude) accel = delta;

            Vector3 vel = body.velocity;
            vel += accel;
            body.velocity = vel;
        }
        public static void AccelerateRigidbody_Z_Axis(Rigidbody body, float targetVelocity, float maxSpeed, float maxAccel, float timeDelta)
        {
            
            float accel = maxAccel * timeDelta;
            float delta = Mathf.Abs(targetVelocity - body.velocity.z);
            if (accel > delta) accel = delta;            

            Vector3 vel = body.velocity;
            if (targetVelocity < vel.z) vel.z -= accel;
            if (targetVelocity > vel.z) vel.z += accel;
            vel.z = Mathf.Clamp(vel.z, -maxSpeed, maxSpeed);
            body.velocity = vel;
        }
        public static void AccelerateRigidbody_X_Axis(Rigidbody body, float targetVelocity, float maxAccel, float timeDelta)
        {
            float accel = maxAccel * timeDelta;
            float delta = targetVelocity - body.velocity.x;
            if (delta < 0) accel *= -1;

            if (accel > delta) accel = delta;

            Vector3 vel = body.velocity;
            vel.x += accel;
            body.velocity = vel;
        }
        public static void AccelerateRigidbody_Y_Axis(Rigidbody body, float targetVelocity, float maxAccel, float timeDelta)
        {
            float accel = maxAccel * timeDelta;
            float delta = targetVelocity - body.velocity.y;
            if (delta < 0) accel *= -1;

            if (accel > delta) accel = delta;

            Vector3 vel = body.velocity;
            vel.y += accel;
            body.velocity = vel;
        }
    }
}