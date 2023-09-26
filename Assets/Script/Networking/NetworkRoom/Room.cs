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
    // Danh sách người chơi có mặt trong phòng
    InRoomPlayerDictionary playerDict = new InRoomPlayerDictionary();
    // Room ID của room này
    public uint RoomID;
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

