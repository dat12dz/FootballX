
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLipSync;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UniVRM10;

[RequireComponent(typeof(VoicelinePlayer))]
public abstract class PlayerModelBase : WaitForStart
{
    public GameObject ActiveModel;
    public uLipSync.uLipSync lipSync;
    public string ModelName;
    [SerializeField] public Animator animator;
    [SerializeField] FootIK footControl;
    public Player player;
    public SpineRef spine;
   [SerializeField] protected Transform InGameCameraPosition;
    [SerializeField]
    public Transform CameraLookAt;
    public Texture2D Thumbnail;
  [SerializeField]  VoicelinePlayer voiceline;
    public static int BLENDSHAPE_CLOSE_EYE = 14;
   public abstract void IdleAnim();
    public abstract void SelectedAnim();
    public abstract void RedTeamInit();
    public abstract void BlueTeamInit();

    public abstract Task<RenderTexture> PlayMvpAnimation();
    public abstract void WingHandRunAnim(float velocity_magnitue);

    [ContextMenu("Play close eyes")]
    public virtual void CloseEye()
    {
        
       var Skin = ActiveModel.GetComponentInChildren<SkinnedMeshRenderer>();
        DOTween.To(() => 0, (value) => Skin.SetBlendShapeWeight(BLENDSHAPE_CLOSE_EYE, value), 100, 2f).WaitForCompletion();
        DOTween.To(() => 100, (value) => Skin.SetBlendShapeWeight(BLENDSHAPE_CLOSE_EYE, value), 0, 2f).Complete();
    }
    protected override void Start()
    {
       player = transform.root.GetComponent<Player>();
        base.Start();
        lipSync = GetComponent<uLipSync.uLipSync>();
        if (player == null)
        {
          var rigs =  GetComponentsInChildren<Rig>(true);
            foreach (var rig in rigs)
            {
                rig.weight = 0;
            }
            return;
        }

        player.Playereyes.transform.position = InGameCameraPosition.position;
        if (player != null)
        {
            var playerVrm10Instace = 
            GetComponentsInChildren<MultiAimConstraint>();
            if (playerVrm10Instace.Length == 0) Debug.Log($"Không tìm thấy Aim constrain trên {name}"); 
            for (int i  = 0; i < playerVrm10Instace.Length; i++)
            {
                playerVrm10Instace[i].data.sourceObjects.Add(new WeightedTransform( player.PlayerLookAt,1));
            }
           
        }
       
    }
    float oldCamYRotation;
    float YrotateThreshold = 40;
    
    TweenerCore<Quaternion, Vector3, QuaternionOptions> tween;

   public void Rotate(bool Interpolate = true)
    {
        var CameraRotate = player.Playereyes.transform.eulerAngles.y;
        if (Mathf.Abs(CameraRotate - oldCamYRotation) > YrotateThreshold)
        {
            var RotateEndPoint = new Vector3(0, CameraRotate, 0);
            if (Interpolate)
            {
                if (tween != null)
                    tween.Kill();
                tween = ActiveModel.transform.DORotate(RotateEndPoint, 0.2f, RotateMode.Fast).SetEase(Ease.InOutQuad);
            }
            else
            {
                ActiveModel.transform.rotation = Quaternion.Euler(RotateEndPoint);
            }
            oldCamYRotation = CameraRotate;
            // Đặt chân lại
            footControl.UpdateFootPlacement();
        }
    }
 
    protected virtual void Update()
    {
        // Nếu đang ở trong game, Ngoài màn hình chọn tướng không tính
        if (player)
        { 
          Rotate();
        }
    }
    public abstract void ResetAnimation();
    public void PlayPickedSound()
    {
        try
        {
            if (voiceline) voiceline = GetComponent<VoicelinePlayer>();
              voiceline.PlayPickSound();
        }
        catch
        {

        }
    }    
}
[Serializable]
public class SpineRef
{
   public Transform head;
}
