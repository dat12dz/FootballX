using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public class GoalScreen : MonoBehaviour
{
    public static GoalScreen instance;
    static VisualElement root;
    static VisualElement container;
    static VisualElement goalScreenScale;
    static VisualElement goalScreen;
    static VisualElement goalScreenClose;
    static VisualElement goalScreenBackground;
    static VisualElement animationIconAndText;
    static VisualElement redHide;
    static VisualElement ballIcon;
    static VisualElement goalScreenBox;
    static VisualElement goalScreenTrail;
    static VisualElement goalScreenTextInfo;
    static Label goalScreenText;
    static Label goalScreenTitle;
    static Label goalScreenDescription;

    public enum ColorName
    {
        red_black,
        blue_white,
        black_white
    }
    void  Start()
    {
        instance = this;
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        goalScreen = root.Q<VisualElement>("goal-screen");
        goalScreenScale = root.Q<VisualElement>("goal-screen-scale");
        goalScreenClose = root.Q<VisualElement>("goal-screen-close");
        goalScreenBox = root.Q<VisualElement>("goal-screen-box");
        goalScreenBackground = root.Q<VisualElement>("goal-screen-background");
        animationIconAndText = root.Q<VisualElement>("animation-icon-and-text");
        redHide = root.Q<VisualElement>("red-hide");
        ballIcon = root.Q<VisualElement>("ball-icon");
        goalScreenTrail = root.Q<VisualElement>("goal-screen-trail");
        goalScreenTextInfo = root.Q<VisualElement>("goal-screen-text-info");
        goalScreenText = root.Q<Label>("goal-screen-text");
        goalScreenTitle = root.Q<Label>("goal-screen-title");
        goalScreenDescription = root.Q<Label>("goal-screen-description");
        InitStyle();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="second"></param>
    /// <param name="color">0 is red-black, 1 is blue-white, 2 is black-white</param>
    static async void EnableGoalScreen(int second, ColorName color = 0)
    {
        await VariableHelper.WaitForVariableNotNullAsync(() => container, 50000);
        await VariableHelper.WaitForVariableNotNullAsync(() => goalScreen, 50000);
        await VariableHelper.WaitForVariableNotNullAsync(() => goalScreenBackground, 50000);
        await VariableHelper.WaitForVariableNotNullAsync(() => goalScreenScale, 50000);
        switch (color)
        {
            case ColorName.red_black:
                {
                    goalScreen.style.backgroundColor = Color.red;
                    goalScreenBackground.style.backgroundColor = Color.black;
                    goalScreenTextInfo.style.color = Color.white;
                    ballIcon.style.unityBackgroundImageTintColor = Color.white;
                    break;
                }
                case ColorName.blue_white:
                { 
                    goalScreen.style.backgroundColor = Color.blue;
                    goalScreenBackground.style.backgroundColor = Color.white;
                    goalScreenTextInfo.style.color = Color.black;
                    ballIcon.style.unityBackgroundImageTintColor = Color.black;
                    break;  
                }
                case ColorName.black_white:
                {
                    goalScreen.style.backgroundColor = Color.black;
                    goalScreenBackground.style.backgroundColor = Color.white;
                    goalScreenTextInfo.style.color = Color.black;
                    ballIcon.style.unityBackgroundImageTintColor = Color.black;
                    break;
                }
        }
        

        container.style.display = DisplayStyle.Flex;

        goalScreen.style.opacity = 1;
        goalScreen.style.opacity = 0;
        await Task.Delay(50);

        goalScreen.style.opacity = 1;
        await Task.Delay(50);

        goalScreen.style.opacity = 0;
        await Task.Delay(50);

        goalScreen.style.opacity = 1;
        goalScreen.style.transitionDuration = new List<TimeValue>()
        {
            new TimeValue(100, TimeUnit.Millisecond)
        };

        goalScreenScale.style.width = new Length(560, LengthUnit.Pixel);
        await Task.Delay(130);

        animationIconAndText.style.translate = new Translate(0, 0);
        animationIconAndText.style.opacity = 1;
        await Task.Delay(20);

        redHide.style.height = 0;
        redHide.style.display = DisplayStyle.Flex;

        goalScreenBackground.style.display = DisplayStyle.Flex;
        await Task.Delay(second * 1000);

        goalScreenClose.style.width = 0;
        await Task.Delay(200);
        ResetGoalScreen();
    }

    static async void ResetGoalScreen()
    {
        await VariableHelper.WaitForVariableNotNullAsync(() => container, 50000);
        container.style.display = DisplayStyle.None;

        goalScreenScale.style.width = new Length(600, LengthUnit.Pixel);
        goalScreenClose.style.width = new Length(100, LengthUnit.Percent);
        //goalScreenClose.style.width = new Length(60, LengthUnit.Percent);

        await VariableHelper.WaitForVariableNotNullAsync(() => goalScreen, 50000);
        goalScreen.style.opacity = 0;
        goalScreen.style.transitionDuration = null;

        goalScreenBackground.style.display = DisplayStyle.None;

        animationIconAndText.style.translate = new Translate(new Length(-5, LengthUnit.Percent), 0);
        animationIconAndText.style.opacity = 0;

        redHide.style.height = new Length(100, LengthUnit.Percent);
        redHide.style.display = DisplayStyle.None;
    }

    void InitStyle()
    {
        VariableHelper.TrackForVariableNotNull(() => container, () => container.style.display = DisplayStyle.None);
        VariableHelper.TrackForVariableNotNull(() => goalScreen, () => goalScreen.style.opacity = 0);
        //container.style.display = DisplayStyle.None;
        //goalScreen.style.opacity = 0;
        animationIconAndText.style.opacity = 0;
    }

    public static void Show(string text, string title, int second, ColorName color = 0)
    {
        VariableHelper.TrackForVariableNotNull(() => goalScreenText, () => goalScreenText.text = text);
        VariableHelper.TrackForVariableNotNull(() => goalScreenTitle, () => goalScreenTitle.text = title);
        //goalScreenText.text = text;
        //goalScreenTitle.text = title;
        EnableGoalScreen(second, color);
    }
}