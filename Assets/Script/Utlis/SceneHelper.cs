using Assets.Script.Utlis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Assets.Script.NetCode
{
    internal class SceneHelper
    {
        public static void WaitForSceneLoaded(int index)
        {
            CancellationTokenSource c = new CancellationTokenSource();
            UnityAction<Scene, LoadSceneMode> WhenLoaded = null;
            WhenLoaded = (s, l) =>
            {
                c.Cancel();
                SceneManager.sceneLoaded -= WhenLoaded;
            };
            MainThreadDispatcher.ExecuteInMainThread(() =>
            {
                SceneManager.LoadScene(index);
            });
           SceneManager.sceneLoaded += WhenLoaded;
            ThreadHelper.WaitForSecond(c, null, 60000);
        }
        public static GameSystem GetGameSystem(Scene scene)
        {
            var allSceneObj = scene.GetRootGameObjects();
            GameSystem res = allSceneObj[0].GetComponent<GameSystem>();
            return res;
        }
    }
}
