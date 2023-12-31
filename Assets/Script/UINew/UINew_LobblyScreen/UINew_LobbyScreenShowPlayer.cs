using Assets.Script.Networking.NetworkRoom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[DefaultExecutionOrder(1)]
public class UINew_LobbyScreenShowPlayer : IUI_PlayerCardBase
{
    private readonly VisualElement playerDescription;
    private readonly VisualElement playerEmpty;
    private readonly VisualElement player;
    private readonly VisualElement playerAvatar;
    private readonly Label playerName;
    private readonly VisualElement playerLeader;
    private readonly VisualElement playerReady;
    private readonly VisualElement notify;
    private readonly Label notifyDescription;
    private readonly ProgressBar progressBar;
    private readonly Button notifyYesBtn;
    private readonly Button notifyNoBtn;
    private readonly Button notifyStopRequestBtn;
    private readonly Button kickBtn;
    private readonly Button changeLeaderBtn;
    private readonly Button swapBtn; 
    // Thời gian đếm giờ sẽ hiển thị trên ui
    Timer counter;
    public PlayerRoomManager roomManager { set; get; }
    public byte slot { set; get; }
    public UINew_LobbyScreenShowPlayer(VisualElement player, byte slot)
    {
        this.player = player;
        this.slot = slot;
        playerDescription = player.Q<VisualElement>("player-description");
        playerEmpty = player.Q<VisualElement>("player-empty");
        playerAvatar = player.Q<VisualElement>("avatar");
        playerName = player.Q<Label>("name");
        playerLeader = player.Q<VisualElement>("leader");
        playerReady = player.Q<VisualElement>("ready");
        swapBtn = player.Q<Button>("swap-btn");
        kickBtn = player.Q<Button>("kick-btn");
        changeLeaderBtn = player.Q<Button>("change-leader-btn");
        notify = player.Q<VisualElement>("notify");
        notifyDescription = player.Q<Label>("notify-description");
        progressBar = player.Q<ProgressBar>("ProgressBar");
        notifyYesBtn = player.Q<Button>("notify-yes-btn");
        notifyNoBtn = player.Q<Button>("notify-no-btn");
        notifyStopRequestBtn = player.Q<Button>("notify-stop-request-btn");

        // Thêm sự kiện click
        playerEmpty.RegisterCallback<PointerDownEvent>(callback =>
        {
            btn_ChangeSlotAction();
        });

        swapBtn.clicked += Btn_SwapRequestAction;

        kickBtn.clicked += btn_KickPlayerAction;

        changeLeaderBtn.clicked += Btn_SetOwnerAction;

        notifyYesBtn.clicked += btn_acceptAction;

        notifyNoBtn.clicked += btn_refuseAction;

        notifyStopRequestBtn.clicked += btn_StopRequestAction;
    }

    // Thiết lập các giá trị
    //public TextMeshProUGUI lb_PlayerName;
    //Button KickBtn;
    
    //public Image img_isHeader;

    //Image img_isReady;
    //Button btn_ChangeSlot,btn_KickPlayer,btn_SetOwner,btn_SwapRequest,btn_AcceptSwapRequest,btn_RefuseSwapRequest,btn_StopRequest;

