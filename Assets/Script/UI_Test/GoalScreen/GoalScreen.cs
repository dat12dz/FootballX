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
    static GoalScreen instance;
    static VisualElement root;
    static VisualElement container;
    static VisualElement goalScreenScale;
    static VisualElement goalScreen;
    static VisualElement goalScreenClose;
    static VisualElement goalScreenBackground;
    static VisualElement animationIconAndText;
    static VisualElement redHide;
    static VisualElement goalScreenBox;
    static VisualElement goalScreenTrail;

    void Start()
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
        DontDestroyOnLoad(gameObject);

        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        goalScreenScale = root.Q<VisualElement>("goal-screen-scale");
        goalScreen = root.Q<VisualElement>("goal-screen");
        goalScreenClose = root.Q<VisualElement>("goal-screen-close");
        goalScreenBox = root.Q<VisualElement>("goal-screen-box");
        goalScreenBackground = root.Q<VisualElement>("goal-screen-background");
        animationIconAndText = root.Q<VisualElement>("animation-icon-and-text");
        redHide = root.Q<VisualElement>("red-hide");
        goalScreenTrail = root.Q<VisualElement>("goal-screen-trail");

        InitStyle();
    }

    public static async void EnableGoalScreen()
    {
        
        container.style.display = DisplayStyle.Flex;

        goalScreen.style.opacity = 1;
        await Task.Delay(50);

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
        await Task.Delay(1000);

        goalScreenClose.style.width = 0;
        await Task.Delay(200);
        ResetGoalScreen();
    }

    static void ResetGoalScreen()
    {
        container.style.display = DisplayStyle.None;
        goalScreenScale.style.width = new Length(600, LengthUnit.Pixel);
        goalScreenClose.style.width = new Length(100, LengthUnit.Percent);
        //goalScreenClose.style.width = new Length(60, LengthUnit.Percent);

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
        container.style.display = DisplayStyle.None;
        goalScreen.style.opacity = 0;
        animationIconAndText.style.opacity = 0;
    }
}