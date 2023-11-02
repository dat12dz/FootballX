using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIBase))]
public class UINew_PauseScreen : MonoBehaviour
{
    //public static UINew_PauseScreen instance;
    private VisualElement root;
    private static VisualElement container;
    private static VisualElement pauseScreen;
    private Button resumeBtn;
    private Button optionsBtn;
    private Button exitBtn;

    void Start()
    {
        //instance = this;
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        pauseScreen = root.Q<VisualElement>("pause-screen");
        resumeBtn = root.Q<Button>("resume-btn");
        optionsBtn = root.Q<Button>("options-btn");
        exitBtn = root.Q<Button>("exit-btn");

        InitStyle();

        resumeBtn.clicked += () =>
        {
            InitStyle();
        };

        optionsBtn.clicked += () =>
        {
            UInew_Setting.instance.Show();
        };

        exitBtn.clicked += () =>
        {
            NetworkManager.Singleton.Shutdown();
        };
    }

    public static async void Show()
    {
        container.style.display = DisplayStyle.Flex;
        pauseScreen.style.transitionTimingFunction = new List<EasingFunction>()
        {
            new EasingFunction(EasingMode.EaseOutCubic)
        };

        pauseScreen.style.borderTopWidth = 6f;
        pauseScreen.style.borderRightWidth = 6f;
        pauseScreen.style.borderBottomWidth = 6f;
        pauseScreen.style.borderLeftWidth = 6f;
        pauseScreen.style.height = new Length(320f, LengthUnit.Pixel);
        await Task.Delay(400);
    }

    async void InitStyle()
    {
        pauseScreen.style.height = 0;
        pauseScreen.style.borderTopWidth = 0;
        pauseScreen.style.borderRightWidth = 0;
        pauseScreen.style.borderBottomWidth = 0;
        pauseScreen.style.borderLeftWidth = 0;
        pauseScreen.style.transitionTimingFunction = new List<EasingFunction>()
        {
            new EasingFunction(EasingMode.EaseInCubic)
        };
        await Task.Delay(400);
        container.style.display = DisplayStyle.None;
    }
}
