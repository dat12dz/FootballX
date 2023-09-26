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
        PlayerRoomManager.GetRoomManger(NetworkManager.Singleton.LocalClientId);
    }
    public void init(RoomRenderAble RoomInfo)
    {

        gameObject.active = true;
        lb_RoomID.text = RoomInfo.RoomId.ToString();
    }
    void Start()
    {
        instance = this;
        gameObject.active = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
