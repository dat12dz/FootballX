using Assets.Script.Player;
using Assets.Script.Utlis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Assets.Script.UI
{
    static class NetworkServer
    {

        public static void StartServer()
        {
            NetworkManager netmang = NetworkManager.Singleton;
            //netmang.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
            if (netmang.ConnectionApprovalCallback == null )
            netmang.ConnectionApprovalCallback += ApprovalCheck;
            SceneManager.LoadScene(1);
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
        }
 

        static void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            // The client identifier to be authenticated
            var clientId = request.ClientNetworkId;
            response.CreatePlayerObject = true;
            
            // Additional connection data defined by user code
            var connectionData = request.Payload;
            var bufferToStrinng = Encoding.UTF8.GetString(connectionData);
            ClientApproveData ap = JsonConvert.DeserializeObject<ClientApproveData>(bufferToStrinng);
            // Your approval logic determines the following values
            response.Approved = true;
            // If additional approval steps are needed, set this to true until the additional steps are complete
            // once it transitions from true to false the connection approval response will be processed.
            response.Pending = false;
        }
    }
}
