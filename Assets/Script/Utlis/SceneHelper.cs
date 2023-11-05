using Assets.Script.Utlis;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
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
        /// <summary>
        /// Only run on server
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public static GameSystem GetGameSystem(Scene scene)
        {
            var allSceneObj = scene.GetRootGameObjects();
            GameSystem res = GameObject.FindAnyObjectByType<GameSystem>();
            if (!NetworkManager.Singleton.IsServer) 
                return GameSystem.instance;
            return res;              
        }
    }
}
