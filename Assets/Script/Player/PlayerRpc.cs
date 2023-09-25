﻿using Assets.Script;
using Assets.Utlis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
public partial class Player
{
    NetworkVariable<bool> isGrabed= new NetworkVariable<bool>();
    [ServerRpc]
    public void GrabItemOrThrowServerRpc(float ThrowForce = 0)
    {
        if (ThrowForce < MinThrowingForce) // pick up item
        {

            RaycastHit r;
            Physics.Raycast(Playereyes.transform.position, Playereyes.transform.forward, out r, LookatDistance, lookatMask);
            if (r.collider != null)
            {
                Grabable GrabAbleItem = r.collider.GetComponent<Grabable>();
                if (GrabAbleItem != null)
                    GrabAbleItem.Grab(Graber);
                grabitem = GrabAbleItem;
                
            }
            else
            {
           
                Logging.Log("Không thể nhặt được item này");
            
            }
        }
      
        if (ThrowForce > MinThrowingForce && MaxThrowingForce > ThrowForce && isGrabed.Value) // throw item
        {
           if (grabitem != null) 
            grabitem.Throw(Playereyes.transform.forward * ThrowForce);
            grabitem = null;
        }
        isGrabed.Value = grabitem != null;

    }
    [ServerRpc]
    public void SendShootForceServerRpc(float shootForce)
    {
        FinalShootForce = shootForce;
    }
}
