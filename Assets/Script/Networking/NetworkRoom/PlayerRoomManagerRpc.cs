using Assets.Script.Networking.NetworkRoom;
using Assets.Script.UI;
using Assets.Script.Utlis.CheckNullProp;
using Assets.Utlis;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
public partial class PlayerRoomManager
{
    [ServerRpc]
    public void CreateRoomServerRpc(ServerRpcParams @params = default)
    {
        // Lấy client id của người gửi
        ulong clientid = NetworkkHelper.GetClientIdFrom(@params);
        // Nếu game là game host chỉ được tạo duy nhất 1 phòng host
        if (NetworkManager.Singleton.IsHost && clientid != 0) return;
        if (RoomID.Value == 0)
        {

            // tạo phòng
            Room r = new Room(this, thisPlayer.PlayerName.Value + " Room's");
            // lấy người gửi
            var netParam = NetworkkHelper.CreateRpcTo(clientid);
 
            if (IsHost)
            {
                Room.hostRoom = r;
            }
            // trả lại cho người gửi
            var renderable = new RoomRenderAble(r.RoomID, r.RoomName, IsHost);
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
            RoomRendererBase.WaitForInstace(() => RoomRendererBase.instance.init(renderable));
            onSlotChange(0, 0);

        }
        Logging.Log($"Client đã nhận được id phòng ID {RoomID.Value} slot : {SlotInRoom.Value} trường phòng:{isHeader.Value}");
    }

    [ServerRpc]
    public void JoinRoomServerRpc(uint roomID)
    {

        Room roomNeedAdd;
        var res = Room.RoomDict.TryGetValue(roomID, out roomNeedAdd);
        if (roomID == 0)
        {
            roomNeedAdd = Room.hostRoom;
            res = roomNeedAdd != null;
        }
        if (res)
        {
            roomNeedAdd.AddPlayer(this);
            Logging.Log("Join phòng thành công" + roomID);
            JoinRoomCompleteClientRpc(new RoomRenderAble(roomNeedAdd.RoomID,roomNeedAdd.RoomName), NetworkkHelper.CreateRpcTo(OwnerClientId));
        }
        else
        {
            Logging.Log("Không thể tìm thấy room id" + roomID);
        }
    }
    [ClientRpc]
    public void JoinRoomCompleteClientRpc(RoomRenderAble renderable,ClientRpcParams @params = default)
    {
        RoomRendererBase.instance.init(renderable);
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

    /// <summary>
    /// Chuyển tới chỗ trống
    /// </summary>
    /// <param name="newSlot">Chỗ trống</param>
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
            if (!thisPlayer.isInGame.Value)
            {
                var RoomPlayerIn = Room.GetRoom(RoomID.Value);
                RoomPlayerIn.StartGame();
            }    
        }
    }
    [ServerRpc] public void ToggleReadyServerRpc()
    {
        if (!isHeader.Value)
        isReady.Value = !isReady.Value;
    }
   public static int SwapTimeout = 120;
   public SlotSwapRequest[] requestList = new SlotSwapRequest[10];
    [ServerRpc] public void StopRequestServerRpc(byte slot)
    {
        var RoomPlayerIn = Room.GetRoom(RoomID.Value);
        if (RoomPlayerIn != null)
        {
            var SwapClient = RoomPlayerIn.playerDict[slot];
            SwapClient.requestList[SlotInRoom.Value].accept = false;
            SwapClient.requestList[SlotInRoom.Value].CancelToken.Cancel();
        }
    }
    /// <summary>
    /// Gửi yêu cầu đổi chỗ tới người chơi đang chiếm vị trí
    /// </summary>
    /// <param name="slot">Chỗ bị chiếm người chơi bởi người chơi khác</param>
    [ServerRpc] public void SendSwapRequestServerRpc(byte slot)
    {
        try
        {
            var RoomPlayerIn = Room.GetRoom(RoomID.Value);
            if (RoomPlayerIn == null)
            {
                return;
            }             
            var SwapClient = RoomPlayerIn.playerDict[slot];
            var SwapClientID = SwapClient.OwnerClientId;
            var ClientnetRpc =  NetworkkHelper.CreateRpcTo(SwapClientID);
            var thisClientnetRpc = NetworkkHelper.CreateRpcTo(OwnerClientId);
            int slotNeedChange = SlotInRoom.Value;
            int newSlot = slot;
            // tao request
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            SwapClient.requestList[slotNeedChange] = new SlotSwapRequest(cancellationToken);
            SlotSwapRequest request = SwapClient.requestList[slotNeedChange];
            // Tạo hàng đợi
            Task.Delay(SwapTimeout * 1000, request.CancelToken.Token).ContinueWith((t) =>
            {
                MainThreadDispatcher.ExecuteInMainThread(() =>
                {
                    SwapSlotEnd_ClientNeedSwapClientRpc(slot, thisClientnetRpc);
                    SwapClient.SwapSlotEnd_ClientBeingRequestClientRpc((byte)slotNeedChange, ClientnetRpc);
                });
                if (t.IsCanceled)
                {
                    // Người chơi tương tác;
                    if (request.accept)
                    {
                        // Đổi vị trí người chơi
                        RoomPlayerIn.SwapPlayerSlot(SlotInRoom.Value, slot);
                    }
                }
                else
                {
                    Logging.LogError("Request swap room time out timeout");
                }
                // Giải phóng cũ
                SwapClient.requestList[slotNeedChange] = null;

            });
            // Xin phép chỗ người chơi
            SwapClient.SwapSlot_ClientBeingRequestClientRpc(fromWho: SlotInRoom.Value, to: ClientnetRpc);
            SwapSlot_ClientNeedSwapClientRpc(slot,thisClientnetRpc);
            // Gửi call back lại client sau khi hoàn thành gửi request 
        }
        catch (KeyNotFoundException)
        {
            Logging.Log("Không tìm thấy người chơi");
        }
    }
    [ClientRpc] public void SwapSlot_ClientNeedSwapClientRpc(byte newslot,ClientRpcParams to)
    {
        RoomRendererBase.instance.ShowPlayerInfoPnl[newslot].ToggleStopReqest(true);
    }
    [ClientRpc] public void SwapSlot_ClientBeingRequestClientRpc(byte fromWho,ClientRpcParams to)
    {
        RoomRendererBase.instance.ShowPlayerInfoPnl[fromWho].ShowPlayerSwapRequest();
    }
    [ClientRpc] public void SwapSlotEnd_ClientNeedSwapClientRpc(byte newslot,ClientRpcParams to)
    {
        RoomRendererBase.instance.ShowPlayerInfoPnl[newslot].ToggleStopReqest(false);
    }
    [ClientRpc]
    public void SwapSlotEnd_ClientBeingRequestClientRpc(byte from,ClientRpcParams to)
    {
        RoomRendererBase.instance.ShowPlayerInfoPnl[from].HidePlayerSwapRequest();
    }
    [ServerRpc] public void SendAcceptSwapRequestServerRpc(byte acceptWho,bool accept_)
    {
        var request = requestList[acceptWho];
        if (request == null)
        {
            Logging.LogError("Request swap slot không hợp lệ");
            return;
        }
        request.accept = accept_;
        request.CancelToken.Cancel();
    }
    
}

