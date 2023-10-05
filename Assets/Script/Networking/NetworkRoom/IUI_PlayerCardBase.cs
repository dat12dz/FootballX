using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Networking.NetworkRoom
{
   public interface IUI_PlayerCardBase
    {
        PlayerRoomManager roomManager { get; set; }
        byte slot { get; set; }
        public void init(PlayerRoomManager roomManager_);
        public void onHeaderChange(bool old, bool curr);
        public void OnReadyChange(bool old, bool curr);
        public void de_init(PlayerRoomManager verify);
        public void btn_ChangeSlotAction();
        public void btn_KickPlayerAction();
        public void Btn_SetOwnerAction();
        public void Btn_SwapRequestAction();
        public void InitCounter();
        public void CounterCallback(object s);
        public void EndCounter();
        public void ToggleStopReqest(bool a);
        public void HidePlayerSwapRequest();
        public void ShowPlayerSwapRequest();
        public void btn_StopRequestAction();
        public void btn_refuseAction();
        public void btn_acceptAction();
        public void SetPlayerName(string Name);
    }
}
