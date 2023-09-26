using Assets.Script.Networking.NetworkRoom;
using Assets.Script.Utlis;
using Assets.Utlis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Room
{

    /// <summary>
    /// Danh sách các phòng có mặt trong game
    /// </summary>
    public static AutoFindNextDictionary<Room> RoomDict = new AutoFindNextDictionary<Room>();
    public static Room GetRoom(uint RoomID)
    {
      return RoomDict[RoomID];
    }
    // Danh sách người chơi có mặt trong phòng
    InRoomPlayerDictionary playerDict = new InRoomPlayerDictionary();
    // Room ID của room này
    public uint RoomID;
    Scene physicScene;
    public Queue<PlayerRoomManager> OwnerQueue = new Queue<PlayerRoomManager>();
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
    public void ChangePlayerSlot(byte oldslot,byte newslot)
    {
        // Nếu đã có người chơi ngồi chỗ
        if (playerDict.ContainsKey(newslot)) 
        {
            Logging.LogError("Đã có người ngồi chỗ này");
            return;
        }
        var temp = playerDict.RemoveOut(oldslot);
        playerDict.Add(newslot, temp);
        temp.SlotInRoom.Value = newslot;
    }
    public PlayerRoomManager RemovePlayer(byte slot)
    {

        var PlayerNeedRemove = playerDict.RemoveOut(slot);
        PlayerNeedRemove.RoomID.Value = 0;
        PlayerNeedRemove.SlotInRoom.Value = 0;
        PlayerNeedRemove.isHeader.Value = false;
        if (playerDict.Count == 0)
        {
            DeleteRoom();
        }
        return PlayerNeedRemove;
    }
    public void DeleteRoom()
    {
        RoomDict.Remove(RoomID);

    }
    public void SetNewOwner(byte slot)
    {
        if (playerDict.ContainsKey(slot))
        {
            var newOwner = playerDict[slot];
            if (newOwner != null) 
            {
                playerDict.Owner = newOwner.OwnerClientId;
            }
        }
    }
    public void StartGame()
    {

    }
}

