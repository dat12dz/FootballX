using Assets.Script.UI;
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
    public static UI_SelectRoomUI instance;
    [SerializeField] Button btn_CreateRoom;
    [SerializeField] Button btn_JoinRoom;
    [SerializeField] Button btn_Disconnect;
    [SerializeField] TMP_InputField inp_RoomID;
    [SerializeField] RoomRendererBase RoomRender;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
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
  //      SceneManager.LoadScene(0);
    }
    public void ToggleVisible(bool isEnable)
    {
        gameObject.SetActive(!isEnable);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
