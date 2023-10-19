using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;


public class UI_CharacterSelection : MonoBehaviour
{
    // Tham chiếu vào tủ nhân vật
    [SerializeField] AllCharectorAssetReference AllChar;
    [SerializeField] Button btn_ChangeCharBase, btn_SelectChar;
    [SerializeField] Button btn_ChangeTeamBlue, btn_ChangeTeamRed;
    [SerializeField] Button btn_exit;
    // Tham chiếu vào UI khởi đầu (UI có nút host,connect)
    [SerializeField] UI_StartScreenHandler startScreenHandler;
    // Khoảng giãn cách
    [SerializeField] float Spacing = 10;
    // Vị trí spawn người chơi
    [SerializeField] Transform SpawnPosition;

    [SerializeField] Transform CharShowCase;
    // Camera ảo
    [SerializeField] CinemachineVirtualCamera camera;
    // Dữ liệu team để hiển thị
    public enum TeamColorEnum
    {
        red,blue
    }
    public TeamColorEnum team;
    // Nhân vật đang hiển thị
    int CharIndex_;
    int CharIndex
    {
        get { return CharIndex_; }
        set
        {
            ShowChar(value);
            CharIndex_ = value;
        }
    }
    // Nhân vật đang được chọn
    int SelectedChar;
     void Start()
    {
        
        SpawnAllButton();
        CharIndex = 0;

        btn_SelectChar.onClick.AddListener(Btn_SelectCharAction);
        btn_ChangeTeamBlue.onClick.AddListener(Btn_ChangeColorBlue);
        btn_ChangeTeamRed.onClick.AddListener(Btn_ChangeColorRed);
        btn_exit.onClick.AddListener(btn_ExitAction);
        
    }
    private void OnEnable()
    {
        Application.targetFrameRate = 60;
    }
    private void OnDisable()
    {
        Application.targetFrameRate = 0;
    }
    public void Display(bool a)
    {
        transform.parent.gameObject.SetActive(a);

    }
    List<PlayerModelBase> models = new List<PlayerModelBase>();
    PlayerModelBase oldModel;
    /// <summary>
    /// Sinh ra tất cả các nút đồng thời sinh ra nhân vật và tạm thời ẩn chúng khi không cần thiết
    /// </summary>
    void SpawnAllButton()
    {
        // lấy nơi sinh ra đầu tiên của nút (UI cũ)
        var SpawnInitialLocaltion = btn_ChangeCharBase.GetComponent<RectTransform>();
        // Lặp qua từng nhân vật
        for (int i = 0; i < AllChar.CharArray.Length; i++)
        {

            PlayerModelBase modelinfo = AllChar.CharArray[i];
            // Nếu nhân vật bằng null -> không tiếp tục
            if (modelinfo == null) continue;
            // Sinh nút
            var newButton = Instantiate(btn_ChangeCharBase, transform);
            RectTransform buttonTransform = newButton.GetComponent<RectTransform>();
            newButton.gameObject.SetActive(true);
            

            // Sinh nhân vật
            PlayerModelBase newModelInfo = Instantiate(modelinfo, SpawnPosition.position, SpawnPosition.rotation, CharShowCase);
            // Gán hình nhân vật cho nút
            Texture2D thumb = newModelInfo.Thumbnail;
            newButton.GetComponent<Image>().sprite = Sprite.Create(thumb ,new Rect(0,0, thumb.width,thumb.height),Vector2.zero);
        
            // tính toán khoảng cách giữa 2 nút (UI cũ)
            float newPositionX = SpawnInitialLocaltion.position.x + i * (Spacing + buttonTransform.rect.width);
            buttonTransform.position = new Vector3(newPositionX, buttonTransform.position.y);

            // Gán sự kiện chuyển nhân vật cho nút
            var index = i;
            newButton.onClick.AddListener(() =>
            {
                CharIndex = index;
            });
            // thêm nhân vật vào danh sánh nhân vật đang load vào ram
            models.Add(newModelInfo);
            // Ẩn tạm thời nhân vật
            newModelInfo.gameObject.SetActive(false);
        }
    }
    void ShowChar(int index)
    {
        // if (CharIndex == index) return;
        // Lấy nhân vật được chọn
        PlayerModelBase selectedPlayerModel = models[index];
        // Ẩn nhân vật cũ
        if (oldModel)
            oldModel.gameObject.SetActive(false);
        // Hiện nhân vật mới
        selectedPlayerModel.gameObject.SetActive(true);
        oldModel = selectedPlayerModel;
        // Hiển thị màu của team nhân vật
        if (team == TeamColorEnum.blue)
        {
            selectedPlayerModel.WaitForStart_(() => selectedPlayerModel.BlueTeamInit());

        }
        else
        {
            selectedPlayerModel.WaitForStart_(() => selectedPlayerModel.RedTeamInit());
        }
        // bắt đầu animation cho nhân vật
        if (SelectedChar == index)
        {
            PlaySelectedAnimation(selectedPlayerModel);
        }
        else
        {
            PlayUnSelectedAnimation(selectedPlayerModel);
        }
        oldModel = selectedPlayerModel;

        camera.LookAt = selectedPlayerModel.CameraLookAt;
    }
    public void Btn_SelectCharAction()
    {
        SelectedChar = CharIndex;
        ShowChar(CharIndex);
    }
    public void Btn_ChangeColorBlue()
    {
        team = TeamColorEnum.blue;
        ShowChar(CharIndex);
    }
    public void Btn_ChangeColorRed()
    {
        team = TeamColorEnum.red;
        ShowChar(CharIndex);
    }
    public void btn_ExitAction()
    {
        startScreenHandler.gameObject.SetActive(true);
        Display(false);
        StartGameInfo.instance.playerData.playerChar = SelectedChar;
    }
    public void PlaySelectedAnimation(PlayerModelBase model)
    {
        model.WaitForStart_(() =>  model.SelectedAnim());
        DOTween.To(() => camera.m_Lens.FieldOfView, (x) => camera.m_Lens.FieldOfView = x, Camera.FocalLengthToFieldOfView(2.1f, 1), 1).SetEase(Ease.InOutQuart);
        btn_SelectChar.interactable = false;
    }
    public void PlayUnSelectedAnimation(PlayerModelBase player)
    {
      player.WaitForStart_(() =>   player.IdleAnim());
        DOTween.To(() => camera.m_Lens.FieldOfView, (x) => camera.m_Lens.FieldOfView = x, Camera.FocalLengthToFieldOfView(1.6f,1) , 1).SetEase(Ease.InOutQuart);
        btn_SelectChar.interactable = true;
    }

    void Update()
    {
        
    }
}