    // Show các thuộc tính của client ra màn hình
    public void init(PlayerRoomManager roomManager_)
    {
        roomManager = roomManager_;
        // Hiển thị tên người chơi
        playerName.text = roomManager_.thisPlayer.initialPlayerData.Value.playerName.ToString();
        //lb_PlayerName.SetText( roomManager_.thisPlayer.PlayerName.Value.ToString());

        // Đóng playerEmpty và mở playerDescription
        playerEmpty.style.display = DisplayStyle.None;
        playerDescription.style.display = DisplayStyle.Flex;

        //  Hiển thị người chơi có phải trưởng phòng hay không
        onHeaderChange(false, roomManager_.isHeader.Value);
        roomManager_.isHeader.OnValueChanged += onHeaderChange;

        // Check xem người chơi đã sẵn sàng chưa
        OnReadyChange(false, roomManager_.isReady.Value);
        roomManager_.isReady.OnValueChanged += OnReadyChange;

    }
    public void onHeaderChange(bool old,bool curr)
    {
        // hiển thị người chơi có là chủ phòng lên ui hay không
        if(curr)
        {
            playerLeader.style.display = DisplayStyle.Flex;
        }
        else playerLeader.style.display = DisplayStyle.None;
        //img_isHeader.gameObject.SetActive(curr);
    }
    // Xóa hiện thị tên của người chơi
    public void OnReadyChange(bool old, bool curr)
    {
        // hiển thị người chơi có sẵn sàng lên ui hay không
        if (curr)
        {
            playerReady.style.display = DisplayStyle.Flex;
        }
        else playerReady.style.display = DisplayStyle.None;
        //img_isReady.gameObject.SetActive(curr);
    }
    public void de_init(PlayerRoomManager verify)
    {
        // nếu người chơi không còn quyền với phòng -> không thực hiện lệnh hủy
        if (roomManager != verify) return;
        // Nếu có hiện tại đang có người chơi đang hiển thị
        if (roomManager != null)
        {
            // Xóa tên người chơi
            playerName.text = "";
            //lb_PlayerName.text = "";
            // Xóa hiển thị có phải trưởng phòng hay không
            roomManager.isHeader.OnValueChanged -= onHeaderChange;
            // Xóa hiện thị đã sẵn sàng 
            roomManager.isReady.OnValueChanged -= OnReadyChange;
            // Giải phóng con trỏ vào người chơi
            roomManager = null;

            playerEmpty.style.display = DisplayStyle.Flex;
            playerDescription.style.display = DisplayStyle.None;
        }
        // Ẩn icon header
        onHeaderChange(false, false);
        OnReadyChange(false, false);
    }
    public void btn_ChangeSlotAction()
    {
        if (PlayerRoomManager.localPlayerRoomManager)
        {
            // Gọi hàm để đổi vị trí mới ở trên chỗ trống
            PlayerRoomManager.localPlayerRoomManager.ChangePlayerSlotServerRpc(slot);
        }
    }
    public void btn_KickPlayerAction()
    {
        if (roomManager)
        {
            byte KickSlot = roomManager.SlotInRoom.Value;
            // gọi hàm này để kick người chơi ở slot này
            PlayerRoomManager.localPlayerRoomManager.KickPlayerServerRpc(KickSlot);
        }
    }
    public void Btn_SetOwnerAction()
    {
        if (roomManager)
            // Gọi hàm để gọi đặt 1 thằng khác làm trưởng phòng
            // Chỉ thực hiện được khi làm trưởng phòng
            PlayerRoomManager.localPlayerRoomManager.SetOwnerServerRpc(slot);
    }

    public void Btn_SwapRequestAction()
    {
        // Gọi hàm này khi cần gửi yêu cầu đổi chỗ
        PlayerRoomManager.localPlayerRoomManager.SendSwapRequestServerRpc(slot);
        ShowNotify();
        notifyYesBtn.style.display = DisplayStyle.None;
        notifyNoBtn.style.display = DisplayStyle.None;
        notifyStopRequestBtn.style.display = DisplayStyle.Flex;
        notifyDescription.text = "Hủy đổi chỗ";
    }
    
