using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[DefaultExecutionOrder(1)]
public class LobbyScreenRoomRender : UI_RoomRenderPnl
{
    //[SerializeField] TextMeshProUGUI lb_RoomID;
    //Button btn_leaveRoom;
    //Button btn_StartGame;
    //[Header("ReadyBtn")]
    //Button btn_ReadyBtn;
    //[SerializeField] Color OnReadyBtnColor,DefaultReadyBtnColor = Color.white;

    public static LobbyScreenRoomRender instance;
    public LobbyScreenShowPlayer[] showPlayerInfo = new LobbyScreenShowPlayer[10];
    private VisualElement root;
    private VisualElement container;
    private Label roomName;
    private Label roomId;
    private static VisualElement exitBtn;
    private Button readyBtn;
    private Button startBtn;

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
        //gameObject.active = true;
        container.style.display = DisplayStyle.Flex;
        OnHeaderChange(false,PlayerRoomManager.localPlayerRoomManager.isHeader.Value);
        roomName.text = RoomInfo.RoomName.ToString();
        roomId.text = RoomInfo.RoomId.ToString();
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
        //btn_ReadyBtn.gameObject.SetActive(!curr);
        // Ẩn nút bắt đầu trận
        //btn_StartGame.gameObject.SetActive(curr);
    }
    /// <summary>
    ///  Khi trạng thái sẵn sàng bị thay đổi hàm sẽ tự động được gọi
    /// </summary>
    /// <param name="old"></param>
    /// <param name="curr">Giá trị bị thay đổi</param>
    public void OnReadyChange(bool old,bool curr)
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
        //btn_StartGame.onClick.AddListener(btn_StartGameAction);

        // Lấy toàn bộ UI thẻ người chơi (10 PlayerCard nên lấy cả 10)
        // showPlayerInfo = GetComponentsInChildren<LobbyScreenShowPlayer>();
        
        //gameObject.active = false;
        //btn_ReadyBtn.onClick.AddListener(Btn_ReadyAction);
        //btn_leaveRoom.onClick.AddListener(Btn_LeaveRoomFunc);
       
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        roomId = root.Q<Label>("room-id");
        roomName = root.Q<Label>("room-name");
        readyBtn = root.Q<Button>("ready-btn");
        startBtn = root.Q<Button>("start-btn");
        exitBtn = root.Q<VisualElement>("exit-btn");

        var players = root.Query("player-card").ToList();
        for(byte i = 0; i <  players.Count; i++)
        {
            showPlayerInfo[i] = new LobbyScreenShowPlayer(players[i], i);
        }

        ClearAllRenderer();

        exitBtn.RegisterCallback<PointerDownEvent>(callback =>
        {
            Btn_LeaveRoomFunc();
        });

        readyBtn.clicked += () => 
        {
            Btn_ReadyAction();
        };

        startBtn.clicked += () =>
        {
            Btn_StartGameAction();
        };
    }
    void Btn_StartGameAction()
    {
        PlayerRoomManager.localPlayerRoomManager.StartGameServerRpc();
    }
    /// <summary>
    ///  Xóa toàn bộ dữ liệu cũ còn sót trong PlayerCard
    /// </summary>
    void ClearAllRenderer()
    {
        for (byte i = 0; i < showPlayerInfo.Length; i++)
        {
            var thisPlayerRender = showPlayerInfo[i];
            // Gán slot vào từng player card tương ứng
            thisPlayerRender.slot = i;
            // Xóa tên người chơi nếu bị thừa
            //thisPlayerRender.lb_PlayerName.text = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
