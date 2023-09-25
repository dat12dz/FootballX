using Assets.Script.Player;
using Assets.Script.Utlis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;

namespace Assets.Script.Networking.NetworkRoom
{
    public class InRoomPlayerDictionary : TrackingDictonary<byte,PlayerRoomManager>
    {
        public Action<ulong> onOwnerChange;
        ulong owner_;
        public ulong Owner { get { return owner_; }
            set {
                // Xóa trưởng phòng cũ
                PlayerRoomManager oldRoomHeader = PlayerRoomManager.GetRoomManger(owner_);
                if (oldRoomHeader != null)
                    oldRoomHeader.isHeader.Value = false;
                // Set lại value
                if (onChange != null)
                onOwnerChange(value); 
                owner_ = value;
                // Đặt trưởng phòng mới
                PlayerRoomManager RoomManager = PlayerRoomManager.GetRoomManger(value);
                RoomManager.isHeader.Value = true;
            } 
        }
        public InRoomPlayerDictionary() : base()
        {
            NetworkManager.Singleton.OnServerStopped += (bool_) => Clear();
        }
        public byte? Add(PlayerRoomManager a)
        {
            var slot = FindEmpTyIndex();
            if (slot != null)
            {
                base.Add((byte)slot,a);
            }
            return slot;
        }
        public byte? FindEmpTyIndex()
        {
            for (byte i = 0; i < 10; i++)
            {
                if (ContainsKey(i))
                {
                    continue;
                }
                return i;
            }
            return null;
        }
    }
}
