using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Assets.Utlis;
using Assets.Script.Utlis;
using System.Collections.Concurrent;
using Unity.VisualScripting;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher instance;

    static ConcurrentQueue<Action> ac = new ConcurrentQueue<Action>();
    static ConcurrentQueue<Action> FixUpdateac = new ConcurrentQueue<Action>();
    static Action ImidiatelyAction;
    
    public const string MAIN_THREAD_NAME = "M";
    PhysicsScene physic;
    public static bool isMainThread()
    {
        return Thread.CurrentThread.Name == MAIN_THREAD_NAME;
    }
    private void Awake()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        // ----------ALL CODE BELOW THHIS LINNE----------
        try
        {
            Thread.CurrentThread.Name = MAIN_THREAD_NAME;
        }
        catch { 
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (ImidiatelyAction != null)
        {
            ImidiatelyAction();
            ImidiatelyAction = null;
        }
        if (ac.Count > 0) 
        {
            Action performAc;
             ac.TryDequeue(out performAc);
            try
            {
                performAc();
            }
           catch (Exception ex)
            {
                Debug.LogError("Lỗi k thể chạy funnction:" + performAc.Method.Name);
                Debug.LogException(ex);
            }
           
        }
    }
   static object  lockobj = new object();
    /// <summary>
    /// slow but manage thread call
    /// </summary>
    /// <param name="action"></param>
    public static void ExecuteInMainThread(Action action)
    {
        if (isMainThread()) action();
        else
        {

            ac.Enqueue(action);
        }
       

    }
    public static void ExecuteInFixedUpdate(Action action)
    {
        if (isMainThread()) action();
        else
        {
            FixUpdateac.Enqueue(action);
        }
    }
    private void FixedUpdate()
    {
        Action performAc;
        FixUpdateac.TryDequeue(out performAc);
        try
        {
            if (performAc != null)
            performAc();
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi k thể chạy funnction:" + performAc.Method.Name);
            Debug.LogException(ex);
        }
    }
    /// <summary>
    /// Fast and unmanage main thread call
    /// </summary>
    /// <param name="action"></param>
    public static void ExecuteInMainThreadImidiately(Action action)
    {
        if (isMainThread()) action();
        else
        {
            ImidiatelyAction += () =>
            {
                try
                {
                    

                    action();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

            };

        }
    }
    public static void DeleteQueque()
    {
        ac.Clear();
    }
    private void OnApplicationQuit()
    {
        
    }
}
