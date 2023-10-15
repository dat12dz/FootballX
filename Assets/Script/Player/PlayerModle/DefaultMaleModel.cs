using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMaleModel : IPlayerModel
{
    [Serializable]
    class TeamReference
    {
        [SerializeField]
        public Material body;
        [SerializeField]
        public Material face;
    }

    [SerializeField]
    TeamReference redTeamRef, BlueTeamRef;
    [SerializeField] Animator animator;
    const string IDLE_ANIM_CLIP = "idlle";
    const string SELECT_ANIM_CLIP = "Root_Selected";
    const int SELECLAYER_LAYER = 1;
    public override void RedTeamInit()
    {
        faceRender.material = redTeamRef.face;
        bodyRender.material = redTeamRef.body;
    }

    public override void BlueTeamInit()
    {
        faceRender.material = BlueTeamRef.face;
        bodyRender.material = BlueTeamRef.body;
    }
    public override void IdleAnim()
    {
        animator.SetLayerWeight(SELECLAYER_LAYER, 0F);
        animator.Play(IDLE_ANIM_CLIP);
    }


    public override void SelectedAnim()
    {
        animator.SetLayerWeight(SELECLAYER_LAYER, 0.86F);
        animator.Play(SELECT_ANIM_CLIP, SELECLAYER_LAYER);
    }

    // Start is called before the first frame update
   
}
