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
    [SerializeField] Button btn_ChangeCharBase,btn_SelectChar;
    [SerializeField] float Spacing = 10;
    [SerializeField] Transform SpawnPosition;
    [SerializeField] Transform CharShowCase;
    [SerializeField] CinemachineVirtualCamera camera;
    int CharIndex_ = -1;
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
    }
    public void Display(bool a)
    {
        transform.parent.gameObject.SetActive(a);
        
    }
   List<PlayerModel> models = new List<PlayerModel>();
    PlayerModel oldModel;
    void SpawnAllButton()
    {
       var SpawnInitialLocaltion = btn_ChangeCharBase.GetComponent<RectTransform>();
        for (int i = 0; i < AllChar.CharArray.Length; i++)
        {
         
            PlayerModel modelinfo = AllChar.CharArray[i];
            if (modelinfo == null) continue;
            var newButton =  Instantiate(btn_ChangeCharBase,transform);
            RectTransform buttonTransform = newButton.GetComponent<RectTransform>();    
            newButton.gameObject.SetActive(true);

            PlayerModel newModelInfo = Instantiate(modelinfo, SpawnPosition.position,SpawnPosition.rotation, CharShowCase)  ;
             var AfterLoadThumbnail = newModelInfo.Thumbnail.LoadAssetAsync();
            AfterLoadThumbnail.Completed += (sprite) => {
                newButton.GetComponent<Image>().sprite = sprite.Result;
            };
            
            float newPositionX = SpawnInitialLocaltion.position.x + i * (Spacing + buttonTransform.localScale.x);
            buttonTransform.position = new Vector3(newPositionX, buttonTransform.position.y);
            newButton.onClick.AddListener(() =>
            {
                CharIndex = i;
            });
            models.Add(newModelInfo);
            newModelInfo.gameObject.SetActive(false);
        }
    }
    void ShowChar(int index)
    {
        if (CharIndex_ == index) return;
       PlayerModel  selectedPlayerModel = models[index];
        selectedPlayerModel.gameObject.SetActive(true);
        if (oldModel)
        oldModel.gameObject.SetActive(false);
        oldModel = selectedPlayerModel;

        if (SelectedChar == index)
        {
            PlaySelectedAnimation(selectedPlayerModel);
        }
        oldModel = selectedPlayerModel;
    }
    public void Btn_SelectCharAction()
    {
        SelectedChar = CharIndex;
    }
    
    public void PlaySelectedAnimation(PlayerModel model)
    {
        model.SelectedAnim();
        DOTween.To(() => camera.m_Lens.FieldOfView, (x) => camera.m_Lens.FieldOfView = x, 2.4f, 2).SetEase(Ease.InOutQuart);
    }
    public void PlayUnSelectedAnimation(PlayerModel player)
    {
        player.SelectedAnim();
        DOTween.To(() => camera.m_Lens.FieldOfView, (x) => camera.m_Lens.FieldOfView = x, 1.8f, 2).SetEase(Ease.InOutQuart);

    }

    void Update()
    {
    
    }
}
