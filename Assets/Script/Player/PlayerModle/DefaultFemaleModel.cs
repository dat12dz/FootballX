
using Assets.Script.ExtensionMethod;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;


public class DefaultFemaleModel : PlayerModelBase
{


    const string SELECT_ANIM_CLIP = "Selected";
    const string IDLE_ANIM_CLIP = "idle";
    const int LAYER_SELECT_ANIM = 1;
    const string PLAYER_MVP_ANIM_CLIP = "MVPPose";
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
    [Header("MVP Animation")]
   [SerializeField] Camera MVPCam,PlayerRenderCam;
    Transform R_hand;
    [SerializeField] Transform Cup;
    float testHoldAnim = 1.566667f;
    [ContextMenu("Play MVP Anim")]
    public override async Task<RenderTexture> PlayMvpAnimation()
    {
      if (player)
        player.Playereyes.gameObject.SetActive( false);
        MVPCam.transform.SetParent(ActiveModel.transform, false);
        MVPCam.gameObject.SetActive(true);
        Cup.gameObject.SetActive(true);
        R_hand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        animator.GetComponentInChildren<Rig>().weight = 0;
        animator.Play(PLAYER_MVP_ANIM_CLIP, 0);
        var CamAnimator = MVPCam.GetComponent<Animator>();
        var t =  CamAnimator.PlayAndWait("Camera|CameraAction", 0);
        await Cup.DOMoveY(Cup.position.y -10, testHoldAnim).AsyncWaitForCompletion();        
        Cup.SetParent(R_hand, false);
        Cup.transform.localPosition =new Vector3(0.0266f, -0.0225f, 0.0204f);
        Cup.transform.localRotation = Quaternion.Euler((Vector3.zero));
        await t;
        return TakePicture(PlayerRenderCam);
    }
    [SerializeField] RenderTexture texture;
    RenderTexture TakePicture(Camera cam)
    {
/*        RenderTexture CurrentRT = RenderTexture.active;
         
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height,24,RenderTextureFormat.ARGB32);
        rt.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;
        rt.dimension = TextureDimension.Tex2D;
        cam.targetTexture = rt;*/
        cam.targetTexture = texture;
        /*        RenderTexture.active = rt;
                Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);


                cam.Render();
                screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

                screenShot.Apply();

                RenderTexture.active = CurrentRT;
                cam.targetTexture = null;
                Destroy(rt);
                return screenShot;*/
        return texture;
        
    }

    [Serializable]
    class TeamReference
    {
        public GameObject model;
    }

}
