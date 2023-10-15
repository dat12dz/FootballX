using Assets.Script.UI;
using Assets.Utlis;

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Jobs;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public struct StartSceneInfo
{
    public string name;
}
public class UI_StartScreenHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField inp_PlayerName, inp_ServerIP;
    [SerializeField] Button btn_Connect,btn_autoLocalhost,btn_HostBtn,btn_ShowCharSelectionUI;
    [SerializeField] UI_CharacterSelection charSelectionUI;
   public static StartSceneInfo StartSceneInfoSync;
    NetworkManager netmang;
    int maxHostPlayer = 10;
    void Start()
    {
            netmang = NetworkManager.Singleton;
            maxHostPlayer = 10;
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
                    StartGameInfo.instance.playerData.playerName = inp_PlayerName.text;
            }
        });
        btn_HostBtn.onClick.AddListener(() =>
        {
            StartGameInfo.instance.playerData.playerName = inp_PlayerName.text;
            netmang.ConnectionApprovalCallback = (req, res) =>
            {
                if (netmang.ConnectedClients.Count > maxHostPlayer)
                {
                    res.Approved = false;
                    res.Reason = "Server is full";

                }
                res.Approved = true;
                res.CreatePlayerObject = true;
            };
            if (netmang.StartHost())
                SceneManager.LoadScene(1);

        });
        btn_autoLocalhost.onClick.AddListener(() =>
        {
            inp_ServerIP.text = "127.0.0.1";
        });
        netmang.OnServerStopped += Netmang_OnServerStopped;
        netmang.OnClientStopped += Netmang_OnClientStopped;
        netmang.OnTransportFailure += Netmang_OnTransportFailure;
        netmang.OnClientDisconnectCallback += Netmang_OnClientDisconnectCallback;
        btn_ShowCharSelectionUI.onClick.AddListener(() =>
        {
            charSelectionUI.Display(true);
            gameObject.SetActive(false);
        });
    }

    private void Netmang_OnClientDisconnectCallback(ulong obj)
    {
        if (!netmang.IsServer)
            MessageBox.Show("Disconnect from server",$"{netmang.DisconnectReason}");
    }

    private void Netmang_OnTransportFailure()
    {
        SceneManager.LoadScene(0);
    }

    private void Netmang_OnClientStopped(bool obj)
    {
        SceneManager.LoadScene(0);            
    }
   
    private void Netmang_OnServerStopped(bool obj)
    {
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame

}
