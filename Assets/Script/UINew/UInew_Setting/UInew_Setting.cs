using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UINew;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIBase))]
public partial class UInew_Setting : MonoBehaviour
{
    public static UInew_Setting instance;

    [SerializeField] UIDocument document;
    static VisualElement root;
    VisualElement container;
    VisualElement tabName;
    VisualElement settingPanel;
    Button saveBtn, closeBtn;
    VisualElement settingBar;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);

        root = document.rootVisualElement;
        container = root.Q<VisualElement>("container");
        settingBar = root.Q<VisualElement>("setting-bar");
        tabName = root.Q<VisualElement>("tab-name");
        settingPanel = root.Q<VisualElement>("setting-panel");
        saveBtn = root.Q<Button>("save-btn");
        closeBtn = root.Q<Button>("close-btn");

        GenerateUI(SettingDAO.Instance.graphicSetting);
        GenerateUI(SettingDAO.Instance.SoundSetting);
        GenerateUI(SettingDAO.Instance.gameplaySetting);

        // Handle btn in tab-name isClick
        var tabBtns = tabName.Children();
        var panelChildren = settingPanel.Children();
        foreach(var visualElement in tabBtns)
        {
            if(visualElement is Button btn)
            {
                string typeName = btn.name.Split('-')[0]; // {typeName}-btn: btn.name.Split('-')[0] = {typeName}, btn.name.Split('-')[1] = btn
                btn.clicked += () =>
                {
                    // Gán display = none cho các panel con
                    foreach(var panelChild in panelChildren)
                    {
                        panelChild.style.display = DisplayStyle.None;
                    }

                    // Hiển thị panel tương ứng khi click vào btn
                    VisualElement vE = root.Query($"{typeName}-panel");
                    vE.style.display = DisplayStyle.Flex;
                };
            }
        }

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

    public static void GenerateUI<T>(T ui) where T : SettingDAO.ISaveableContent
    {
        var type = typeof(T);        
        string typeName = type.Name;

        // Create tab
        VisualElement tabName = root.Q<VisualElement>("tab-name");
        Button btn = new Button();
        btn.name = $"{typeName}-btn"; // {typeName}-btn
        btn.AddToClassList("setting-btn");
        btn.text = typeName;
        tabName.Add(btn);

        // Create panel
        VisualElement settingPanel = root.Q<VisualElement>("setting-panel");
        VisualElement panel = new VisualElement();
        panel.AddToClassList("panel");
        panel.name = $"{typeName}-panel"; // {typeName}-panel
        var fields = type.GetFields();
        try
        {
            foreach (var f in fields)
            {
                ShowOnSettingAttribute attr = f.GetCustomAttribute<ShowOnSettingAttribute>();

                

                switch (attr.BindingType)
                {
                    case ShowOnSettingAttribute.SettingBindingType.SliderInt:
                        {
                            SliderInt slider = new SliderInt();
                            slider.label = attr.Name;
                            slider.lowValue = attr.MinValue;
                            slider.highValue = attr.MaxValue;
                            VariableHelper.CheckVariableChange(() => f.GetValue(ui), (obj) =>
                            {
                                MainThreadDispatcher.ExecuteInMainThread(() =>
                                {
                                    slider.value = (int)f.GetValue(ui);
                                    Debug.Log(obj.ToString());
                                });
                            });
                            slider.RegisterCallback<ChangeEvent<int>>((evt) =>
                            {
                                try
                                {
                                    f.SetValue(ui,evt.newValue);
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogException(ex);
                                    Debug.LogError("\n Kiểu dữ liệu không hợp lệ");
                                }
                            });
                            slider.AddToClassList("child-panel");
                            panel.Add(slider);
                            break;
                        }

                    case ShowOnSettingAttribute.SettingBindingType.SliderFloat:
                        {
                            Slider slider = new Slider();
                            slider.label = attr.Name;
                            slider.lowValue = attr.MinValue;
                            slider.highValue = attr.MaxValue;
                            VariableHelper.CheckVariableChange(() => f.GetValue(ui), (obj) =>
                            {
                                MainThreadDispatcher.ExecuteInMainThread(() =>
                                {
                                    slider.value = (float)f.GetValue(ui);
                                    Debug.Log(obj.ToString());
                                });
                            });
                            slider.RegisterCallback<ChangeEvent<float>>((evt) =>
                            {
                                try
                                {
                                    f.SetValue(ui, evt.newValue);
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogException(ex);
                                    Debug.LogError("\n Kiểu dữ liệu không hợp lệ");
                                }
                            });
                            slider.AddToClassList("child-panel");
                            panel.Add(slider);
                            break;
                        }

                    case ShowOnSettingAttribute.SettingBindingType.TextField:
                        {
                            TextField textField = new TextField();
                            textField.label = attr.Name;
                            VariableHelper.CheckVariableChange(() => f.GetValue(ui), (obj) =>
                            {
                                MainThreadDispatcher.ExecuteInMainThread(() =>
                                {
                                    textField.value = (string)f.GetValue(ui);
                                    Debug.Log(obj.ToString());
                                });
                            });
                            textField.AddToClassList("child-panel");
                            textField.RegisterCallback<ChangeEvent<string>>((evt) =>
                            {
                                try
                                {
                                    f.SetValue(ui, evt.newValue);
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogException(ex);
                                    Debug.LogError("\n Kiểu dữ liệu không hợp lệ");
                                }
                            });
                            panel.Add(textField);
                            break;
                        }

                    case ShowOnSettingAttribute.SettingBindingType.IntegerField:
                        {
                            IntegerField intField = new IntegerField();
                            intField.label = attr.Name;
                            VariableHelper.CheckVariableChange(() => f.GetValue(ui), (obj) =>
                            {
                                MainThreadDispatcher.ExecuteInMainThread(() =>
                                {
                                    intField.value = (int)f.GetValue(ui);
                                    Debug.Log(obj.ToString());
                                });
                            });
                            intField.AddToClassList("child-panel");
                            intField.RegisterCallback<ChangeEvent<int>>((evt) =>
                            {
                                try
                                {
                                    f.SetValue(ui, evt.newValue);
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogException(ex);
                                    Debug.LogError("\n Kiểu dữ liệu không hợp lệ");
                                }
                            });
                            panel.Add(intField);
                            break;
                        }

                    case ShowOnSettingAttribute.SettingBindingType.FloatField:
                        {
                            FloatField floatField = new FloatField();
                            floatField.label = attr.Name;
                            VariableHelper.CheckVariableChange(() => f.GetValue(ui), (obj) =>
                            {
                                MainThreadDispatcher.ExecuteInMainThread(() =>
                                {
                                    floatField.value = (float)f.GetValue(ui);                                    
                                    Debug.Log(obj.ToString());
                                });
                            });
                            floatField.AddToClassList("child-panel");
                            floatField.RegisterCallback<ChangeEvent<float>>((evt) =>
                            {
                                try
                                {
                                    f.SetValue(ui, evt.newValue);
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogException(ex);
                                    Debug.LogError("\n Kiểu dữ liệu không hợp lệ");
                                }
                            });
                            panel.Add(floatField);
                            break;
                        }
                    case ShowOnSettingAttribute.SettingBindingType.Toggle:
                        {
                            Toggle toggleField = new Toggle();
                            toggleField.label = attr.Name;
                            VariableHelper.CheckVariableChange(() => f.GetValue(ui), (obj) =>
                            {
                                MainThreadDispatcher.ExecuteInMainThread(() =>
                                {
                                    toggleField.value = (bool)f.GetValue(ui);                                    
                                    Debug.Log(obj.ToString());
                                });
                            });
                            toggleField.AddToClassList("child-panel");
                            toggleField.RegisterCallback<ChangeEvent<bool>>((evt) =>
                            {
                                try
                                {
                                    f.SetValue(ui, evt.newValue);
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogException(ex);
                                    Debug.LogError("\n Kiểu dữ liệu không hợp lệ");
                                }
                            });
                            panel.Add(toggleField);
                            break;
                        }
                }
            }
        }
        catch (Exception ex)
        {

        }
        settingPanel.Add(panel);
    }        
}