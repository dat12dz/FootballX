using Assets.Script.UI;
using Assets.Utlis;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
public struct StartSceneInfo
{
    public string name;
}
public class UI_StartScreenHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField inp_PlayerName, inp_ServerIP;
    [SerializeField] Button btn_Connect,btn_autoLocalhost;
   public static StartSceneInfo StartSceneInfoSync;
    NetworkManager netmang;
    void Start()
    {
            netmang = NetworkManager.Singleton;
            Logging.CheckNLogObjectNull(inp_PlayerName,nameof(inp_PlayerName));
            Logging.CheckNLogObjectNull(inp_ServerIP, nameof(inp_ServerIP));
            Logging.CheckNLogObjectNull(btn_Connect, nameof(btn_Connect));
            Logging.CheckNLogObjectNull(btn_autoLocalhost, nameof(btn_autoLocalhost));
            btn_Connect.onClick.AddListener(() => {
            if (inp_PlayerName.text == string.Empty)
            {
                MessageBox.Show("Player name cannot empty", "Please fill in your player name");
                return;
            }
            if (inp_ServerIP.text == string.Empty)
            {

                  
                       if (netmang.StartServer())
              
                  NetworkServer.StartServer();
            } 
                

            else
            {
                    NetworkClient_.StartClient(inp_ServerIP.text,inp_PlayerName.text);
                    StartGameInfo.instance.PlayerName = inp_PlayerName.text;
            }
        });
        btn_autoLocalhost.onClick.AddListener(() =>
        {
            inp_ServerIP.text = "127.0.0.1";
        });
      /*  netins.OnConnecting += () =>
        {
            try
            {
                btn_Connect.GetComponentInChildren<TextMeshProUGUI>().text = "Connecting...";
                btn_Connect.enabled = false;

            }
            catch { }
        };
        netins.OnConnectFail += () =>
        {
            btn_Connect.GetComponentInChildren<TextMeshProUGUI>().text = "Connect";
            btn_Connect.enabled = true;
        };*/
    }

    // Update is called once per frame

}
