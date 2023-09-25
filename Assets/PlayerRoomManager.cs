using Assets.Script.Networking.NetworkRoom;
using Assets.Script.Utlis;
using Assets.Script.Utlis.CheckNullProp;
using Assets.Utlis;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Jobs;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRoomManager : NetworkBehaviour
{
    Player thisPlayer;
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
        NetworkObject.CheckObjectVisibility += CheckForVisibility;
        RoomID.OnValueChanged += OnRoomIDChange;
    }
   // ClientRenderer
   
    public NetworkVariable<uint> RoomID;
    public NetworkVariable<byte> SlotInRoom;
    public NetworkVariable<bool> isHeader;
    bool CheckForVisibility(ulong id)
    {
        var PlayerRoom = GetRoomManger(id);
        return (RoomID == PlayerRoom.RoomID && RoomID.Value != 0) || PlayerRoom.IsOwner;
    }
    protected 
    void OnRoomIDChange(uint old,uint curr)
    {
       if (IsLocalPlayer)
       if (curr == 0)
        {
            UI_RoomRenderPnl.instance.gameObject.active = false;
        }
        else
        {
            UI_RoomRenderPnl.instance.gameObject.active = true;
            UI_RoomRenderPnl.instance.init(curr);
        }
    }
    [ServerRpc]
    public void CreateRoomServerRpc(ServerRpcParams @params = default)
    {
        // Lấy client id của người gửi
      ulong clientid=  NetworkkHelper.GetClientIdFrom(@params);
        // tạo phòng
        Room r = new Room(this);
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

    [ServerRpc]
    public void JoinRoomServerRpc(uint roomID)
    {
        Room roomNeedAdd;
       var res =  Room.RoomDict.TryGetValue(roomID, out roomNeedAdd);
        if (res)
        {
            roomNeedAdd.AddPlayer(this);
            Logging.Log("Join phòng thành công" + roomID);
            JoinRoomCompleteClientRpc(NetworkkHelper.CreateRpcTo(OwnerClientId));

        }
        else
        {
            Logging.Log("Không thể tìm thấy room id" + roomID);
        }
    }
    [ClientRpc]
    public void JoinRoomCompleteClientRpc(ClientRpcParams @params = default)
    {
        Logging.Log("Đã join thành công");
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
        Logging.Log("Tạo phòng thành công ID:" + RoomID);

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