using Assets.Utlis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


    public partial class Move
    {
        [SerializeField] Transform GroundCheck;
         bool isGrounded;
        public float GroundCheckRadius;
        [SerializeField] LayerMask GroundMask;
       [SerializeField] float GravityInit = 9.81f;
        float GravityRuntime;
        // Create a sphere chheck physic
       bool CheckisGrounded()
       {
            if (!GroundCheck)
            {
                Logging.LogObjectNull(nameof(GroundCheck));
            }
            var res = Physics.CheckSphere(GroundCheck.position, GroundCheckRadius, GroundMask);
            if (res) GravityRuntime = -2;
           return res;
       }
      
    }
 
