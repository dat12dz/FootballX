using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_SelectRoomUI : MonoBehaviour
{
    [SerializeField] Button btn_CreateRoom;
    [SerializeField] Button btn_JoinRoom;
    [SerializeField] Button btn_Disconnect;
    [SerializeField] TMP_InputField inp_RoomID;
    [SerializeField] UI_RoomRenderPnl RoomRender;

    void Start()
    {
     
        btn_CreateRoom.onClick.AddListener(() =>
        {
            
            PlayerRoomManager localRoomManager = GetLocalRoomnanager();
            localRoomManager.CreateRoomServerRpc();
        });
        btn_JoinRoom.onClick.AddListener(() =>
        {
            PlayerRoomManager localRoomManager = GetLocalRoomnanager();

            var RoomID = Convert.ToUInt32(inp_RoomID.text);
            localRoomManager.JoinRoomServerRpc(RoomID);
        });
        btn_Disconnect.onClick.AddListener(Btn_DisconnectAction);
    }
    PlayerRoomManager GetLocalRoomnanager()
    {
        PlayerRoomManager localRoomManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerRoomManager>();
        return localRoomManager;

    }
    void Btn_DisconnectAction()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
