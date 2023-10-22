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
    public static void  TrackForVariableNotNull(Func<object> value, Action callBack,bool ExecuteImidiate = false)
    {
   
        ThreadHelper.SafeThreadCall(() =>
        {
        
            int i = 0;
            var Value_ = value();
/*            if (Value_ is UnityEngine.Object)
            {
                HandleUnityObject(Value_ as UnityEngine.Object, callBack);

            }
            else*/
            { 
            if (Value_ != null)
            {
                    if (ExecuteImidiate)
                    {
                        MainThreadDispatcher.ExecuteInMainThreadImidiately(() =>
                        {
                            callBack();
                        });
                    }
                    else
                MainThreadDispatcher.ExecuteInMainThread(() =>
                {
                    callBack();
                });
                return;
            }
            
            while (true)
            {
                i++;
                if (Value_ != null || i >= loop)
                {
                        if (ExecuteImidiate)
                        {
                            MainThreadDispatcher.ExecuteInMainThreadImidiately(() =>
                            {
                                callBack();
                            });
                        }
                        else
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
    static async void HandleUnityObject(UnityEngine.Object obj_, Action callBack)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        int i = 0;
            MainThreadDispatcher.ExecuteInMainThread(() =>
            {
                if (obj_)
                {
                    callBack();

                    cancellationTokenSource.Cancel();
                }
            });
        Task t = Task.Delay(9000, cancellationTokenSource.Token);
        try
        {
            await t;
        }
        catch { 
        
        }
        if (!t.IsCanceled)
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
    
    
}


