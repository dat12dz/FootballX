using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

static class NetworkClient_
{
    static bool FirstTimeInit;
    public static void StartClient(string ip, string name)
    {
        UINew_ChangeSceneEffect.Open();
        var ip_ = "";
        NetworkManager netmang = NetworkManager.Singleton;
        ClientApproveData c = new ClientApproveData() { name = name };
        netmang.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(c));
        if (ip == "localhost")
        {
            ip_ = "127.0.0.1";
        }
        else
        {
            ip_ = ip;
        }
        netmang.GetComponent<UnityTransport>().ConnectionData.Address = ip_;
        if (!FirstTimeInit)
        {
            //netmang.OnClientStarted += Netmang_OnClientStarted;
            FirstTimeInit = false;
        }
      
  
        if (netmang.StartClient())
        {
            SceneManager.LoadScene(1);
        }
      

    }

}
public class ClientApproveData
{
    public string name;

}

