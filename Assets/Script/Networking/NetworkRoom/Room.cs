using Assets.Script.Networking.NetworkRoom;
using Assets.Script.Utlis;
using Assets.Utlis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room
{

    /// <summary>
    /// Danh sách các phòng có mặt trong game
    /// </summary>
    public static AutoFindNextDictionary<Room> RoomDict = new AutoFindNextDictionary<Room>();
  
    public static Room GetRoom(uint RoomID)
    {
        try
        {
            return RoomDict[RoomID];
        }
        catch
        {
            Logging.LogError("Không thể tìm thấy phòng ID" + RoomID);
            return null; 
        }
    }
    // Danh sách người chơi có mặt trong phòng
    public InRoomPlayerDictionary playerDict = new InRoomPlayerDictionary();
    // Room ID của room này
    public uint RoomID;
    Scene physicScene;
    public SortedList<long, PlayerRoomManager> JoinedTimeList = new SortedList<long, PlayerRoomManager>();
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
            // Set thời gian người chơi tham gia
            player.joinedTime = DateTime.Now;
            JoinedTimeList.Add(DateTime.Now.Ticks, player);
            // Set thông tin người chơi
            player.RoomID.Value = RoomID;
            player.GetComponent<PlayerNetworkTransform>().TeleportImidiateClientRpc(player.transform.position);
            player.SlotInRoom.Value = (byte)slot;
            player.ShowToAllClientInTheRoom(this);
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
        // Đặt người chơi vào vị trí mới
        var temp = playerDict.RemoveOut(oldslot);
        playerDict.Add(newslot, temp);
        temp.SlotInRoom.Value = newslot;
    }
    public PlayerRoomManager RemovePlayer(byte slot)
    {
        // Xóa người chơi khỏi danh sách và lấy người chơi bị xóa
        var PlayerNeedRemove = playerDict.RemoveOut(slot);
        // Xóa người chơi khỏi danh sách chờ làm trưởng phòng
        JoinedTimeList.Remove(PlayerNeedRemove.joinedTime.Ticks);
        // Nếu người chơi bị xóa là trưởng phòng nhường trưởng phòng cho người chơi khác
        if (PlayerNeedRemove.isHeader.Value)
        {
            if (JoinedTimeList.Count > 0)
            {
                var newOwner = JoinedTimeList.First().Value;
                if (newOwner != null)
                    SetNewOwner(newOwner.SlotInRoom.Value);
            }
        }
        // Set tất cả về giá trị mặc định
        PlayerNeedRemove.RoomID.Value = 0;
        PlayerNeedRemove.SlotInRoom.Value = PlayerRoomManager.PLAYER_OUTSIDE_ROOM;
        PlayerNeedRemove.isHeader.Value = false;
        PlayerNeedRemove.joinedTime = default;
        PlayerNeedRemove.isReady.Value = false;
        PlayerNeedRemove.HideToAllClientInTheRoom(this);
        // Nếu trong phòng kkhoong còn ai -> Xóa phòng
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
        var LoadGame = SceneManager.LoadScene(2, new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D));
        SceneManager.sceneLoaded += (scene, loadmoded) =>
        {
            if (scene == LoadGame)
            {
                var RootGameObj =  LoadGame.GetRootGameObjects();
                try
                {
                    GameSystem loadedGameSystem = RootGameObj[0].GetComponent<GameSystem>();
                    loadedGameSystem.Init(this);
                }
                catch (Exception e) 
                {
                    Logging.LogError("Không thể tìm thấy Game system của scene,hãy chắc chắn là game system đang ở vị trí đầu tiên của GameScene");
                }
            }
        };

    }
    public void SwapPlayerSlot(byte a,byte b)
    {
        PlayerRoomManager PlayerA = null,PlayerB =null;
        playerDict.TryGetValue(a, out PlayerA);
        playerDict.TryGetValue(b, out PlayerB);
        if (PlayerA != null && PlayerB != null)
        {
            playerDict[a] = PlayerB;
            playerDict[b] = PlayerA;
            PlayerA.SlotInRoom.Value = b;
            PlayerB.SlotInRoom.Value = a;
        }
    }

}

