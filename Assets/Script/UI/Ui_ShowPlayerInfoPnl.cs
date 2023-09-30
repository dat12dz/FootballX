using Assets.Utlis;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Button btn_ChangeSlot,btn_KickPlayer,btn_SetOwner,btn_SwapRequest;

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
        img_isHeader.gameObject.SetActive(curr);
    }
    // Xóa hiện thị tên của người chơi
    void OnReadyChange(bool old, bool curr)
    {
        img_isReady.gameObject.SetActive(curr);
    }
    public void de_init()
    {
      
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

    }
    void Start()
    {
        btn_ChangeSlot.onClick.AddListener(btn_ChangeSlotAction);
        btn_KickPlayer.onClick.AddListener(btn_KickPlayerAction);
        btn_SetOwner.onClick.AddListener(Btn_SetOwnerAction);
        btn_SwapRequest.onClick.AddListener(Btn_SwapRequestAction);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
