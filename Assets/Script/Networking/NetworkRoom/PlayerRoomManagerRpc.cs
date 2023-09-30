﻿using Assets.Script.Utlis.CheckNullProp;
using Assets.Utlis;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
public partial class PlayerRoomManager
{
    [ServerRpc]
    public void CreateRoomServerRpc(ServerRpcParams @params = default)
    {
        if (RoomID.Value == 0)
        {
            // Lấy client id của người gửi
            ulong clientid = NetworkkHelper.GetClientIdFrom(@params);
            // tạo phòng
            Room r = new Room(this);
            // lấy người gửi
            var netParam = NetworkkHelper.CreateRpcTo(clientid);
            // trả lại cho người gửi
            var renderable = new RoomRenderAble(r.RoomID);
            OnCreateRoomCompleteClientRpc(renderable, netParam);
        }
        else
        {
            Logging.Log(thisPlayer.PlayerName + ":Đang ở trong phòng không thể tạo thêm phòng");
        }
    }
    [ClientRpc]
    public void OnCreateRoomCompleteClientRpc(RoomRenderAble renderable, ClientRpcParams @params = default)
    {
        if (IsLocalPlayer)
        {
            UI_RoomRenderPnl.instance.init(renderable);
            onSlotChange(0, 0);

        }
        Logging.Log($"Client đã nhận được id phòng ID {RoomID.Value} slot : {SlotInRoom.Value} trường phòng:{isHeader.Value}");
    }

    [ServerRpc]
    public void JoinRoomServerRpc(uint roomID)
    {
        Room roomNeedAdd;
        var res = Room.RoomDict.TryGetValue(roomID, out roomNeedAdd);
        if (res)
        {
            roomNeedAdd.AddPlayer(this);
            Logging.Log("Join phòng thành công" + roomID);
            JoinRoomCompleteClientRpc(new RoomRenderAble(roomNeedAdd.RoomID), NetworkkHelper.CreateRpcTo(OwnerClientId));
        }
        else
        {
            Logging.Log("Không thể tìm thấy room id" + roomID);
        }
    }
    [ClientRpc]
    public void JoinRoomCompleteClientRpc(RoomRenderAble renderable,ClientRpcParams @params = default)
    {
        UI_RoomRenderPnl.instance.init(renderable);
        Logging.Log("Đã join thành công");
        onSlotChange(0, 0);
    }
    [ServerRpc]
    public void LeaveRoomServerRpc()
    {


        // Khi người chơi ngắt kết nối hoặc bấm dấu X rời phòng
        var slotInRoom = SlotInRoom.Value;
        var RoomPlayerIn = Room.GetRoom(RoomID.Value);
        RoomPlayerIn.RemovePlayer(slotInRoom);
        ClientRpcParams c = NetworkkHelper.CreateRpcTo(OwnerClientId);
        LeaveRoomCompleteClientRpc(c);

    }
    [ClientRpc]
    public void LeaveRoomCompleteClientRpc(ClientRpcParams pa)
    {
        Logging.Log("Xóa người chơi thành công");
    }
    [ServerRpc] 
    public void ChangePlayerSlotServerRpc(byte newSlot)
    {
        Room.GetRoom(RoomID.Value).ChangePlayerSlot(SlotInRoom.Value, newSlot);
    }
    [ServerRpc] public void KickPlayerServerRpc(byte slot)
    {
       if (isHeader.Value)
        {
            var RoomPlayerIn = Room.GetRoom(RoomID.Value);
            if (SlotInRoom.Value == slot)
            {
                Logging.LogError("Không thể tự kickk bản thân");
                return;
            }
           var KKickkedPlayer = RoomPlayerIn.RemovePlayer(slot);

            var pa = NetworkkHelper.CreateRpcTo(KKickkedPlayer.OwnerClientId);
            OnKickPlayerCompleteClientRpc(pa);
        }
       else
        {
            Logging.LogError("Không phải trưởng phòng ko thể kick thành viên");
        }
        
    }

    [ClientRpc] public void OnKickPlayerCompleteClientRpc(ClientRpcParams pa)
    {
        Logging.Log("bạn đã bị buộc rời khỏi phòng");
        MessageBox.Show("Rời khỏi phòng", "Bạn đã bị buộc rời khỏi phòng");
    }
    [ServerRpc] public void SetOwnerServerRpc(byte slot)
    {
        if (isHeader.Value)
        {
            var RoomPlayerIn = Room.GetRoom(RoomID.Value);
            RoomPlayerIn.SetNewOwner(slot);
        }
        else
        {
            Logging.LogError("Không phải trưởng phòng ko thể thực hiện lệnh nhường trưởng phòng");
        }
    }
    [ServerRpc] public void StartGameServerRpc()
    {
        // Nếu là trưởng phòng thì mới có quyền chạy tiếp
        if (isHeader.Value)
        {
            var RoomPlayerIn = Room.GetRoom(RoomID.Value);
            RoomPlayerIn.StartGame();
        }
    }
    [ServerRpc] public void ToggleReadyServerRpc()
    {
        if (!isHeader.Value)
        isReady.Value = !isReady.Value;
    }
    CancellationTokenSource[] SwapSlotRequests = new CancellationTokenSource[10];
    [ServerRpc] public void SendSwapRequestServerRpc(byte slot)
    {
        var RoomPlayerIn = Room.GetRoom(RoomID.Value);
        try
        {
            var SwapClient = RoomPlayerIn.playerDict[slot];
            var SwapClientID = SwapClient.OwnerClientId;
            var ClientnetRpc =  NetworkkHelper.CreateRpcTo(SwapClientID);
           /* SwapClient.SwapSlotRequests[]
            Task.Delay(SwapRequestTimeout).ContinueWith((t) => { 
                if (t.IsCanceled)
                {
                    // đổi chỗ thành công;
                }
            });
            SendSwapRequestToClientRpc(fromWho: SlotInRoom.Value,to: ClientnetRpc);*/
        }
        catch (KeyNotFoundException)
        {
            Logging.Log("Không tìm thấy người chơi");
        }
    }
    [ClientRpc] void SendSwapRequestToClientRpc(byte fromWho,uint cancelID,ClientRpcParams to)
    {
        
    }
    [ServerRpc] public void SendAcceptSwapRequestServerRpc()
    {
        
    }
}

