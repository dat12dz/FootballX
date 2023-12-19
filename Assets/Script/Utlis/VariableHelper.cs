using Assets.Script.Utlis;
using Mono.CSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


internal class VariableHelper
{
    static int loop = 100000;
    public static async Task WaitForVariableNotNullAsync(Func<object> value, int TimeoutInMs = 50000, Action callBack = null)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        TrackForVariableNotNull(value, () =>
        {
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
    public static void TrackForVariableNotNull(Func<object> value, Action callBack, bool ExecuteImidiate = false)
    {

        ThreadHelper.SafeThreadCall((cancl) =>
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
    class getterNActionOnObjectChange
    {
        public object oldVal;
        public Func<object> getter;
        public Action<object> OnObjectChange;
    }
    static ConcurrentBag<getterNActionOnObjectChange> getterList = new ConcurrentBag<getterNActionOnObjectChange>();

    public static Thread CheckVariableChange_Thread = new Thread(() =>
    {

        while (true)
        {

            for (int i = 0; i < getterList.Count; i++)
            {
                var ithGetter = getterList.ElementAt(i);
                var getter_new_value = ithGetter.getter();
                if (!getter_new_value.Equals(ithGetter.oldVal))
                {
                    ithGetter.OnObjectChange(getter_new_value);
                    ithGetter.oldVal = getter_new_value;
                }

            }
            Thread.Sleep(1000);
        }
    });
    /// <summary>
    /// Kiểm tra mỗi 1 giây xem biến có được thay đổi hay không , nếu bị thay đổi thì thực hiện hàm OnObjectChange với tham số truyền vào là giá trị mới thay đổi
    /// </summary>
    /// <param name="getter">hàm để lấy giá trị biến ví dụ () => objectName </param>
    /// <param name="OnObjectChange">Chương trình sẽ gọi hàm nay khi biến bị thay đổi thành bất kì giá trị khác. Ví dụ : (newValue)=>{ //  
    /// newValue là giá trị mới của biến sau khi bị thay đổi, hàm được gọi trên luồng khác nên tránh sử dụng Unity Object trong hàm  
    /// }</param>
    public static void CheckVariableChange(Func<object> getter, Action<object> OnObjectChange)
    {
        getterList.Add(new getterNActionOnObjectChange() { getter = getter, OnObjectChange = OnObjectChange });
    }


}


