using System;
using System.Threading;
using UnityEngine;
namespace Assets.Utlis
{
    class Logging
    {
        
        public static void CheckNLogObjectNull(object type, string name)
        {
            if (type == null)
                Debug.LogError($"Object is null: {name},Please assign it");
        }
        public static void LogObjectNull(string name)
        {
            Debug.LogError($"Object is null: {name},Please assign it");
        }
        public static void Log(GameObject owner, string a)
        {
/*            if (MainThreadDispatcher.isMainThread())
                Debug.Log($"{owner.name}:{a}");
            else
                MainThreadDispatcher.ExecuteInMainThread(() =>
                {
                    Debug.Log($"{owner.name}:{a}");
                });*/
        }

        public static void LogError(string a)
        {
            if (MainThreadDispatcher.isMainThread())
                Debug.LogError($"{a}"); 
            else
            MainThreadDispatcher.ExecuteInFixedUpdate(() =>
            {
                Debug.LogError($"{a}");
            });
        }
        public static void Log(string a)
        {
           if (MainThreadDispatcher.isMainThread())
                Debug.Log($"{a}");
            else
                MainThreadDispatcher.ExecuteInFixedUpdate(() =>
                {
                    Debug.Log($"{a}");
                });

           
        }
        public static void Log(Exception e)
        {
            if (MainThreadDispatcher.isMainThread())
                Debug.LogException(e);
            else
                MainThreadDispatcher.ExecuteInFixedUpdate(() =>
                {
                    Debug.LogException(e);
                });
        }
    }
}
