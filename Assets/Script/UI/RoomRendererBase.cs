using Assets.Script.Networking.NetworkRoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Script.UI
{
    public abstract class RoomRendererBase : WaitForInstaceNotNull<RoomRendererBase>
    {
        [SerializeField] public IUI_PlayerCardBase[] ShowPlayerInfoPnl = new IUI_PlayerCardBase[10];
        public abstract void init(RoomRenderAble RoomInfo);
        public abstract void de_init();
        public abstract void OnHeaderChange(bool old, bool curr);
        public abstract void OnReadyChange(bool old, bool curr);
       public abstract void Btn_LeaveRoomFunc();
       public abstract void Btn_ReadyAction();
       public abstract void btn_StartGameAction();
       public abstract void ClearAllRenderer();
    }
}
