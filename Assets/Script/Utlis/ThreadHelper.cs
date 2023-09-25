using Assets.Utlis;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
namespace Assets.Script.Utlis
{
    //using Debug = UnityEngine.Debug;
    internal class ThreadHelper
    {
     public static readonly object locker = new object();
        public static void CreateNewThread(Action a,string name = "")
        {
            Thread thread = new Thread(() => {
                try
                {
                    a();
                }
                catch (Exception e)
                {
                    Logging.Log(e);
                }
            });
            thread.Name = name;
            thread.Start();
        }
        public static void SafeThreadCall(Action a, string name = "")
        {
            if (a != null)
            {
               Task.Run(() =>
                {
              
                        try
                        { 
                            a();
                        }
                        catch (Exception e)
                        {
                            Logging.Log(e);
                        }
                });
            }
        }
        public static void WaitForSecond(CancellationTokenSource s, Action a = null, int timeout = 3000)
        {
            try
            {
                Task.Delay(timeout, s.Token).Wait();
            }
            catch
            {

            }
            if (a != null) 
            a();
        }
        public static void CloseAllIngameThread()
        {
/*            for (int i= 0; i < InGamethreads.Count; i++)
            {
                foreach(Thread thread in InGamethreads)
                try
                {
                    thread.Interrupt();
                        
                }
                catch { 
                }
                InGamethreads.Clear();
            }*/

        }
    }

}
