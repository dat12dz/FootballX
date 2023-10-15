
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DefaultFemaleModel : IPlayerModel
{

    [SerializeField] Animator animator;
    const string SELECT_ANIM_CLIP = "Selected";
    const string IDLE_ANIM_CLIP = "idle";
    const int LAYER_SELECT_ANIM = 1;
    [SerializeField]
    SkinnedMeshRenderer faceRender,bodyRender;
    [SerializeField]
    TeamReference redTeamRef, BlueTeamRef;
    void Start()
    {
        animator = GetComponent<Animator>();
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
        faceRender.material = redTeamRef.face;
        bodyRender.material = redTeamRef.body;
    }

    public override void BlueTeamInit()
    {
        faceRender.material = BlueTeamRef.face;
        bodyRender.material = BlueTeamRef.body;
    }
    void CloseEye()
    {
        float x = 0;
       
      //  DOTween.To(() => x,)
    }
    void Update()
    {
        
    }

    [Serializable]
    class TeamReference
    {
        [SerializeField]
       public Material body;
        [SerializeField]
        public Material face;
    }

}
