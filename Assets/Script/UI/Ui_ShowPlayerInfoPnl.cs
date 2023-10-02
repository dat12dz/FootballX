using Assets.Utlis;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    [SerializeField] Image img_isReady;
    public byte slot;
    [SerializeField] Button btn_ChangeSlot,btn_KickPlayer,btn_SetOwner,btn_SwapRequest,btn_AcceptSwapRequest,btn_RefuseSwapRequest,btn_StopRequest;

   // Show các thuộc tính của client ra màn hình
    public void init(PlayerRoomManager roomManager_)
    {
            roomManager = roomManager_;
            // Hiển thị tên người chơi
            lb_PlayerName.SetText( roomManager_.thisPlayer.PlayerName.Value.ToString());
            //  Hiển thị người chơi có phải trưởng phòng hay không
            onHeaderChange(false, roomManager_.isHeader.Value);
             roomManager_.isHeader.OnValueChanged += onHeaderChange;
           // Check xem người chơi đã sẵn sàng chưa
            OnReadyChange(false,roomManager_.isReady.Value);
            roomManager_.isReady.OnValueChanged += OnReadyChange;
    }
    void onHeaderChange(bool old,bool curr)
    {
        // hiển thị người chơi có là chủ phòng lên ui hay không
        img_isHeader.gameObject.SetActive(curr);
    }
    // Xóa hiện thị tên của người chơi
    void OnReadyChange(bool old, bool curr)
    {
        // hiển thị người chơi có sẵn sàng lên ui hay không
        img_isReady.gameObject.SetActive(curr);
    }
    /// <summary>
    /// Gỡ người chơi khỏi UI,đặt ui về trắng 
    /// trắng : trạng thái không có người chơi
    /// </summary>
    /// <param name="verify"></param>
    public void de_init(PlayerRoomManager verify)
    {
        // nếu người chơi không còn quyền với phòng -> không thực hiện lệnh hủy
        if (roomManager != verify) return; 
        // Nếu có hiện tại đang có người chơi đang hiển thị
        if (roomManager != null)
        {
            // Xóa tên người chơi
            lb_PlayerName.text = "";
            // Xóa hiển thị có phải trưởng phòng hay không
            roomManager.isHeader.OnValueChanged -= onHeaderChange;
            // Xóa hiện thị đã sẵn sàng 

            roomManager.isReady.OnValueChanged -= OnReadyChange;
            // Giải phóng con trỏ vào người chơi
            roomManager = null;
        }
        // Ẩn icon header
        onHeaderChange(false, false);
        OnReadyChange(false, false);
    }
    void btn_ChangeSlotAction()
    {

        if (PlayerRoomManager.localPlayerRoomManager)
        {
            // Gọi hàm để đổi vị trí mới ở trên chỗ trống
            PlayerRoomManager.localPlayerRoomManager.ChangePlayerSlotServerRpc(slot);
        }
    }
    void btn_KickPlayerAction()
    {

        if (roomManager)
        {
            byte KickSlot = roomManager.SlotInRoom.Value;
            // gọi hàm này để kick người chơi ở slot này
            PlayerRoomManager.localPlayerRoomManager.KickPlayerServerRpc(KickSlot);
        }
    }
    void Btn_SetOwnerAction()
    {
        if (roomManager)
            // Gọi hàm để gọi đặt 1 thằng khác làm trưởng phòng
            // Chỉ thực hiện được khi làm trưởng phòng
        PlayerRoomManager.localPlayerRoomManager.SetOwnerServerRpc(slot);
    }
    void Btn_SwapRequestAction()
    {
        // Gọi hàm này khi cần gửi yêu cầu đổi chỗs
        PlayerRoomManager.localPlayerRoomManager.SendSwapRequestServerRpc(slot);
    }
    // Thời gian đếm giờ sẽ hiển thị trên ui
    int LastSecondToRequest;
    Timer counter;
    /// <summary>
    ///  Bắt đầu bộ đếm giờ
    /// </summary>
    void InitCounter()
    {
        counter = new Timer(CounterCallback, null, 0, 1000);
        LastSecondToRequest = PlayerRoomManager.SwapTimeout;
    }
    /// <summary>
    /// Khi đồng hồ chạy được 1 tick thì sẽ gọi hàm này (mặc định 1 tick là 1s)
    /// </summary>
    /// <param name="s">Biến thêm vào cho vui</param>
    void CounterCallback(object s)
    {
        // Giảm thời gian còn lại của đồng hồ đếm giờ
        LastSecondToRequest--;
        MainThreadDispatcher.ExecuteInMainThreadImidiately(() =>
        {
            // Hiển thị thời gian đếm lên ui
            btn_AcceptSwapRequest.GetComponentInChildren<TextMeshProUGUI>().text = LastSecondToRequest.ToString();
        });
        if (LastSecondToRequest == 0)
        {
            // Ngừng hiển thị yêu cầu đổi chỗ của client
            HidePlayerSwapRequest();
            EndCounter(); 
        }
    }
    /// <summary>
    ///  Ngừng đồng hồ đếm giờ
    /// </summary>
    void EndCounter()
    {
        counter.Dispose();
    }
   /// <summary>
   /// Hiển thị hoặc ẩn panel hủy yêu cầu đổi chỗ của người chơi
   /// </summary>
   /// <param name="a">Ẩn (false) hiện (true)</param>
    public void ToggleStopReqest(bool a)
    {
        btn_StopRequest.gameObject.SetActive(a);
    }
    /// <summary>
    /// Ngừng hiển thị (Ẩn) panel yêu cầu đổi chỗ của người chơi 
    /// </summary>
    public void HidePlayerSwapRequest()
    {
        // ẩn file
        // Hiện hai nút
        btn_RefuseSwapRequest.interactable = false;
        btn_AcceptSwapRequest.interactable = false;
        //  Ngừng đồng hồ đếm giờ
        EndCounter();

    }
    /// <summary>
    /// Hiển thị yêu cầu đổi chỗ của người chơi trên ui
    /// </summary>
    public void ShowPlayerSwapRequest()
    {
        btn_RefuseSwapRequest.interactable = true;
        btn_AcceptSwapRequest.interactable = true;
        //  Bắt đầu đồng hồ đếm giờ
        InitCounter();
    }
    /// <summary>
    /// Thực hiện hành động gửi yêu cầu vị t
    /// </summary>
    void btn_StopRequestAction()
    {
        // Gọi hàm để hủy yêu cầu đổi chỗ
        PlayerRoomManager.localPlayerRoomManager.StopRequestServerRpc(slot);
    }
    public void btn_acceptAction()
    {
        // gọi hàm để chấp nhận yêu cầu đổi chỗ
        PlayerRoomManager.localPlayerRoomManager.SendAcceptSwapRequestServerRpc(slot,true);
    }
    public void btn_refuseAction()
    {
        // Gọi hàm để từ chối yêu cầu đổi chỗ
        PlayerRoomManager.localPlayerRoomManager.SendAcceptSwapRequestServerRpc(slot, false);

    }
    void Start()
    {
        // gán hàm vào nút
        btn_ChangeSlot.onClick.AddListener(btn_ChangeSlotAction);
        btn_KickPlayer.onClick.AddListener(btn_KickPlayerAction);
        btn_SetOwner.onClick.AddListener(Btn_SetOwnerAction);
        btn_SwapRequest.onClick.AddListener(Btn_SwapRequestAction);
        btn_AcceptSwapRequest.onClick.AddListener(btn_acceptAction);
        btn_RefuseSwapRequest.onClick.AddListener(btn_refuseAction);
         btn_StopRequest.onClick.AddListener(btn_StopRequestAction);
    }


}
