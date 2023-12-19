using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


namespace Assets.Script.ExtensionMethod
{
    static class AnimatorExtension
    {
        public static async Task PlayAndWait(this Animator animator,string AnimName,int layer = 0)
        {
            animator.Play(AnimName, layer);
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            float ClipLength = 0;
            foreach (var clip in clips)
            {
                if (clip.name == AnimName)
                {
                    ClipLength = clip.length;
                    break;
                }
            }           
            await Task.Delay((int)(ClipLength * 1000));
        }
        public static async Task WaitForFrame(this Animator animator,int Frame)
        {
            var v = animator.GetCurrentAnimatorStateInfo(0);
        }
    }
}
