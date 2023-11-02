
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIBase))]
public partial class UInew_Setting : MonoBehaviour
{
 
    class CustomTabsItem
    {
        public CustomTabsItem(VisualElement base__) {
            base_ = base__;
        }
        public void Display(bool a)
        {
            if (a)
            base_.style.display = DisplayStyle.Flex;
            else
                base_.style.display = DisplayStyle.None;
        }
        VisualElement base_;
    }
    class Panel_GraphicsTabsElement : CustomTabsItem
    {
       public SliderInt slider_graphicLevel;

        public Panel_GraphicsTabsElement(VisualElement base__) : base(base__)
        {
            

        }
        public void InitValue()
        {
            slider_graphicLevel.value = SettingDAO.Instance.graphicSetting.ChosenPreset;
        }
    }
    class Panel_GamplayTabsElement : CustomTabsItem
    {
        public FloatField txtbox_mouseSen;

        public Panel_GamplayTabsElement(VisualElement base__) : base(base__)
        {
        }
        public void InitValue()
        {
            txtbox_mouseSen.value = SettingDAO.Instance.gameplaySetting.MouseSen;
        }
    }
    class Panel_SoundTabsElement : CustomTabsItem
    {
        public Slider slider_volume;

        public Panel_SoundTabsElement(VisualElement base__) : base(base__)
        {
        }
        public void InitValue()
        {
            slider_volume.value = SettingDAO.Instance.SoundSetting.volume;
        }
    }

    public static UInew_Setting instance;
    
    [SerializeField] UIDocument document;
    List<CustomTabsItem> tabs = new();
    VisualElement container;
    Button lb_btn_Graphic, lb_btn_Gameplay, lb_btn_Sound;
    Button saveBtn, closeBtn;
    VisualElement settingBar;


    private void Start()
    {
        if(instance == null) 
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);

        var root = document.rootVisualElement;
        
        // Graphic tab
        var GraphicTab = new Panel_GraphicsTabsElement(root.Q<VisualElement>("panel_Graphic"));
        GraphicTab.slider_graphicLevel = root.Q<SliderInt>("slider_graphicPreset");
        GraphicTab.slider_graphicLevel.highValue = SettingDAO.Instance.graphicSetting.GraphicPresetArray.Length - 1;
        GraphicTab.slider_graphicLevel.RegisterValueChangedCallback(OnSlider_GrapicPresetChange);
        GraphicTab.InitValue();
        tabs.Add(GraphicTab);
        // Gameplay tab
        var GameplayTabs = new Panel_GamplayTabsElement(root.Q<VisualElement>("panel_Gameplay"));
        GameplayTabs.txtbox_mouseSen = root.Q<FloatField>("numbox_MouseSen");
        GameplayTabs.InitValue();
        tabs.Add(GameplayTabs);
        // Sound tab
        var SoundTab = new Panel_SoundTabsElement(root.Q<VisualElement>("panel_Sound"));
        SoundTab.slider_volume = root.Q<Slider>("Slider_volume");
        SoundTab.slider_volume.RegisterValueChangedCallback(OnSlider_VolumeChange);
        SoundTab.InitValue();
        tabs.Add(SoundTab);

        // Label
        container = root.Q<VisualElement>("container");
        lb_btn_Graphic = root.Q<Button>("lb_Graphic");
        lb_btn_Gameplay = root.Q<Button>("lb_Gamplay");
        lb_btn_Sound = root.Q<Button>("lb_Sound");
        settingBar = root.Q<VisualElement>("setting-bar");
        saveBtn = root.Q<Button>("save-btn");
        closeBtn = root.Q<Button>("close-btn");

        lb_btn_Graphic.clicked += () =>
        {
            lb_btn_Graphic_onClick();
        };

        lb_btn_Sound.clicked += () => 
        {

            lb_btn_Sound_onClick();
        };

        lb_btn_Gameplay.clicked += () =>
        {
            lb_btn_Gameplay_onClick();
        };

        closeBtn.clicked += () =>
        {
            InitStyle();
        };

        InitStyle();
    }

    void InitStyle()
    {
        container.style.opacity = 0;
        container.style.display = DisplayStyle.None;

        settingBar.style.transitionTimingFunction = new List<EasingFunction>()
        {
            new EasingFunction(EasingMode.EaseInCubic)
        };
        settingBar.style.translate = new Translate(new Length(-100f, LengthUnit.Percent), 0);
    }

    public async void Show()
    {
        container.style.display = DisplayStyle.Flex;
        container.style.opacity = 1;

        settingBar.style.transitionTimingFunction = new List<EasingFunction>()
        {
            new EasingFunction(EasingMode.EaseOutCubic)
        };
        settingBar.style.translate = new Translate(new Length(0, LengthUnit.Percent), 0);
        await Task.Delay(400);
    }

    public void OnSlider_GrapicPresetChange(ChangeEvent<int> ev)
    {
         SettingDAO.Instance.graphicSetting.SetGraphicPreset(ev.newValue);
        switch (ev.newValue)
        {
                case 0:
                   
                break;
                case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
    public void OnSlider_VolumeChange(ChangeEvent<float> ev)
    {
        SettingDAO.Instance.SoundSetting.ChangeVolume(ev.newValue);
    }
   public void Display(bool a)
    {
        gameObject.SetActive(a);
    }
    
    void HideAllTabs()
    {
        foreach (var tab in tabs)
        {
            tab.Display(false);
        }
    }
    void lb_btn_Graphic_onClick()
    {
        HideAllTabs();
        tabs[0].Display(true);
    }
    void lb_btn_Gameplay_onClick()
    {
        HideAllTabs();
        tabs[1].Display(true);
    }
    void lb_btn_Sound_onClick()
    {
        HideAllTabs();
        tabs[2].Display(true);
    }
}

