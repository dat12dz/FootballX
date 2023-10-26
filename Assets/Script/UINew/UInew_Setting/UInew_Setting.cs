
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

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
    [SerializeField] UIDocument document;
    List<CustomTabsItem> tabs = new();
    Label lb_btn_Graphic, lb_btn_Gameplay, lb_btn_Sound;

    private void Start()
    {

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
        lb_btn_Graphic = root.Q<Label>("lb_Graphic");
        lb_btn_Gameplay = root.Q<Label>("lb_Gamplay");
        lb_btn_Sound = root.Q<Label>("lb_Sound");
        lb_btn_Graphic.RegisterCallback<PointerDownEvent>(lb_btn_Graphic_onClick);
        lb_btn_Sound.RegisterCallback<PointerDownEvent>(lb_btn_Sound_onClick);
        lb_btn_Gameplay.RegisterCallback<PointerDownEvent>(lb_btn_Gameplay_onClick);

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
    void lb_btn_Graphic_onClick(PointerDownEvent t)
    {
        HideAllTabs();
        tabs[0].Display(true);
    }
    void lb_btn_Gameplay_onClick(PointerDownEvent t)
    {
        HideAllTabs();
        tabs[1].Display(true);
    }
    void lb_btn_Sound_onClick(PointerDownEvent t)
    {
        HideAllTabs();
        tabs[2].Display(true);
    }
}

