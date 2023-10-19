using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


    public class AnimationState
    {
        string AnimName;
        Animator animator;
        protected int layer;
        public AnimationState(string AnimName_, Animator t)
        {
            AnimName = AnimName_;
            animator = t;
        }
        void PlayAnim()
        {
            animator.Play(AnimName);
            var b = animator.GetCurrentAnimatorStateInfo(layer);

        }
        async void PlayAnimAsync()
        {
            animator.Play(AnimName);
            var b = animator.GetCurrentAnimatorStateInfo(layer);
            await Task.Delay((int)(1000 * b.length));
        }
    }
    class FaceAnimState : AnimationState
    {
        public FaceAnimState(string AnimName_, Animator t) : base(AnimName_, t)
        {
            layer = 2;
        }
    }

