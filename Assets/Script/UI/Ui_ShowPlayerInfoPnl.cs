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
    public byte slot;
    [SerializeField] Button btn_ChangeSlot,btn_KickPlayer,btn_SetOwner;
   // Show các thuộc tính của client ra màn hình
    public void init(PlayerRoomManager roomManager_)
    {
        
            roomManager = roomManager_;
            // Hiển thị tên người chơi
            lb_PlayerName.SetText( roomManager_.thisPlayer.PlayerName.Value.ToString());
             //  Hiển thị người chơi có phải trưởng phòng hay không
            onHeaderChange(false, roomManager_.isHeader.Value);
            roomManager_.isHeader.OnValueChanged += onHeaderChange;      
    }
    void onHeaderChange(bool old,bool curr)
    {
        img_isHeader.gameObject.SetActive(curr);
    }
    // Xóa hiện thị tên của người chơi
    public void de_init()
    {
      
        // Nếu có hiện tại đang có người chơi đang hiển thị
        if (roomManager != null)
        {
            // Xóa tên người chơi
            lb_PlayerName.text = "";
            // Xóa hiển thị có phải trưởng phòng hay không
            roomManager.isHeader.OnValueChanged -= onHeaderChange;
            roomManager = null;
        }
        // Ẩn icon header
        onHeaderChange(false, false);
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
    void Start()
    {
        btn_ChangeSlot.onClick.AddListener(btn_ChangeSlotAction);
        btn_KickPlayer.onClick.AddListener(btn_KickPlayerAction);
        btn_SetOwner.onClick.AddListener(Btn_SetOwnerAction);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
