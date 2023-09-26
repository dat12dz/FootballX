using Assets.Script.Utlis.CheckNullProp;
using Assets.Utlis;
using JetBrains.Annotations;
using Unity.Netcode;


public partial class PlayerRoomManager
{
    [ServerRpc]
    public void CreateRoomServerRpc(ServerRpcParams @params = default)
    {
        // Lấy client id của người gửi
        ulong clientid = NetworkkHelper.GetClientIdFrom(@params);
        // tạo phòng
        Room r = new Room(this);
        // lấy người gửi
        var netParam = NetworkkHelper.CreateRpcTo(clientid);
        // trả lại cho người gửi
        var renderable = new RoomRenderAble(r.RoomID);
        OnCreateRoomCompleteClientRpc(renderable,netParam);
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
    }
}

