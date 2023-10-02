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
    [SerializeField] Button btn_StartGame;
    [Header("ReadyBtn")]
    [SerializeField] Button btn_ReadyBtn;
    [SerializeField] Color OnReadyBtnColor,DefaultReadyBtnColor = Color.white;
    public Ui_ShowPlayerInfoPnl GetPlayerInfoRender(int slot)
    {
        return ShowPlayerInfoPnl[slot];
    }
    public void init(PlayerRoomManager localPlayyer)
    {

    }
    public void init(RoomRenderAble RoomInfo)
    {
        gameObject.active = true;
                OnHeaderChange(false,PlayerRoomManager.localPlayerRoomManager.isHeader.Value);

        lb_RoomID.text = RoomInfo.RoomId.ToString();
    }
    public void de_init()
    {
        gameObject.SetActive(false);
    }
    public void OnHeaderChange(bool old,bool curr)
    {
        btn_ReadyBtn.gameObject.SetActive(!curr);
        btn_StartGame.gameObject.SetActive(curr);
    }
    public void OnReadyChange(bool old,bool curr)
    {
        if (curr)
        {
            ColorBlock ButtonColor = btn_ReadyBtn.colors;
            ButtonColor.selectedColor = OnReadyBtnColor;
            btn_ReadyBtn.colors = ButtonColor;
        }
        else
        {
            ColorBlock ButtonColor = btn_ReadyBtn.colors;
            ButtonColor.selectedColor = DefaultReadyBtnColor;
            btn_ReadyBtn.colors = ButtonColor;
        }

    }
    void Btn_LeaveRoomFunc()
    {
        PlayerRoomManager.localPlayerRoomManager.LeaveRoomServerRpc();
        ClearAllRenderer();
        de_init();
    }
    void Btn_ReadyAction()
    {
        PlayerRoomManager.localPlayerRoomManager.ToggleReadyServerRpc();

    }
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        btn_StartGame.onClick.AddListener(btn_StartGameAction);

        // Get all
        ShowPlayerInfoPnl = GetComponentsInChildren<Ui_ShowPlayerInfoPnl>();
        ClearAllRenderer();
        gameObject.active = false;
        btn_ReadyBtn.onClick.AddListener(Btn_ReadyAction);
        btn_leaveRoom.onClick.AddListener(Btn_LeaveRoomFunc);
    }
    void btn_StartGameAction()
    {
        PlayerRoomManager.localPlayerRoomManager.StartGameServerRpc();
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
