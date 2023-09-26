using Assets.Utlis;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(0)]
public class Ui_ShowPlayerInfoPnl : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI lb_PlayerName;
    [SerializeField] Button KickBtn;
    public PlayerRoomManager roomManager;
    public Image img_isHeader;
    public byte slot;
    [SerializeField] Button btn_ChangeSlot,btn_KickPlayer,btn_SetOwner;
   
    public void init(PlayerRoomManager roomManager_)
    {

            roomManager = roomManager_;
            lb_PlayerName.text = roomManager_.thisPlayer.PlayerName.Value.ToString();
            onHeaderChange(false, roomManager_.isHeader.Value);
            roomManager_.isHeader.OnValueChanged += onHeaderChange;      
    }
    void onHeaderChange(bool old,bool curr)
    {
        img_isHeader.gameObject.SetActive(curr);
    }
    public void de_init()
    {
        lb_PlayerName.text = null;
        roomManager.isHeader.OnValueChanged -= onHeaderChange;
        roomManager = null;
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
