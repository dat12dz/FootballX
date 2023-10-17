using Assets.Script;
using Assets.Script.NetCode;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.Jobs;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : Grabable
{
 
   public Rigidbody rb;
    // Client instance
    public static Ball instance;
    NetworkObject thisNetobj;
    public GameSystem thisSceneGameSystem;
    Transform ballSpawnPoint;
    private void Awake()
    {
      //  base.Awake();
    }
    public void init(GameSystem g,Transform spawnPoint)
    {
        thisSceneGameSystem = g;
        ballSpawnPoint = spawnPoint;
    }
    public override void OnNetworkSpawn()//
    {
        base.OnNetworkSpawn();
        if (IsClient && !IsHost)
        gameObject.SetActive(true);
        Debug.Log(gameObject.name);
    }
    void Start()
    {
        //base.Start();
        if (!IsSpawned) gameObject.SetActive(false);
        if (IsClient)
        instance = this;
        rb = GetComponent<Rigidbody>();
        thisNetobj = NetworkObject;
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
    public void Suppress(bool t)
    {
        rb.isKinematic = t;  
    }
    public void BackToSpawnPos()
    {
        transform.position = ballSpawnPoint.position;
    }
}
