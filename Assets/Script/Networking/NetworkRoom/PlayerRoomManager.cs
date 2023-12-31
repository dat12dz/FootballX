using Assets.Script.Networking.NetworkRoom;
using Assets.Script.UI;
using Assets.Script.Utlis;
using Assets.Script.Utlis.CheckNullProp;
using Assets.Utlis;
using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Unity.Jobs;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
[DefaultExecutionOrder(2)]
[DebuggerDisplay("{name}")]
public partial class PlayerRoomManager : NetworkBehaviour
{
    public const byte PLAYER_OUTSIDE_ROOM = 255;
    public static PlayerRoomManager localPlayerRoomManager;
    public Player thisPlayer;
    [SerializeField] IUI_PlayerCardBase pnl_PlayerInfoPanel;
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
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        SlotInRoom.Value = PLAYER_OUTSIDE_ROOM;
    }
    private void Start()
    {
        thisPlayer = GetComponent<Player>();
        if (IsClient || IsHost)
        {
            DontDestroyOnLoad(gameObject);
            NetworkObject.CheckObjectVisibility += CheckForVisibility;
            RoomID.OnValueChanged += OnRoomIDChange;
            SlotInRoom.OnValueChanged = onSlotChange;
            if (RoomID.Value != 0)
            {
                AddPlayerToRoomRender(SlotInRoom.Value);
            }
            if (IsLocalPlayer)
            {
                
                localPlayerRoomManager = this;
                    UnityAction<Scene, LoadSceneMode> action = null;
                    action = (scene, loadmode) =>
                    {
                        RoomRendererBase.WaitForInstace(() =>  isHeader.OnValueChanged += RoomRendererBase.instance.OnHeaderChange);
                        RoomRendererBase.WaitForInstace(() =>   isReady.OnValueChanged += RoomRendererBase.instance.OnReadyChange);
                        SceneManager.sceneLoaded -= action;
                    };                
                    SceneManager.sceneLoaded += action;
            }
            
        }
        if (IsHost && IsServer)
        {
            UnityAction<Scene, LoadSceneMode> action = null;
            action = (scene, loadmode) =>
            {
                CreateRoomServerRpc();
                SceneManager.sceneLoaded -= action;
            };
            SceneManager.sceneLoaded += action;
        }
        if (IsServer && !IsHost)
        {
            NetworkObject.NetworkShow(OwnerClientId);
        }
        if (IsClient && !IsHost)
        {
            JoinRoomServerRpc(0);
        }
    }
    // ClientRenderer

    public NetworkVariable<uint> RoomID = new NetworkVariable<uint>();
    void OnRoomIDChange(uint old, uint curr)
    {
        if (IsLocalPlayer)
            if (curr == 0)
            {
                RoomRendererBase.instance.de_init();
            }
            else
            {
                RoomRendererBase.WaitForInstace(() => RoomRendererBase.instance.gameObject.active = true);
            }
    }
    public NetworkVariable<byte> SlotInRoom = new NetworkVariable<byte>() { Value = PLAYER_OUTSIDE_ROOM };

    void onSlotChange(byte old, byte curr)
    {
        if (pnl_PlayerInfoPanel != null)
        pnl_PlayerInfoPanel.de_init(this);
        AddPlayerToRoomRender(curr);
            // GoalKeeper
        thisPlayer.isGoalKeeper = curr == 4 || curr == 9;
        
    }
    public NetworkVariable<bool> isHeader;
    bool CheckForVisibility(ulong id)
    {
        var PlayerRoom = GetRoomManger(id);
        return (RoomID.Value == PlayerRoom.RoomID.Value && RoomID.Value != 0) || OwnerClientId == id;
    }
    public NetworkVariable<bool> isReady;

    void AddPlayerToRoomRender(byte slot)
    {
        VariableHelper.TrackForVariableNotNull(() => thisPlayer.thisPlayerModel, () =>
        {
            ChangeColorModel(slot);
            if (slot != PLAYER_OUTSIDE_ROOM)
                RoomRendererBase.WaitForInstace((RoomRendererBase instace) =>
                {   
                    pnl_PlayerInfoPanel = instace.ShowPlayerInfoPnl[slot];
                    pnl_PlayerInfoPanel.init(this);
                });
            thisPlayer.thisPlayerModel.gameObject.SetActive(slot != PLAYER_OUTSIDE_ROOM);
            if (IsServer)
            {
                if (thisPlayer.team == null)
                    thisPlayer.team = new TeamClass(Room.GetRoom(RoomID.Value), SlotInRoom.Value);
                else
                {
                    thisPlayer.team.GameRoom = Room.GetRoom(RoomID.Value);
                    thisPlayer.team.team = TeamClass.GetTeamFromSlot(SlotInRoom.Value);
                }
            }
        });

    }
    
    public void ChangeColorModel(byte slot)
    {
        if (slot < 5 && slot >= 0)
            thisPlayer.thisPlayerModel.RedTeamInit();

        if (slot < 10 && slot >= 5)
            thisPlayer.thisPlayerModel.BlueTeamInit();
    }   
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (IsClient)
        {
            if (pnl_PlayerInfoPanel != null)
                pnl_PlayerInfoPanel.de_init(this);
        }
    }
    bool isDisconnected;
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            try
            {
                isDisconnected = true;
                Room.GetRoom(RoomID.Value).RemovePlayer(SlotInRoom.Value);
            }
            catch
            {

            }
        }
        base.OnNetworkDespawn();
    }
    public void ShowToAllClientInTheRoom(Room r)
    {
        if (IsServer)
        {
            var ListPlayer = r.playerDict;
            foreach (var player in ListPlayer.Values)
            {
                try
                {
                    if (player == this) continue;
                    NetworkObject.NetworkShow(player.OwnerClientId);
                    player.NetworkObject.NetworkShow(OwnerClientId);
                }
                catch
                {

                }
            }   
        }
    }
    public void HideToAllClientInTheRoom(Room r)
    {
        if (IsServer)
        {
            var ListPlayer = r.playerDict;
            foreach (var player in ListPlayer.Values)
            {
                if (player == this || player.IsHost) continue;
                
                NetworkObject.NetworkHide(player.OwnerClientId);
            }
        }
    }
    private void FixedUpdate()
    {
            
    }

}
