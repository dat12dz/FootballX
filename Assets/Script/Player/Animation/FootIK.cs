using Assets.Script.Utlis;
using Assets.Utlis;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Mono.CSharp;
using System;
using UnityEngine;


public class FootIK : MonoBehaviour
{

    // Start is called before the first frame update
    [SerializeField] PlayerModelBase baseModel;
    [SerializeField] LayerMask groundMask;
    [SerializeField] GameObject Raycaster, Hint;
    [SerializeField] FootRef LFoot, RFoot;
    [SerializeField] AnimationCurve FootPlaceMentCurve;
    [SerializeField] SoundPlayer SoundPlayer_StepOnGrass;
    Player player;

    Transform spine;
    void Start()
    {

        groundMask = LayerMask.GetMask("Ground");
        player = transform.root.GetComponent<Player>();
        if (player != null)
        {

            var rotate = player.GetComponentInChildren<Rotate>();
            Hint.transform.parent = rotate.HeadObj;
            Raycaster.transform.parent = rotate.HeadObj;

            spine = baseModel.animator.GetBoneTransform(HumanBodyBones.Hips);

            LFoot.hint.transform.SetParent(spine, false);
            RFoot.hint.transform.SetParent(spine, false);



            player.isInGame.OnValueChanged += (old, curr) =>
            {
                if (curr)
                {

                    LFoot.footIK.transform.parent = null;
                    RFoot.footIK.transform.parent = null;

                }
                else
                {
                    LFoot.footIK.transform.parent = transform;
                    RFoot.footIK.transform.parent = transform;
                }

            };
            LFoot.footIK.transform.parent = null;
            RFoot.footIK.transform.parent = null;
        }


    }
    [SerializeField] float Spacing = 0.2f;
    bool FootPlacement(FootRef foot, float distanceCheck = 0)
    {
        RaycastHit hitInfo;
        var physisScene = gameObject.scene.GetPhysicsScene();
        bool isSucess;
        if (physisScene != null)
        {
            isSucess = physisScene.Raycast(foot.Raycaster.position, Vector3.down, out hitInfo, 3, groundMask);
        }
        else
        {
            isSucess = Physics.Raycast(foot.Raycaster.position, Vector3.down, out hitInfo, 3, groundMask);
        }
        if (isSucess && hitInfo.collider != null)
        {
            if (MathHelper.DistanceNoSqrt(hitInfo.point, foot.LastFootPlaceMent) >= distanceCheck * distanceCheck)
            {
                Vector3 FootPlacePoint;
                FootPlacePoint = hitInfo.point;
                Vector3 PlayerVel()
                {
                    return player.Vel;
                }
                if (hitInfo.point.y < spine.position.y)
                {

                    foot.StartInterpolated(foot.footIK.transform.position, hitInfo.point + PlayerVel() * Spacing);

                    foot.LastFootPlaceMent = FootPlacePoint;
                }
                foot.footIK.transform.rotation = Quaternion.Euler(baseModel.ActiveModel.transform.eulerAngles);
                return true;
            };
        }
        return false;
    }

    [SerializeField] float RunningTimeIncSpeed = 0.04f;
    float runningTimes;
    bool Interpolating;
    public void UpdateFootPosition()
    {
        var CurrentRate = runningTimes % 2;
        if (Interpolating)
        {
            if (CurrentRate > 1)
            {
                LFoot.currentRate = CurrentRate - 1;
                LFoot.Interpolater(FootPlaceMentCurve, SoundPlayer_StepOnGrass);
            }
            if (CurrentRate < 1)
            {
                RFoot.currentRate = CurrentRate;
                RFoot.Interpolater(FootPlaceMentCurve, SoundPlayer_StepOnGrass);
    
            }
            runningTimes += RunningTimeIncSpeed * Time.deltaTime;
        }
       

        if (player.Vel.sqrMagnitude > 0.0001f)
        {
            Interpolating = true;
            FootPlacement(LFoot, DefaultFootPlacementDistance);
            FootPlacement(RFoot, DefaultFootPlacementDistance);
          
        }
        else
        {
            if (runningTimes >= 2)
            {
                Interpolating = false;
            }
          
        }

    }
    void SetupInterpolate()
    {
        if (runningTimes > 2)
        {
            Interpolating = true;
            runningTimes = 0;
        }

    }
    TweenerCore<float, float, FloatOptions> task;
    public void UpdateFootPlacement()
    {
        /*        Hint.transform.DORotate(baseModel.ActiveModel.transform.eulerAngles, 0.2f);
                Raycaster.transform.DORotate(baseModel.ActiveModel.transform.eulerAngles,0.2f).onComplete += () => {


                };*/
        SetupInterpolate();
        FootPlacement(LFoot, 0);
        FootPlacement(RFoot, 0);
    }


    [SerializeField] float DefaultFootPlacementDistance = 3.5f;

    Vector3 lastPos;
    bool isStopped_;
    bool isStopped
    {
        get { return isStopped_; }
        set
        {
            if (value != isStopped_)
            {
                isStopped_ = value;
                if (value)
                {
                    OnStopped();
                }
            }
        }
    }
    void OnStopped()
    {
        UpdateFootPlacement();//t
    }
    private void Update()
    {
        if (player == null) return;
        player.Vel = transform.position - lastPos;
        VariableHelper.TrackForVariableNotNull(() => baseModel.animator, () => baseModel.WingHandRunAnim(player.Vel.magnitude), true);

        lastPos = transform.position;
        UpdateFootPosition();
        isStopped = player.Vel.magnitude < 0.01f;
        if (!isStopped)
        {
    
            baseModel.Rotate(false);
        }
    }
}
[Serializable]

class FootRef
{

    public Transform hint;
    public Transform Raycaster;
    public Transform footIK;
    public Vector3 LastFootPlaceMent;
    public static bool Rfoot;
    Vector3 StartPosition, Destination;
    Vector2 CurrentXZ;
    float CurrentY;
    public float currentRate;
    public float incRateSpeed = 5f;

    public float Amp = 0.3f;
    bool Stepped;
    public void StartInterpolated(Vector3 StartPos, Vector3 des)
    {
        if (currentRate > 0.90)
        {
            StartPosition = StartPos; Destination = des;
            currentRate = 0;
        }
        else
        {
            Destination = des;
        }
    }
    float currentTime;
    public static bool isLFoot;
    public void Interpolater(AnimationCurve curve, SoundPlayer soundPlayer)
    {
        if (currentRate > 0.90)
        {
            isLFoot = !isLFoot;
            Destination = StartPosition;

            soundPlayer.PlayRandomSound();
            return;
        }

       
      
        var StartPosXZ = new Vector2(StartPosition.x, StartPosition.z);
        var DestinationXZ = new Vector2(Destination.x, Destination.z);
        CurrentXZ = Vector2.Lerp(StartPosXZ, DestinationXZ, currentRate);    
        CurrentY = StartPosition.y + (Destination.y - StartPosition.y) * currentRate;
        var CurvedY = CurrentY + Mathf.Sin(currentRate * MathF.PI) * Amp;
        try
        {
            footIK.transform.position = new Vector3(CurrentXZ.x, CurvedY, CurrentXZ.y);
        }
        catch
        {

        }
    }


}