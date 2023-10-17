using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UniVRM10;


public abstract class PlayerModelBase : WaitForStart
{
    public GameObject ActiveModel;
    [SerializeField] FootIK footControl;
    public Player player;
    public SpineRef spine;
   [SerializeField] protected Transform InGameCameraPosition;
    [SerializeField]
    public Transform CameraLookAt;
    public Texture2D Thumbnail;
   public abstract void IdleAnim();
    public abstract void SelectedAnim();
    public abstract void RedTeamInit();
    public abstract void BlueTeamInit();

    protected override void Start()
    {
       player = transform.root.GetComponent<Player>();
        base.Start();
        if (player == null) return;

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
    void UpdateFootPlacement()
    {
       
    }
    protected virtual void Update()
    {
        // Nếu đang ở trong game, Ngoài màn hình chọn tướng không tính
        if (player)
        { 
          Rotate();
        }
    }
}
[Serializable]
public class SpineRef
{
   public Transform head;
}
class AnimationState
{
    string AnimName;
    Animator animator;
    protected int layer;
    public AnimationState(string AnimName_ ,Animator t)
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