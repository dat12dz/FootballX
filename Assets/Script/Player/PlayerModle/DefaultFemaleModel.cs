
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DefaultFemaleModel : PlayerModelBase
{

    [SerializeField] Animator animator;
    const string SELECT_ANIM_CLIP = "Selected";
    const string IDLE_ANIM_CLIP = "idle";
    const int LAYER_SELECT_ANIM = 1;

    [SerializeField]
    TeamReference redTeamRef, BlueTeamRef;
    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        ActiveModel = redTeamRef.model;
        BlueTeamRef.model.SetActive(true);
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
