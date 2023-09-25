using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Script.Player
{
     class ControlKey
    {
        public static KeyCode Shoot = KeyCode.Mouse0;
        public static KeyCode GoForward = KeyCode.W;
        public static KeyCode GoBackward = KeyCode.S;
        public static KeyCode GoLeft = KeyCode.A;
        public static KeyCode GoRight = KeyCode.D;
        public static KeyCode Jump = KeyCode.Space;
        public static KeyCode GrabNThrow = KeyCode.Mouse1;
       
    }
}
