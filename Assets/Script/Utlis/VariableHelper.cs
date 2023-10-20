using Assets.Script.Utlis;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Jobs;


internal class VariableHelper
{
  static  int loop = 100000;
    public static async Task WaitForVariableNotNullAsync(Func<object> value,int TimeoutInMs = 50000, Action callBack = null)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        TrackForVariableNotNull(value,() => { 
             
                cancellationTokenSource.Cancel();
            if (callBack != null)
                callBack();

        });
        try
        {
            await Task.Delay(TimeoutInMs, cancellationTokenSource.Token);
        }
        catch
        {
            //
        }
    }
    public static void  TrackForVariableNotNull(Func<object> value, Action callBack)
    {
        /*    Job a = new Job() { value = value, callback = callBack };
            JobHandle jobHandle = a.Schedule(1, 1);
            jobHandle.Complete();*/
        ThreadHelper.SafeThreadCall(() =>
        {
        
            int i = 0;
            var Value_ = value();
            if (Value_ is UnityEngine.Object)
            {
                HandleUnityObject(Value_ as UnityEngine.Object, callBack);
              
            }
            else
            { 
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
            }
        });
    }
    static void HandleUnityObject(UnityEngine.Object obj_, Action callBack)
    {
       
        int i = 0;
        bool Continue = true;
            MainThreadDispatcher.ExecuteInMainThread(() =>
            {
                if (obj_)
                {
                    callBack();
                    lock (new object())
                    {
                        Continue = false;
                    }
                }
            });
        if (Continue)
        while (true)
        {
            i++;
            if (obj_ || i >= loop)
            {
                MainThreadDispatcher.ExecuteInMainThread(() =>
                {
                    callBack();
                });
                break;
            }
            Thread.Sleep(10);
        }
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


