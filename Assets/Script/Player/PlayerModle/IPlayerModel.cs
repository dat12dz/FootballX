using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UniVRM10;
public abstract class IPlayerModel : WaitForStart
{

    public Player player;
    public SpineRef spine;
    protected 
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
        if (player != null)
        {
            var playerVrm10Instace = 
            GetComponentsInChildren<MultiAimConstraint>();
            if (playerVrm10Instace.Length == 0) Debug.Log($"Không tìm thấy Aim constrain trên {name}"); 
            for (int i  = 0; i < playerVrm10Instace.Length; i++)
            {
                playerVrm10Instace[i].data.sourceObjects.Add( new WeightedTransform( player.PlayerLookAt,1));
            }
           
        }
        base.Start();
    }
}

[Serializable]
public class SpineRef
{
   public Transform head;
}