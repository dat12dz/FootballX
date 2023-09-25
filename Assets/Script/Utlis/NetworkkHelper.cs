using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;


namespace Assets.Script.Utlis.CheckNullProp
{
    static class NetworkkHelper
    {
        public static ClientRpcParams CreateRpcTo(ulong[] ClientID)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = ClientID
                }
            };
           return clientRpcParams;
        }
        public static ClientRpcParams CreateRpcTo(ulong ClientID)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { ClientID }
                }
            };
            return clientRpcParams;
        }
        public static ulong GetClientIdFrom(ServerRpcParams r)
        {
            return  r.Receive.SenderClientId;
        }
    }
}
