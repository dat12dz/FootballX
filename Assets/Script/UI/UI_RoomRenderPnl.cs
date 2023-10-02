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
    public void init(RoomRenderAble RoomInfo)
    {
        gameObject.active = true;
                OnHeaderChange(false,PlayerRoomManager.localPlayerRoomManager.isHeader.Value);

        lb_RoomID.text = RoomInfo.RoomId.ToString();
    }
    /// <summary>
    /// Ẩn UI hiển thị phòng
    /// </summary>
    public void de_init()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// Hàm được gọi khi quyền làm chủ phòng bị thay đổi hàm sẽ tự động được gọi
    /// </summary>
    /// <param name="old"></param>
    /// <param name="curr">Người chơi có còn là chủ phòng không</param>
    public void OnHeaderChange(bool old,bool curr)
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
       // Gọi hàm này khi rời phòng
        PlayerRoomManager.localPlayerRoomManager.LeaveRoomServerRpc();
        ClearAllRenderer();
        de_init();
    }
    void Btn_ReadyAction()
    {
        // Gọi hàm này khi sẵn sàng hoặc hủy sẵn sàng 
        PlayerRoomManager.localPlayerRoomManager.ToggleReadyServerRpc();

    }
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        btn_StartGame.onClick.AddListener(btn_StartGameAction);

        // Lấy toàn bộ UI thẻ người chơi (10 PlayerCard nên lấy cả 10)
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
    /// <summary>
    ///  Xóa toàn bộ dữ liệu cũ còn sót trong PlayerCard
    /// </summary>
    void ClearAllRenderer()
    {
        for (byte i = 0; i < ShowPlayerInfoPnl.Length; i++)
        {
            var thisPlayerRender = ShowPlayerInfoPnl[i];
            // Gán slot vào từng player card tương ứng
            thisPlayerRender.slot = i;
            // Xóa tên người chơi nếu bị thừa
            thisPlayerRender.lb_PlayerName.text = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
