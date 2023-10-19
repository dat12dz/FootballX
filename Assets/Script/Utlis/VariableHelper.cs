using Assets.Script.Utlis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

internal class VariableHelper
{
    public static void  TrackForVariableNotNull(Func<object> value, Action callBack)
    {
        /*    Job a = new Job() { value = value, callback = callBack };
            JobHandle jobHandle = a.Schedule(1, 1);
            jobHandle.Complete();*/
        ThreadHelper.SafeThreadCall(() =>
        {
            var loop = 100000;
            int i = 0;
            if (value() != null)
            {
                MainThreadDispatcher.ExecuteInMainThread(() =>
                {
                    callBack();
                });
                return;
            }
            while (true)
            {
                i++;
                if (value() != null || i >= loop)
                {
                    MainThreadDispatcher.ExecuteInMainThread(() =>
                    {
                        callBack();
                    });
                    break;
                }
                Thread.Sleep(10);
            }
        });
    }

    public unsafe struct Job : IJobParallelFor
    {
        public Func<object> value;
       
        public Action callback;
        public int loop;
        public void Execute(int index)
        {
        
            loop = 100000;
            int i= 0;
            if (value != null)
            {
                callback();
                return;
            }
            while (true)
            {
                i++;
                if (value != null|| i >= loop)
                {
                    callback();
                    break;
                }

            }
        }
    }
}


