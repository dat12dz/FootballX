
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class DefaultFemaleModel : PlayerModelBase
{


    const string SELECT_ANIM_CLIP = "Selected";
    const string IDLE_ANIM_CLIP = "idle";
    const int LAYER_SELECT_ANIM = 1;
    public AnimationState idle, RunninghandAnim;
    [SerializeField]
    TeamReference redTeamRef, BlueTeamRef;
    private void Awake()
    {
  
        idle = new AnimationState(IDLE_ANIM_CLIP,animator);
    }
    protected override void Start()
    {
        base.Start();
    }
    [ContextMenu("PlayIdle")]
    public override void IdleAnim()
    {
        animator.SetLayerWeight(LAYER_SELECT_ANIM, 0);
        animator.PlayInFixedTime(IDLE_ANIM_CLIP);
    }
    [ContextMenu("Play Selection animation")]
    public override void SelectedAnim()
    {
        animator.SetLayerWeight(LAYER_SELECT_ANIM, 0.84f);
        animator.PlayInFixedTime(SELECT_ANIM_CLIP,1);
    }
    public override void RedTeamInit()
    {
        redTeamRef.model.SetActive(true);
        ActiveModel = redTeamRef.model;
        animator = redTeamRef.model.GetComponent<Animator>();
        BlueTeamRef.model.SetActive(false);
    }
    void init(GameObject model)
    {

    }
    public override void BlueTeamInit()
    {
        redTeamRef.model.SetActive(false);
        animator = BlueTeamRef.model.GetComponent<Animator>();
        ActiveModel = BlueTeamRef.model;
        BlueTeamRef.model.SetActive(true);
    }
    const string RUNNING_WING_HAND_ANIM = "VelMagnitue";
    public override void WingHandRunAnim(float velocity_magnitue)
    {
        animator.SetFloat(RUNNING_WING_HAND_ANIM, velocity_magnitue);
    }
    void CloseEye()
    {
        float x = 0;
       
      //  DOTween.To(() => x,)
    }
    protected override void Update()
    {
        base.Update();
    }

    [Serializable]
    class TeamReference
    {
        public GameObject model;
    }

}
