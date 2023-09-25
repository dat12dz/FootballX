using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Utlis
{
    internal class PostProcessingHelper
    {
        public static PostProcessingHelper Instance { get; set; }
        public PostProcessingHelper() 
        {
            
        }
        public static void ToggleNoSaturation(Volume v,bool t)
        {
            if (v)
            MainThreadDispatcher.ExecuteInMainThread(() => {
                ColorAdjustments blackAndWhite = new ColorAdjustments();
                v.profile.TryGet(out blackAndWhite);
                blackAndWhite.saturation.overrideState = t;
            });
        }
    }
}
