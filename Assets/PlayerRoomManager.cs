using Assets.Script.Networking.NetworkRoom;
using Assets.Script.Utlis;
using Assets.Script.Utlis.CheckNullProp;
using Assets.Utlis;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRoomManager : NetworkBehaviour
{
    Player thisPlayer;
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
        e
        thisPlayer = GetComponent<Player>();
        NetworkObject.CheckObjectVisibility += CheckForVisibility;
    }
   // ClientRenderer
   
    public NetworkVariable<uint> RoomID;
    public NetworkVariable<byte> SlotInRoom;
    public NetworkVariable<bool> isHeader;
    [ServerRpc]
    public void CreateRoomServerRpc(ServerRpcParams @params = default)
    {
        // Lấy client id của người gửi
      ulong clientid=  NetworkkHelper.GetClientIdFrom(@params);
        // tạo phòng
        Room r = new Room(this);
        Logging.Log("Tạo phòng thành công");
        // lấy người gửi
      var netParam =  NetworkkHelper.CreateRpcTo(clientid);
        // trả lại cho người gửi
        OnCreateRoomCompleteClientRpc(netParam);
    }
    [ClientRpc]
    public void OnCreateRoomCompleteClientRpc(ClientRpcParams @params = default)
    {
        Logging.Log($"Client đã nhận được id phòng ID {RoomID.Value} slot : {SlotInRoom.Value} trường phòng:{isHeader.Value}");
    }
    bool CheckForVisibility(ulong id)
    {
        var PlayerRoom = GetRoomManger(id);
        return (RoomID == PlayerRoom.RoomID && RoomID.Value != 0) || PlayerRoom.IsOwner;

    }
    [ServerRpc]
    public void JoinRoomServerRpc(uint roomID)
    {
        Room roomNeedAdd;
       var res =  Room.RoomDict.TryGetValue(roomID, out roomNeedAdd);
        if (res)
        {
            roomNeedAdd.AddPlayer(this);
            Logging.Log("Join phòng thành công" + roomID);

        }
        else
        {
            Logging.Log("Không thể tìm thấy room id" + roomID);
        }
    }

    
}
public class Room
{
    /// <summary>
    /// Danh sách các phòng có mặt trong game
    /// </summary>
    public static AutoFindNextDictionary<Room> RoomDict = new AutoFindNextDictionary<Room>();
    // Danh sách người chơi có mặt trong phòng
    InRoomPlayerDictionary playerDict = new InRoomPlayerDictionary();
    // Room ID của room này
    uint RoomID;
    Scene physicScene;
    public Room(PlayerRoomManager RoomOwner)
    {   
        // Thêm phòng
        RoomID = RoomDict.Add(this);

        // Thêm người chơiii
        AddPlayer(RoomOwner);
     
        playerDict.Owner = RoomOwner.OwnerClientId;

    }
    public bool AddPlayer(PlayerRoomManager player)
    {
        // lấy slot của người chơi sau khi thêm vào
        var slot = playerDict.Add(player);
        // Nếu có slot
        if (slot != null)
        {
            player.RoomID.Value = RoomID;
            player.SlotInRoom.Value = (byte)slot;
        }

        return slot != null;
    }
    public void RemovePlayer(uint Player)
    {

    }
    public void DeleteRoom()
    {
        RoomDict.Remove(RoomID);
    }
    public void StartGame()
    {

    }
}