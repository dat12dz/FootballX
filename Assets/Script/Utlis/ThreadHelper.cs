using Assets.Utlis;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
namespace Assets.Script.Utlis
{
    //using Debug = UnityEngine.Debug;
    internal class ThreadHelper
    {
        public static readonly object locker = new object();
        public static void CreateNewThread(Action a, string name = "")
        {
            Thread thread = new Thread(() =>
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
            thread.Name = name;
            thread.Start();
        }
        public static void SafeThreadCall(Action<CancellationToken> a, string name = "", bool destroyWScene = true)
        {
            var CancelToken = new CancellationTokenSource();

            if (a != null)
            {
             var t = Task.Run(
                   () =>
                   {

                       try
                       {
                           a(CancelToken.Token);
                       }
                       catch (Exception e)
                       {
                           Logging.Log(e);
                       }

                   });
                if (destroyWScene)
                { 
                    UnityAction<Scene> onSceneLoaded = null;
                    onSceneLoaded = (s1) =>
                    {
                        CancelToken.Cancel();                      
                        SceneManager.sceneUnloaded -= onSceneLoaded;
                    };
                    SceneManager.sceneUnloaded += onSceneLoaded;
                }

            }
        }
        public static async Task WaitForSecond(CancellationTokenSource s, Action a = null, int timeout = 3000)
        {
            try
            {
                await Task.Delay(timeout, s.Token);
            }
            catch
            {

            }
            if (a != null)
                a();
        }

    }

}
