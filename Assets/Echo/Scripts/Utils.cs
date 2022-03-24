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
        public static void AccelerateRigidbody_X_Axis(Rigidbody body, float targetVelocity, float maxSpeed ,float maxAccel, float timeDelta)
        {
            float accel = maxAccel * timeDelta;
            float delta = Mathf.Abs(targetVelocity - body.velocity.x);
            if (accel > delta) accel = delta;

            Vector3 vel = body.velocity;
            if (targetVelocity < vel.x) vel.x -= accel;
            if (targetVelocity > vel.x) vel.x += accel;
            vel.x = Mathf.Clamp(vel.x, -maxSpeed, maxSpeed);
            body.velocity = vel;
        }
        public static void AccelerateRigidbody_Y_Axis(Rigidbody body, float targetVelocity, float maxSpeed, float maxAccel, float timeDelta)
        {
            float accel = maxAccel * timeDelta;
            float delta = Mathf.Abs(targetVelocity - body.velocity.y);
            if (accel > delta) accel = delta;

            Vector3 vel = body.velocity;
            if (targetVelocity < vel.y) vel.y -= accel;
            if (targetVelocity > vel.y) vel.y += accel;
            vel.z = Mathf.Clamp(vel.y, -maxSpeed, maxSpeed);
            body.velocity = vel;
        }

        public static float Normalize(float val, float min, float max)
        {
            return ((val - min) / (max - min));
        }
        public static float DeNormalize(float val, float min, float max)
        {
            return (val *(max-min)) + min;
        }
    }
}