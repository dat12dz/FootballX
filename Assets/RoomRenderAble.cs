using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;


    internal struct RoomRenderAble : INetworkSerializeByMemcpy
    {
        public static RoomRenderAble ClientRoom;
        string RoomName;
        uint RoomId;
    }

