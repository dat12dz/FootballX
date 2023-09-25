﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script
{
    public class Grabable : MonoBehaviour
    {


        protected Transform Graber { get; set; }

        public bool isGrab()
        {
            return Graber != null;
        }
        public virtual void Throw(Vector3 force)
        {
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;

            if (rb != null)
            Graber = null;
            rb.AddForce(force,ForceMode.VelocityChange);

        }
        void HolderCheck()
        {
            if (Graber != null)
            {
                transform.position = Graber.position;
                transform.rotation = Graber.rotation;
            }
        }
        protected virtual void Update()
        {
            HolderCheck();
        }
        public virtual void Grab(Transform graber) 
        {
            Graber = graber;
            var rb = GetComponent<Rigidbody>();
            if (rb != null )
            {
                rb.isKinematic = true;
            }
        }
    }
}
