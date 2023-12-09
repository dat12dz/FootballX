using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UINew_MVPScreen
{
    VisualElement container;
    public static VisualElement mvpScreen;
    VisualElement firstBg;
    VisualElement secondBg;
    VisualElement thirdBg;
    VisualElement fourthBg;
    VisualElement widthScale;
    VisualElement fifthBg;
    VisualElement sixthBg;
    VisualElement sixthBgBox;
    VisualElement seventhBg;
    VisualElement seventhBgBox;
    VisualElement pbl_MvpInfo;
    VisualElement pbl_MvpInfoBox;

    Label lb_MVP;
    Label lb_PlayerName;
    Button Btn_continue1;
    public UINew_MVPScreen(VisualElement root)
    {
        container = root.Q<VisualElement>("container");
        mvpScreen = root.Q<VisualElement>("mvp-screen");
        firstBg = root.Q<VisualElement>("first-bg");
        secondBg = root.Q<VisualElement>("second-bg");
        thirdBg = root.Q<VisualElement>("third-bg");
        fourthBg = root.Q<VisualElement>("fourth-bg");
        widthScale = root.Q<VisualElement>("width-scale");
        fifthBg = root.Q<VisualElement>("fifth-bg");
        sixthBg = root.Q<VisualElement>("sixth-bg");
        sixthBgBox = root.Q<VisualElement>("sixth-bg-box");
        seventhBg = root.Q<VisualElement>("seventh-bg");
        seventhBgBox = root.Q<VisualElement>("seventh-bg-box");
        pbl_MvpInfo = root.Q<VisualElement>("pbl_MvpInfo");
        pbl_MvpInfoBox = root.Q<VisualElement>("pbl_MvpInfo-box");
        Btn_continue1 = root.Q<Button>("Btn_continue1");
        lb_MVP = root.Q<Label>("lb_MVP");
        lb_PlayerName = root.Q<Label>("lb_PlayerName");

        ResetStyle();
    }

    public void ResetStyle()
    {
        mvpScreen.style.height = new Length(580, LengthUnit.Pixel);

        firstBg.style.opacity = 0;

        secondBg.style.opacity = 0;

        thirdBg.style.opacity = 0;
        thirdBg.style.unityBackgroundImageTintColor = new StyleColor(new Color(0,0,0));

        fourthBg.style.translate = new Translate(new Length(-120, LengthUnit.Percent), 0);
        fourthBg.style.backgroundColor = new Color(1,1,1);
        fourthBg.style.transitionDuration = new List<TimeValue>()
        {
            new TimeValue(400f, TimeUnit.Millisecond)
        };
        widthScale.style.width = new Length(120, LengthUnit.Pixel);

        fifthBg.style.width = new Length(0, LengthUnit.Pixel);

        sixthBg.style.display = DisplayStyle.None;
        sixthBg.style.backgroundColor = new StyleColor(StyleKeyword.None);
        sixthBg.style.scale = new Scale(new Vector2(1.2f, 1.2f));
        sixthBgBox.style.unityBackgroundImageTintColor = new Color(1,1,1);
        sixthBgBox.style.opacity = 1;

        seventhBg.style.display = DisplayStyle.Flex;
        seventhBgBox.style.left = new Length(-26f, LengthUnit.Percent);

        pbl_MvpInfo.style.display = DisplayStyle.None;
        pbl_MvpInfoBox.style.opacity = 0;
        lb_PlayerName.style.opacity= 0;
        lb_MVP.style.translate = new Translate(new Length(-160f, LengthUnit.Percent), 0);

        Btn_continue1.style.right = new Length(-20f, LengthUnit.Percent);
    }

    public async void Show()
    {
        firstBg.style.opacity = 1;
        secondBg.style.opacity = 1;
        thirdBg.style.opacity = 1;

        thirdBg.style.unityBackgroundImageTintColor = new StyleColor(new Color(0, 190f / 255, 240f / 255));

        fourthBg.style.translate = new Translate(0, 0);
        widthScale.style.width = new Length(0, LengthUnit.Pixel);
        fifthBg.style.width = new Length(1200, LengthUnit.Pixel);
        seventhBgBox.style.left = new Length(6f, LengthUnit.Percent);
        await Task.Delay(400);

        //await Task.Delay(100);
        mvpScreen.style.height = new Length(400, LengthUnit.Pixel);

        fourthBg.style.width = new Length(940, LengthUnit.Pixel);
        await Task.Delay(400);

        fourthBg.style.transitionDuration = new List<TimeValue>()
        {
            new TimeValue(150f, TimeUnit.Millisecond)
        };
        fourthBg.style.width = new Length(1200, LengthUnit.Pixel);
        seventhBg.style.display = DisplayStyle.None;
        await Task.Delay(150);

        fourthBg.style.backgroundColor = new Color(0, 0, 0, 0);

        sixthBg.style.display = DisplayStyle.Flex;
        sixthBg.style.scale = new Scale(new Vector2(1f, 1f));
        pbl_MvpInfo.style.display = DisplayStyle.Flex;
        lb_MVP.style.translate = new Translate(new Length(0, LengthUnit.Percent), 0);
        Btn_continue1.style.right = new Length(20f, LengthUnit.Percent);
        await Task.Delay(200);

        sixthBgBox.style.unityBackgroundImageTintColor = new Color(0, 0, 0);
        sixthBg.style.backgroundColor = new Color(1, 1, 1);
        await Task.Delay(200);

        //mvpScreen.style.backgroundColor = new Color(1, 1, 1, 1);
        pbl_MvpInfoBox.style.opacity = 1;
        lb_PlayerName.style.opacity = 1;
        sixthBgBox.style.opacity = 0;
    }
}