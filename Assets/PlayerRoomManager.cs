using Assets.Script.Networking.NetworkRoom;
using Assets.Script.Utlis;
using Assets.Script.Utlis.CheckNullProp;
using Assets.Utlis;
using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Jobs;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PlayerRoomManager : NetworkBehaviour
{
   public Player thisPlayer;
    [SerializeField] Ui_ShowPlayerInfoPnl pnl_PlayerInfoPerf;
    public static PlayerRoomManager GetRoomManger(ulong id)
    {
        NetworkClient cl;
       var res = NetworkManager.Singleton.ConnectedClients.TryGetValue(id, out cl);
        if (res)
        {
            return  cl.PlayerObject.GetComponent<PlayerRoomManager>();
        }
        else
        {
            Logging.Log("Không tìm thấy player object id:" + id);
        }
        return null;
    }    
    private void Start()
    {
        thisPlayer = GetComponent<Player>();
        if (IsClient)
        {
            NetworkObject.CheckObjectVisibility += CheckForVisibility;
            SlotInRoom.OnValueChanged = onSlotChange;
            if (RoomID.Value != 0)
            {

            }    
        }
    }
   // ClientRenderer
   
    public NetworkVariable<uint> RoomID;
    void OnRoomIDChange(uint old, uint curr)
    {
        if (IsLocalPlayer)
            if (curr == 0)
            {
                UI_RoomRenderPnl.instance.gameObject.active = false;
            }
            else
            {
                UI_RoomRenderPnl.instance.gameObject.active = true;
             
            }
    }
    public NetworkVariable<byte> SlotInRoom;
    void onSlotChange(byte old, byte curr)
    {
        UI_RoomRenderPnl.instance.ShowPlayerInfoPnl[curr].init(this);
    }
    public NetworkVariable<bool> isHeader;
    bool CheckForVisibility(ulong id)
    {
        var PlayerRoom = GetRoomManger(id);
        return (RoomID.Value == PlayerRoom.RoomID.Value && RoomID.Value != 0) || OwnerClientId == id;
    }
    protected 


    private void FixedUpdate()
    {
        if (IsServer)
        {
            var ClientArr = NetworkManager.Singleton.ConnectedClientsIds;

            for (int i = 0; i < ClientArr.Count; i++)
            {
                if (CheckForVisibility(ClientArr[i]))
                {
                    try
                    {
                        NetworkObject.NetworkShow(ClientArr[i]);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    try
                    {
                        NetworkObject.NetworkHide(ClientArr[i]);
                    }
                    catch
                    {

                    }
                };
            }
        }
    }

}
