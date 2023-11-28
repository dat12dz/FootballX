using Assets.Script.ExtensionMethod;
using Assets.Utlis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering.Universal;

public class DefaultMaleModel : PlayerModelBase
{
    [Serializable]
    class TeamReference
    {
        public GameObject model;
    }

    [SerializeField]
    TeamReference redTeamRef, BlueTeamRef;
  
    const string IDLE_ANIM_CLIP = "idlle";
    const string SELECT_ANIM_CLIP = "Root_Selected";
    const int SELECLAYER_LAYER = 1;
    public override void RedTeamInit()
    {
        redTeamRef.model.SetActive(true);
        animator = redTeamRef.model.GetComponent<Animator>();
        ActiveModel = redTeamRef.model;
        BlueTeamRef.model.SetActive(false);
    }
    
    public override void BlueTeamInit()
    {
        redTeamRef.model.SetActive(false);
        animator = BlueTeamRef.model.GetComponent<Animator>();
        ActiveModel = BlueTeamRef.model;
        BlueTeamRef.model.SetActive(true);
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
    protected override void Start()
    {
        base.Start();
        mvpAnimation.OnCleanUp +=() =>  {
            cup.SetActive(false);
            var rig = player.GetComponentInChildren<Rig>();
            rig.weight = 1;
        };
        tex =  new RenderTexture(Screen.width, Screen.height, 24);
    }

    const string RUNNING_WING_HAND_ANIM = "VelMagnitue";
    public override void WingHandRunAnim(float velocity_magnitue)
    {
        animator.SetFloat(RUNNING_WING_HAND_ANIM, velocity_magnitue);
    }
    [Header("MVP animation")]
    [SerializeField] float CupAppearTimes;
    [SerializeField] GameObject cup;
    [SerializeField]
    MVPAnimationRef mvpAnimation;
    RenderTexture tex;
    [ContextMenu("Play MVP animation")]

    public override async Task<RenderTexture> PlayMvpAnimation()
    {
     
        Camera GetPlayereye()
        {
            if (player)
            {
                return player.Playereyes;
            } 
                return null;
        }
      var rig =  player.GetComponentInChildren<Rig>();
        rig.weight = 0;
        try
        {
            mvpAnimation.MvpAnimationRoot.localRotation = ActiveModel.transform.localRotation;
            ActiveModel.transform.localPosition = Vector3.zero;
            var t =  mvpAnimation.Play(animator, GetPlayereye(),true);
            await Task.Delay((int)(CupAppearTimes * 1000));
            cup.SetActive(true);
            await t;
            mvpAnimation.Player_ShowingCam.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Base;
            mvpAnimation.Player_ShowingCam.clearFlags = CameraClearFlags.Nothing;
            mvpAnimation.Player_ShowingCam.targetTexture = tex;
            return tex;
        }
        catch (Exception e) 
        {
            Debug.LogException(e);
        }

        return default;
    }
    [ContextMenu("Reset")]
    public override void ResetAnimation()
    {
        mvpAnimation.Player_ShowingCam.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
        
        mvpAnimation.Player_ShowingCam.targetTexture = null;
        mvpAnimation.CleanUp();
    }
}
