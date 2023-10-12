using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;


public class UI_CharacterSelection : MonoBehaviour
{
    [SerializeField] AllCharectorAssetReference AllChar;
    [SerializeField] Button btn_ChangeCharBase, btn_SelectChar;
    [SerializeField] Button btn_ChangeTeamBlue, btn_ChangeTeamRed;
    [SerializeField] float Spacing = 10;
    [SerializeField] Transform SpawnPosition;
    [SerializeField] Transform CharShowCase;
    [SerializeField] CinemachineVirtualCamera camera;

    public enum TeamColorEnum
    {
        red,blue
    }
    public TeamColorEnum team;
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
    int SelectedChar;
    void Start()
    {

        SpawnAllButton();
        CharIndex = 0;

        btn_SelectChar.onClick.AddListener(Btn_SelectCharAction);
        btn_ChangeTeamBlue.onClick.AddListener(Btn_ChangeColorBlue);
        btn_ChangeTeamRed.onClick.AddListener(Btn_ChangeColorRed);
    }
    public void Display(bool a)
    {
        transform.parent.gameObject.SetActive(a);

    }
    List<IPlayerModel> models = new List<IPlayerModel>();
    IPlayerModel oldModel;
    void SpawnAllButton()
    {
        var SpawnInitialLocaltion = btn_ChangeCharBase.GetComponent<RectTransform>();
        for (int i = 0; i < AllChar.CharArray.Length; i++)
        {

            IPlayerModel modelinfo = AllChar.CharArray[i];
            if (modelinfo == null) continue;
            var newButton = Instantiate(btn_ChangeCharBase, transform);
            RectTransform buttonTransform = newButton.GetComponent<RectTransform>();
            newButton.gameObject.SetActive(true);

            IPlayerModel newModelInfo = Instantiate(modelinfo, SpawnPosition.position, SpawnPosition.rotation, CharShowCase);

            Texture2D thumb = newModelInfo.Thumbnail;
            newButton.GetComponent<Image>().sprite = Sprite.Create(thumb ,new Rect(0,0, thumb.width,thumb.height),Vector2.zero);
        

            float newPositionX = SpawnInitialLocaltion.position.x + i * (Spacing + buttonTransform.rect.width);
            buttonTransform.position = new Vector3(newPositionX, buttonTransform.position.y);
            var index = i;
            newButton.onClick.AddListener(() =>
            {
                CharIndex = index;
            });
            models.Add(newModelInfo);
            newModelInfo.gameObject.SetActive(false);
        }
    }
    void ShowChar(int index)
    {
        // if (CharIndex == index) return;
        IPlayerModel selectedPlayerModel = models[index];
        if (oldModel)
            oldModel.gameObject.SetActive(false);
        selectedPlayerModel.gameObject.SetActive(true);
        oldModel = selectedPlayerModel;

        if (SelectedChar == index)
        {
            PlaySelectedAnimation(selectedPlayerModel);
        }
        else
        {
            PlayUnSelectedAnimation(selectedPlayerModel);
        }
        oldModel = selectedPlayerModel;
        if (team == TeamColorEnum.blue)
        {
            selectedPlayerModel.BlueTeamInit();
           
        }    
        else
        {
            selectedPlayerModel.RedTeamInit();
        }
        camera.LookAt = selectedPlayerModel.head;
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
    public void PlaySelectedAnimation(IPlayerModel model)
    {
        model.SelectedAnim();
        DOTween.To(() => camera.m_Lens.FieldOfView, (x) => camera.m_Lens.FieldOfView = x, Camera.FocalLengthToFieldOfView(2.1f, 1), 1).SetEase(Ease.InOutQuart);
        btn_SelectChar.interactable = false;
    }
    public void PlayUnSelectedAnimation(IPlayerModel player)
    {
        player.IdleAnim();
        DOTween.To(() => camera.m_Lens.FieldOfView, (x) => camera.m_Lens.FieldOfView = x, Camera.FocalLengthToFieldOfView(1.6f,1) , 1).SetEase(Ease.InOutQuart);
        btn_SelectChar.interactable = true;
    }

    void Update()
    {
        
    }
}
