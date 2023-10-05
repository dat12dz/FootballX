using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Script.Networking
{
    internal class CustomNetworkManager : NetworkManager
    {
        private void Awake()
        {
            if (Singleton != null)
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
  
        }
    }
}
