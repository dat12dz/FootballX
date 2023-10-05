using Assets.Script.Networking.NetworkRoom;
using Assets.Script.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[DefaultExecutionOrder(1)]

public class UI_RoomRenderPnl : RoomRendererBase
{
    
    [SerializeField] TextMeshProUGUI lb_RoomID;

    bool isHostGame;
    [SerializeField] Button btn_leaveRoom;
    [SerializeField] Button btn_StartGame;
        
    [Header("ReadyBtn")]
    [SerializeField] Button btn_ReadyBtn;
   
    [SerializeField] Color OnReadyBtnColor,DefaultReadyBtnColor = Color.white;
/*    public Ui_ShowPlayerInfoPnl GetPlayerInfoRender(int slot)
    {
        return ShowPlayerInfoPnl[slot];
    }*/
/*    public void init(PlayerRoomManager localPlayyer)
    {

    }*/
/// <summary>
/// Hiển thị thông tin phòng 
/// </summary>
/// <param name="RoomInfo">Thông tin phòng được gửi từ server xuống</param>
    public override void init(RoomRenderAble RoomInfo)
    {

        isHostGame = RoomInfo.isHostGame;
        gameObject.active = true;
        OnHeaderChange(false,PlayerRoomManager.localPlayerRoomManager.isHeader.Value);
        lb_RoomID.text = RoomInfo.RoomName.ToString() + " " +  RoomInfo.RoomId.ToString(); 
    }
    /// <summary>
    /// Ẩn UI hiển thị phòng
    /// </summary>
    public override void de_init()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// Hàm được gọi khi quyền làm chủ phòng bị thay đổi hàm sẽ tự động được gọi
    /// </summary>
    /// <param name="old"></param>
    /// <param name="curr">Người chơi có còn là chủ phòng không</param>
    public override void OnHeaderChange(bool old,bool curr)
    {
        // Hiển thị nút sẵn sàng
        btn_ReadyBtn.gameObject.SetActive(!curr);
        // Ẩn nút bắt đầu trận
        btn_StartGame.gameObject.SetActive(curr);
    }
    /// <summary>
    ///  Khi trạng thái sẵn sàng bị thay đổi hàm sẽ tự động được gọi
    /// </summary>
    /// <param name="old"></param>
    /// <param name="curr">Giá trị bị thay đổi</param>
    public override void OnReadyChange(bool old,bool curr)
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
   public override void Btn_LeaveRoomFunc()
    {
       // Gọi hàm này khi rời phòng
        PlayerRoomManager.localPlayerRoomManager.LeaveRoomServerRpc();
        ClearAllRenderer();
        if (isHostGame)
        {
            NetworkManager.Singleton.Shutdown();
     //       SceneManager.LoadScene(0);
        }
        de_init();
    }
   public override void Btn_ReadyAction()
    {
        // Gọi hàm này khi sẵn sàng hoặc hủy sẵn sàng 
        PlayerRoomManager.localPlayerRoomManager.ToggleReadyServerRpc();

    }
    private void Awake()
    {
    }
   public  void Start()
    {
        btn_StartGame.onClick.AddListener(btn_StartGameAction);

        // Lấy toàn bộ UI thẻ người chơi (10 PlayerCard nên lấy cả 10)
        ShowPlayerInfoPnl = GetComponentsInChildren<IUI_PlayerCardBase>();
        ClearAllRenderer();
        gameObject.active = false;
        btn_ReadyBtn.onClick.AddListener(Btn_ReadyAction);
        btn_leaveRoom.onClick.AddListener(Btn_LeaveRoomFunc);
        instance = this;
    }
   public override void btn_StartGameAction()
    {
        PlayerRoomManager.localPlayerRoomManager.StartGameServerRpc();
    }
    /// <summary>
    ///  Xóa toàn bộ dữ liệu cũ còn sót trong PlayerCard
    /// </summary>
   public override void ClearAllRenderer()
    {
        for (byte i = 0; i < ShowPlayerInfoPnl.Length; i++)
        {
            var thisPlayerRender = ShowPlayerInfoPnl[i];
            // Gán slot vào từng player card tương ứng
            thisPlayerRender.slot = i;
            // Xóa tên người chơi nếu bị thừa
            thisPlayerRender.SetPlayerName(null);
        }
    }
  
}