    /// <summary>
    ///  Bắt đầu bộ đếm giờ
    /// </summary>
    public void InitCounter()
    {
        counter = new Timer(CounterCallback, null, 0, 1000);
        progressBar.highValue = PlayerRoomManager.SwapTimeout;
        progressBar.value = progressBar.highValue;
    }
    /// <summary>
    /// Khi đồng hồ chạy được 1 tick thì sẽ gọi hàm này (mặc định 1 tick là 1s)
    /// </summary>
    /// <param name="s">Biến thêm vào cho vui</param>
    public void CounterCallback(object s)
    {
        MainThreadDispatcher.ExecuteInMainThreadImidiately(() =>
        {
            // Giảm thời gian còn lại của đồng hồ đếm giờ
            // Hiển thị thời gian đếm lên ui
            progressBar.value--;
            //btn_AcceptSwapRequest.GetComponentInChildren<TextMeshProUGUI>().text = LastSecondToRequest.ToString();
        });
        if (progressBar.value == 0)
        {
            // Ngừng hiển thị yêu cầu đổi chỗ của client
            HidePlayerSwapRequest();
            HideNotify();
        }
    }
    /// <summary>
    ///  Ngừng đồng hồ đếm giờ
    /// </summary>
    public void EndCounter()
    {
        counter.Dispose();
    }
    /// <summary>
    /// Hiển thị hoặc ẩn panel hủy yêu cầu đổi chỗ của người chơi
    /// </summary>
    /// <param name="a">Ẩn (false) hiện (true)</param>
    // Người gửi
    public void ToggleStopReqest(bool a)
    {
        //btn_StopRequest.gameObject.SetActive(a);
        if (a)
        {
            ShowNotify();
        }
        else
        {
            HideNotify();
        }
    }
    /// <summary>
    /// Ngừng hiển thị (Ẩn) panel yêu cầu đổi chỗ của người chơi
    /// </summary>
    //Người nhận
    public void HidePlayerSwapRequest()
    {
        // ẩn file
        // Hiện hai nút
        //btn_RefuseSwapRequest.interactable = false;
        //btn_AcceptSwapRequest.interactable = false;
        //  Ngừng đồng hồ đếm giờ
        EndCounter();
        HideNotify();
        notifyYesBtn.style.display = DisplayStyle.None;
        notifyNoBtn.style.display = DisplayStyle.None;
        notifyStopRequestBtn.style.display = DisplayStyle.Flex;
    }
    /// <summary>
    /// Hiển thị yêu cầu đổi chỗ của người chơi trên ui
    /// </summary>
    public void ShowPlayerSwapRequest()
    {
        ShowNotify();
        notifyDescription.text = $"{roomManager.thisPlayer.initialPlayerData.Value.playerName} muốn đổi chỗ với bạn";
        notifyYesBtn.style.display = DisplayStyle.Flex;
        notifyNoBtn.style.display = DisplayStyle.Flex;
        notifyStopRequestBtn.style.display = DisplayStyle.None;
        //btn_RefuseSwapRequest.interactable = true;
        //btn_AcceptSwapRequest.interactable = true;
        //  Bắt đầu đồng hồ đếm giờ
        InitCounter();
    }
    /// <summary>
    /// Thực hiện hành động gửi yêu cầu vị t
    /// </summary>
    public void btn_StopRequestAction()
    {
        // Gọi hàm để hủy yêu cầu đổi chỗ
        PlayerRoomManager.localPlayerRoomManager.StopRequestServerRpc(slot);
    }

    public void btn_acceptAction()
    {
        // gọi hàm để chấp nhận yêu cầu đổi chỗ
        PlayerRoomManager.localPlayerRoomManager.SendAcceptSwapRequestServerRpc(slot, true);
        HideNotify();
    }

    public void btn_refuseAction()
    {
        // Gọi hàm để từ chối yêu cầu đổi chỗ
        PlayerRoomManager.localPlayerRoomManager.SendAcceptSwapRequestServerRpc(slot, false);
    }

    public void SetPlayerName(string Name)
    {
        
    }
    async void ShowNotify()
    {
        notify.style.transitionTimingFunction = new List<EasingFunction>()
        {
            new EasingFunction(EasingMode.EaseOutCubic)
        };
        notify.style.width = 120f;
        await Task.Delay(400);
    }

    async void HideNotify()
    {
        notify.style.transitionTimingFunction = new List<EasingFunction>()
        {
            new EasingFunction(EasingMode.EaseInCubic)
        };
        notify.style.width = 0f;
        await Task.Delay(400);
    }

}
