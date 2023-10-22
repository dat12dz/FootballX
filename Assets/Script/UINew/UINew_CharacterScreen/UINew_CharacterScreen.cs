using Cinemachine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
public class UINew_CharacterScreen : MonoBehaviour
{
    [SerializeField] AllCharectorAssetReference allChar;
    // Vị trí spawn người chơi
    [SerializeField] Transform SpawnPosition;
    [SerializeField] Transform CharShowCase;
    // Camera ảo
    [SerializeField] CinemachineVirtualCamera camera,BackgroundCamera;
    // Tham chiếu vào UI khởi đầu (UI có nút host,connect)
    [SerializeField] UINew_MultiplePlayerScreen startScreenHandler;

    VisualElement root;
    VisualElement container;
    VisualElement characterScreen;
    VisualElement unityContentContainer;
    Button exitBtn;
    Button selectBtn;
    Button redClothes;
    Button blueClothes;
    Button[] charBtns;
    UINew_CharacterSelection characterSelection;
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
    List<PlayerModelBase> models = new List<PlayerModelBase>();
    PlayerModelBase oldModel;

    // Dữ liệu team để hiển thị
    public enum TeamColorEnum
    {
        red, blue
    }
    public TeamColorEnum team;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        characterScreen = root.Q<VisualElement>("character-screen");
        unityContentContainer = root.Q<VisualElement>("unity-content-container");
        exitBtn = root.Q<Button>("exit-btn");
        selectBtn = root.Q<Button>("select-btn");
        redClothes = root.Q<Button>("red-clothes");
        blueClothes = root.Q<Button>("blue-clothes");
        characterSelection = GetComponent<UINew_CharacterSelection>();
        SpawnAllButton();
        CharIndex = 1;

        selectBtn.clicked += () =>
        {
            Btn_SelectCharAction();
        };

        redClothes.clicked += () =>
        {
            Btn_ChangeColorRed();
        };

        blueClothes.clicked += () =>
        {
            Btn_ChangeColorBlue();
        };

        exitBtn.clicked += () =>
        {
            btn_ExitAction();
        };
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
        //characterSelection.gameObject.SetActive(a);
        UINew_CharacterSelection.SetActive(a);
    }

    void SpawnAllButton()
    {
        for (int i = 0; i < allChar.CharArray.Length; i++)
        {
            PlayerModelBase modelinfo = allChar.CharArray[i];
            // Nếu nhân vật bằng null -> không tiếp tục
            if (modelinfo == null) continue;

            // Sinh nút
            Button b = new Button();
            b.AddToClassList("character");
            b.AddToClassList("btn-handle");
            b.style.backgroundImage = allChar.CharArray[i].Thumbnail;
            unityContentContainer.Add(b);

            // Sinh nhân vật
            PlayerModelBase newModelInfo = Instantiate(modelinfo, SpawnPosition.position, SpawnPosition.rotation, CharShowCase);

            // Gán sự kiện chuyển nhân vật cho nút
            var index = i;
            b.clicked += () =>
            {
                CharIndex = index;
            };

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
        ShowPlayerName(selectedPlayerModel.ModelName);


    }
    [Header("background")]
   [SerializeField]  Material PlayerNamebackgroundMaterial;
    async void ShowPlayerName(string t)
    {
        
        PlayerNamebackgroundMaterial.SetInt("_isOff", 0);
        PlayerNamebackgroundMaterial.SetFloat("_Interpolate", 1.005f);
        await Task.Delay(50);

        PlayerNamebackgroundMaterial.SetFloat("_Interpolate", 0.995f);
        await Task.Delay(50);
        var TextMeshpro = CharShowCase.GetComponentsInChildren<TextMeshPro>();
        for (int i = 0; i < TextMeshpro.Length; i++)
        {
            TextMeshpro[i].text = t;
        }
        PlayerNamebackgroundMaterial.SetFloat("_Interpolate", 1.002f);
        await Task.Delay(50);
        PlayerNamebackgroundMaterial.SetFloat("_Interpolate", 0.998f);
        await Task.Delay(50);
        PlayerNamebackgroundMaterial.SetInt("_isOff", 1);
        PlayerNamebackgroundMaterial.SetFloat("_Interpolate", 1);

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
        //startScreenHandler.gameObject.SetActive(true);
        startScreenHandler.gameObject.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
        //Display(false);
        StartGameInfo.instance.playerData.playerChar = SelectedChar;
    }

    public void PlaySelectedAnimation(PlayerModelBase model)
    {
        model.WaitForStart_(() => model.SelectedAnim());
        DOTween.To(() => camera.m_Lens.FieldOfView, (x) => camera.m_Lens.FieldOfView = x, Camera.FocalLengthToFieldOfView(2.1f, 1), 1).SetEase(Ease.InOutQuart);
        DOTween.To(() => BackgroundCamera.m_Lens.FieldOfView, (x) => BackgroundCamera.m_Lens.FieldOfView = x, Camera.FocalLengthToFieldOfView(2.1f, 1), 1).SetEase(Ease.InOutQuart);
        //btn_SelectChar.interactable = false;
    }
    public void PlayUnSelectedAnimation(PlayerModelBase player)
    {
        player.WaitForStart_(() => player.IdleAnim());
        DOTween.To(() => camera.m_Lens.FieldOfView, (x) => camera.m_Lens.FieldOfView = x, Camera.FocalLengthToFieldOfView(1.6f, 1), 1).SetEase(Ease.InOutQuart);
        DOTween.To(() => BackgroundCamera.m_Lens.FieldOfView, (x) => BackgroundCamera.m_Lens.FieldOfView = x, Camera.FocalLengthToFieldOfView(1.6f, 1), 1).SetEase(Ease.InOutQuart);
        //btn_SelectChar.interactable = true;
    }
}