using Assets.Utlis;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(0)]
public class Ui_ShowPlayerInfoPnl : MonoBehaviour
{
    // Thiết lập các giá trị
    [SerializeField] public TextMeshProUGUI lb_PlayerName;
    [SerializeField] Button KickBtn;
    public PlayerRoomManager roomManager;
    public Image img_isHeader;

    [SerializeField] Image img_isReady;
    public byte slot;
    [SerializeField] Button btn_ChangeSlot,btn_KickPlayer,btn_SetOwner,btn_SwapRequest,btn_AcceptSwapRequest,btn_RefuseSwapRequest,btn_StopRequest;

   // Show các thuộc tính của client ra màn hình
    public void init(PlayerRoomManager roomManager_)
    {
            roomManager = roomManager_;
            // Hiển thị tên người chơi
            lb_PlayerName.SetText( roomManager_.thisPlayer.PlayerName.Value.ToString());
            //  Hiển thị người chơi có phải trưởng phòng hay không
            onHeaderChange(false, roomManager_.isHeader.Value);
             roomManager_.isHeader.OnValueChanged += onHeaderChange;
           // Check xem người chơi đã sẵn sàng chưa
            OnReadyChange(false,roomManager_.isReady.Value);
            roomManager_.isReady.OnValueChanged += OnReadyChange;
    }
    void onHeaderChange(bool old,bool curr)
    {
        // hiển thị người chơi có là chủ phòng lên ui hay không
        img_isHeader.gameObject.SetActive(curr);
    }
    // Xóa hiện thị tên của người chơi
    void OnReadyChange(bool old, bool curr)
    {
        // hiển thị người chơi có sẵn sàng lên ui hay không
        img_isReady.gameObject.SetActive(curr);
    }
    public void de_init(PlayerRoomManager verify)
    {
        // nếu người chơi không còn quyền với phòng -> không thực hiện lệnh hủy
        if (roomManager != verify) return; 
        // Nếu có hiện tại đang có người chơi đang hiển thị
        if (roomManager != null)
        {
            // Xóa tên người chơi
            lb_PlayerName.text = "";
            // Xóa hiển thị có phải trưởng phòng hay không
            roomManager.isHeader.OnValueChanged -= onHeaderChange;
            // Xóa hiện thị đã sẵn sàng 

            roomManager.isReady.OnValueChanged -= OnReadyChange;
            // Giải phóng con trỏ vào người chơi
            roomManager = null;
        }
        // Ẩn icon header
        onHeaderChange(false, false);
        OnReadyChange(false, false);
    }
    void btn_ChangeSlotAction()
    {
        if (PlayerRoomManager.localPlayerRoomManager)
        {
            PlayerRoomManager.localPlayerRoomManager.ChangePlayerSlotServerRpc(slot);
        }
    }
    void btn_KickPlayerAction()
    {
        if (roomManager)
        {
            byte KickSlot = roomManager.SlotInRoom.Value;
            PlayerRoomManager.localPlayerRoomManager.KickPlayerServerRpc(KickSlot);
        }
    }
    void Btn_SetOwnerAction()
    {
        if (roomManager)
        PlayerRoomManager.localPlayerRoomManager.SetOwnerServerRpc(slot);
    }
    void Btn_SwapRequestAction()
    {
        PlayerRoomManager.localPlayerRoomManager.SendSwapRequestServerRpc(slot);
    }
    int LastSecondToRequest = 30;
    Timer counter;
    void InitCounter()
    {
        counter = new Timer(CounterCallback, null, 0, 1000);
        LastSecondToRequest = PlayerRoomManager.SwapTimeout;
    }
    void CounterCallback(object s)
    {
        LastSecondToRequest--;
        MainThreadDispatcher.ExecuteInMainThreadImidiately(() =>
        {
            btn_AcceptSwapRequest.GetComponentInChildren<TextMeshProUGUI>().text = LastSecondToRequest.ToString();
        });
        if (LastSecondToRequest == 0)
        {
            HidePlayerSwapRequest();
            EndCounter(); 
        }
    }
    void EndCounter()
    {
        counter.Dispose();
    }
   
    public void ToggleStopReqest(bool a)
    {
        btn_StopRequest.gameObject.SetActive(a);
    }
    public void HidePlayerSwapRequest()
    {
        btn_RefuseSwapRequest.interactable = false;
        btn_AcceptSwapRequest.interactable = false;
        EndCounter();

    }
    public void ShowPlayerSwapRequest()
    {
        btn_RefuseSwapRequest.interactable = true;
        btn_AcceptSwapRequest.interactable = true;
        InitCounter();
    }
    void btn_StopRequestAction()
    {
        PlayerRoomManager.localPlayerRoomManager.StopRequestServerRpc(slot);
    }
    public void btn_acceptAction()
    {
        PlayerRoomManager.localPlayerRoomManager.SendAcceptSwapRequestServerRpc(slot,true);
    }
    public void btn_refuseAction()
    {
        PlayerRoomManager.localPlayerRoomManager.SendAcceptSwapRequestServerRpc(slot, false);

    }
    void Start()
    {
        btn_ChangeSlot.onClick.AddListener(btn_ChangeSlotAction);
        btn_KickPlayer.onClick.AddListener(btn_KickPlayerAction);
        btn_SetOwner.onClick.AddListener(Btn_SetOwnerAction);
        btn_SwapRequest.onClick.AddListener(Btn_SwapRequestAction);
        btn_AcceptSwapRequest.onClick.AddListener(btn_acceptAction);
        btn_RefuseSwapRequest.onClick.AddListener(btn_refuseAction);
         btn_StopRequest.onClick.AddListener(btn_StopRequestAction);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
