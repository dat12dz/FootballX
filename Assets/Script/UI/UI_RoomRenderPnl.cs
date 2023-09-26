using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(1)]
public class UI_RoomRenderPnl : MonoBehaviour
{
    public static UI_RoomRenderPnl instance;    
    [SerializeField] TextMeshProUGUI lb_RoomID;
    public Ui_ShowPlayerInfoPnl[] ShowPlayerInfoPnl;
    [SerializeField] Button btn_leaveRoom;
    public Ui_ShowPlayerInfoPnl GetPlayerInfoRender(int slot)
    {
        return ShowPlayerInfoPnl[slot];
    }
    public void init(PlayerRoomManager localPlayyer)
    {
        localPlayyer.RoomID.OnValueChanged += OnRoomIDChange;
    }
    public void OnRoomIDChange(uint old,uint curr)
    {
    }
    void ChangeRoomID()
    {
    }
    public void init(RoomRenderAble RoomInfo)
    {

        gameObject.active = true;
        lb_RoomID.text = RoomInfo.RoomId.ToString();
    }
    public void de_init()
    {
        gameObject.SetActive(false);
    }

    void GetLocalClientManager()
    {

    }
    void Btn_LeaveRoomFunc()
    {
        PlayerRoomManager.localPlayerRoomManager.LeaveRoomServerRpc();
        ClearAllRenderer();
        de_init();
    }
    void Start()
    {
        // Get all
        ShowPlayerInfoPnl = GetComponentsInChildren<Ui_ShowPlayerInfoPnl>();
        ClearAllRenderer();
        instance = this;
        gameObject.active = false;

        btn_leaveRoom.onClick.AddListener(Btn_LeaveRoomFunc);
    }
    void ClearAllRenderer()
    {
        for (byte i = 0; i < ShowPlayerInfoPnl.Length; i++)
        {
            var thisPlayerRender = ShowPlayerInfoPnl[i];
            thisPlayerRender.slot = i;
            thisPlayerRender.lb_PlayerName.text = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
