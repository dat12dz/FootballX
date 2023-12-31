using Assets.Script.Networking.NetworkRoom;
using Assets.Script.UI;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

//[DefaultExecutionOrder(1)]
[RequireComponent(typeof(UIBase))]
public class UINew_LobbyScreenRoomRender : RoomRendererBase
{
    //[SerializeField] TextMeshProUGUI lb_RoomID;
    //Button btn_leaveRoom;
    //Button btn_StartGame;
    //[Header("ReadyBtn")]
    //Button btn_ReadyBtn;
    //[SerializeField] Color OnReadyBtnColor,DefaultReadyBtnColor = Color.white;

    private VisualElement root;
    private VisualElement container;
    private Label roomName;
    private Label roomId;
    private static Button exitBtn;
    private Button readyBtn;
    private Button startBtn;
    private bool isHostGame;

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
        //Debug.LogError("init");
        //gameObject.active = true;
        UINew_LobbyScreen.Show();
        OnHeaderChange(false,PlayerRoomManager.localPlayerRoomManager.isHeader.Value);
        roomName.text = RoomInfo.RoomName.ToString();
        roomId.text = RoomInfo.RoomId.ToString();
        isHostGame = RoomInfo.isHostGame;
    }
    /// <summary>
    /// Ẩn UI hiển thị phòng
    /// </summary>
    public override void de_init()
    {
        //gameObject.SetActive(false);
        //UINew_LobbyScreen.ResetStyle();
    }
    /// <summary>
    /// Hàm được gọi khi quyền làm chủ phòng bị thay đổi hàm sẽ tự động được gọi
    /// </summary>
    /// <param name="old"></param>
    /// <param name="curr">Người chơi có còn là chủ phòng không</param>
    public override void OnHeaderChange(bool old,bool curr)
    {
        // Hiển thị nút sẵn sàng
        //btn_ReadyBtn.gameObject.SetActive(!curr);
        if(curr)
        {
            readyBtn.style.display = DisplayStyle.None;
            startBtn.style.display = DisplayStyle.Flex;
        }
        else
        {
            readyBtn.style.display = DisplayStyle.Flex;
            startBtn.style.display = DisplayStyle.None;
        }
        // Ẩn nút bắt đầu trận
        //btn_StartGame.gameObject.SetActive(curr);
    }
    /// <summary>
    ///  Khi trạng thái sẵn sàng bị thay đổi hàm sẽ tự động được gọi
    /// </summary>
    /// <param name="old"></param>
    /// <param name="curr">Giá trị bị thay đổi</param>
    public override void OnReadyChange(bool old,bool curr)
    {
        //if (curr)
        //{
        //    ColorBlock ButtonColor = btn_ReadyBtn.colors;
        //    ButtonColor.selectedColor = OnReadyBtnColor;
        //    btn_ReadyBtn.colors = ButtonColor;
        //}
        //else
        //{
        //    ColorBlock ButtonColor = btn_ReadyBtn.colors;
        //    ButtonColor.selectedColor = DefaultReadyBtnColor;
        //    btn_ReadyBtn.colors = ButtonColor;
        //}
    }
    public override void Btn_LeaveRoomFunc()
    {
       // Gọi hàm này khi rời phòng
        PlayerRoomManager.localPlayerRoomManager.LeaveRoomServerRpc();
        ClearAllRenderer();
        if (isHostGame)
        {
            NetworkManager.Singleton.Shutdown();
        }
        de_init();
    }
    public override void Btn_ReadyAction()
    {
        // Gọi hàm này khi sẵn sàng hoặc hủy sẵn sàng 
        PlayerRoomManager.localPlayerRoomManager.ToggleReadyServerRpc();

    }
    void Start()
    {
  
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        roomId = root.Q<Label>("room-id");
        roomName = root.Q<Label>("room-name");
        readyBtn = root.Q<Button>("ready-btn");
        startBtn = root.Q<Button>("start-btn");
        exitBtn = root.Q<Button>("exit-btn");
        players = root.Query("player-card").ToList();
        RenderPlayerCard();
        ClearAllRenderer();

        exitBtn.clicked += Btn_LeaveRoomFunc;

        readyBtn.clicked += Btn_ReadyAction;

        startBtn.clicked += btn_StartGameAction; 

        instance = this;
    }

   List<VisualElement> players;
    private void RenderPlayerCard()
    {
       if (players != null)
        for (byte i = 0; i < players.Count; i++)
        {
            IUI_PlayerCardBase playerCard = new UINew_LobbyScreenShowPlayer(players[i], i);
            ShowPlayerInfoPnl[i] = playerCard;
        }
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
            //thisPlayerRender.lb_PlayerName.text = null;
        }
    }
    // Update is called once per frame
    public override void Show(bool show)
    {
        if (!show)
        root.style.display = DisplayStyle.None;
        else
            root.style.display = DisplayStyle.Flex;

    }
}
