using Assets.Script.Utlis;
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
        static Vector3 LastScreenPos;
        public static Vector2 ConvertToCanvasSpace(Canvas c, Vector3 WorldPos)
        {
            var CameraRenering = Camera.allCameras[0];
            //first you need the RectTransform component of your canvas
            RectTransform CanvasRect = c.GetComponent<RectTransform>();
            //then you calculate the position of the UI element
            //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
            Vector2 ViewportPosition = CameraRenering.WorldToViewportPoint(WorldPos, Camera.MonoOrStereoscopicEye.Mono);
            Vector3 CamToWorldPos = WorldPos - CameraRenering.transform.position;
            Ray viewRay = CameraRenering.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.width / 2));
            if (MathHelper.Angle(viewRay.direction, CamToWorldPos) > 90)
            {
                Vector2 GetCornerOfScreen(int corID)
                {
                    switch (corID)
                    {
                        case 0:
                            return new Vector2(-1, 1);
                            break;
                        case 1:
                            return new Vector2(-1, -1);
                            break;
                        case 3:
                            return new Vector2(1, -1);
                            break;
                    }
                    return default;
                }
                var withhigh = new Vector2(Screen.width, Screen.height)/2;
                var res1 = MathHelper.FindIntersectionPoint(Vector2.zero, (Vector2)CamToWorldPos, GetCornerOfScreen(0) * withhigh, Vector2.right);
                var res2 = MathHelper.FindIntersectionPoint(Vector2.zero, (Vector2)CamToWorldPos, GetCornerOfScreen(1) * withhigh, Vector2.right);
                var res3 = MathHelper.FindIntersectionPoint(Vector2.zero, (Vector2)CamToWorldPos, GetCornerOfScreen(1) * withhigh, Vector2.up);
                var res4 = MathHelper.FindIntersectionPoint(Vector2.zero, (Vector2)CamToWorldPos, GetCornerOfScreen(3) * withhigh, Vector2.up);
                if (res1.HasValue)
                    return res1.Value;
                if (res2.HasValue)
                    return res2.Value; 
                if (res3.HasValue)
                    return res3.Value; 
                if (res4.HasValue)
                    return res4.Value;
            }
            Vector2 WorldObject_ScreenPosition = new Vector2(
               Mathf.Clamp(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)), -Screen.width / 2, Screen.width / 2),
                Mathf.Clamp(
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)), -Screen.height / 2, Screen.height / 2));
            LastScreenPos = WorldObject_ScreenPosition;
            //now you can set the position of the ui element
            return WorldObject_ScreenPosition;

        }
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
        public static GameSystem cache;
        /// <summary>
        /// Only run on server
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public static async Task<GameSystem> GetGameSystem(Scene scene)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                await VariableHelper.WaitForVariableNotNullAsync(() => GameSystem.instance);
                return GameSystem.instance;
            }
            if (cache)
                return cache;
            var allSceneObj = scene.GetRootGameObjects();
            for (int i = 0; i < allSceneObj.Length; i++)
            {

                var ithObj = allSceneObj[i];
                var gst = ithObj.GetComponent<GameSystem>();
                if (gst)
                {
                    cache = gst;
                    return gst;
                }

            }
            return null;
        }
        public static void TurnOffAllCamera()
        {

        }
    }
}
