using Assets.Script;
using Assets.Script.NetCode;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : Grabable
{
    [HideInInspector]
   public Rigidbody rb;
    public static Ball instance;
    NetworkObject thisNetobj;
    GameSystem thisSceneGameSystem;
    void Start()
    {

        instance = this;
        rb = GetComponent<Rigidbody>();
        thisNetobj = NetworkObject;
        if (NetworkManager.Singleton.IsServer)
        {
            thisNetobj.SpawnWithObservers = false;
            thisNetobj.Spawn();
            thisSceneGameSystem = SceneHelper.GetGameSystem(gameObject.scene);

            foreach (var player in thisSceneGameSystem.room.playerDict.Values)
            {
                thisNetobj.NetworkShow(player.OwnerClientId);
            }
        }
    }
   
    public void Shoot(Vector3 force)
    {
        rb.AddForce(force,ForceMode.VelocityChange);
    }
    public void Shoot(float x,float y,float z)
    {
        rb.AddForce(new Vector3(x,y,z), ForceMode.VelocityChange);
    }
    
    protected override void Update()
    {
        base.Update();
    }

    public override void Grab(Transform graber_)
    {
        base.Grab(graber_);
    }
}
