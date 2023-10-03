using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;


    public struct RoomRenderAble : INetworkSerializeByMemcpy
    {
        public RoomRenderAble(uint RoomID,string name)
        {
            RoomId = RoomID;
            RoomName = name;
        }
       public string RoomName;
       public uint RoomId;
    }

