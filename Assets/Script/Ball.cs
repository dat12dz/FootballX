using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : Grabable
{
    [HideInInspector]
   public Rigidbody rb;
    public static Ball instance;
    void Start()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
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
