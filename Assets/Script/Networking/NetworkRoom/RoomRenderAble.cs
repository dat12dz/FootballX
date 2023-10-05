using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;


    public struct RoomRenderAble : INetworkSerializeByMemcpy
    {
        public RoomRenderAble(uint RoomID,string name,bool hostGame = false)
        {
            RoomId = RoomID;
            RoomName = name;
             isHostGame = hostGame;
        }
       public FixedString32Bytes RoomName;
       public uint RoomId;
        public bool isHostGame;
    }

