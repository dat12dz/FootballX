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
    public const byte PLAYER_OUTSIDE_ROOM = 255;
    public static PlayerRoomManager localPlayerRoomManager;
    public Player thisPlayer;
    [SerializeField] Ui_ShowPlayerInfoPnl pnl_PlayerInfoPanel;
    public DateTime joinedTime;

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
            RoomID.OnValueChanged += OnRoomIDChange;
            SlotInRoom.OnValueChanged = onSlotChange;
            if (RoomID.Value != 0)
            {
                AddPlayerToRoomRender(SlotInRoom.Value);
            }
            if (IsLocalPlayer) localPlayerRoomManager = this;
        }
        if (IsServer)
        {
        
        }
    }
    // ClientRenderer

    public NetworkVariable<uint> RoomID = new NetworkVariable<uint>();
    void OnRoomIDChange(uint old, uint curr)
    {
        if (IsLocalPlayer)
            if (curr == 0)
            {
                UI_RoomRenderPnl.instance.de_init();
            }
            else
            {
                UI_RoomRenderPnl.instance.gameObject.active = true;
             
            }
    }
    public NetworkVariable<byte> SlotInRoom = new NetworkVariable<byte>(PLAYER_OUTSIDE_ROOM);
    void onSlotChange(byte old, byte curr)
    {
        if (pnl_PlayerInfoPanel)
        pnl_PlayerInfoPanel.de_init();
        AddPlayerToRoomRender(curr);
    }
    public NetworkVariable<bool> isHeader;
    bool CheckForVisibility(ulong id)
    {
        var PlayerRoom = GetRoomManger(id);
        return (RoomID.Value == PlayerRoom.RoomID.Value && RoomID.Value != 0) || OwnerClientId == id;
    }
    void AddPlayerToRoomRender(byte slot)
    {
        if (slot != PLAYER_OUTSIDE_ROOM)
        pnl_PlayerInfoPanel = UI_RoomRenderPnl.instance.ShowPlayerInfoPnl[slot];
        pnl_PlayerInfoPanel.init(this);
    }

    private new void OnDestroy()
    {
        if (IsClient)
        {
            if (pnl_PlayerInfoPanel)
            pnl_PlayerInfoPanel.de_init();
            base.OnDestroy();
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            Room.GetRoom(RoomID.Value).RemovePlayer(SlotInRoom.Value);
        }
        base.OnNetworkDespawn();
    }
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
