﻿using Assets.Script;
using Assets.Utlis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
public partial class Player
{
    NetworkVariable<bool> isGrabed= new NetworkVariable<bool>();
    [SerializeField] TextMeshPro NameTag_;
    [ServerRpc] 
    void UpdateVollyRuntime_ServerRpc(float curr)
    {
        VolleyRuntime = curr;
    }
    [ServerRpc]
    public void GrabItemOrThrowServerRpc(float ThrowForce = 0)
    {
        if (!grabitem && isGoalKeeper) // if you are not the goalkeeper so notdoing anything
        {

            RaycastHit r;
            
            thisPhysicScene.Raycast(Playereyes.transform.position, Playereyes.transform.forward, out r, LookatDistance, lookatMask);
            if (r.collider != null)
            {
                CatchedBallTimes.Value++;
                Grabable GrabAbleItem = r.collider.GetComponent<Grabable>();
                Grab(GrabAbleItem,true);            
            }
            else
            {          
                Logging.Log("Không thể nhặt được item này"); 
            }
        }
      
        else // throw item
        {
            Throw(ThrowForce);
        }
        isGrabed.Value = grabitem != null;
    }
    public Action<Grabable> OnThrowSomeThing;
    public Action OnShootBall;
    public void Grab(Grabable GrabAbleItem,bool isPlayerAction = false)
    {

        if (GrabAbleItem != null && !GrabAbleItem.isGrab() &&  CanGrab && GrabAbleItem.Grab(Graber, isPlayerAction))
        {
            System.MatchAction.GoalKeeperCatchTheBall(this);
            grabitem = GrabAbleItem;
            if (GrabAbleItem is Ball)
            {
                var ball = (Ball)GrabAbleItem;
                ball.lastToucher = this;
            }
        }
    }
   
    public void Throw(float Force)
    {
        if (grabitem != null)
            grabitem.Throw(Playereyes.transform.forward * Force);
        grabitem = null;
        if (OnThrowSomeThing != null)
        {
            OnThrowSomeThing(grabitem);
        }
    }
    [ServerRpc]
    public void SendShootForceServerRpc(float shootForce)
    {
        FinalShootForce = shootForce;
    }
    [ServerRpc] 
    public void SendPlayerNameToServerRpc(InitialPlayerData playeData)
    {
        initialPlayerData.Value = playeData;       
    }

    [ClientRpc] public void ToggleUnstandbaleZone_ClientRpc(bool a)
    {
        ToggleUnstanableZone(a);
    }
    
}
