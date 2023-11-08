using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UINew_MVPScreen : MonoBehaviour
{
    VisualElement root;
    static VisualElement container;
    static VisualElement mvpScreen;
    static VisualElement firstBg;
    static VisualElement secondBg;
    static VisualElement thirdBg;
    static VisualElement fourthBg;
    static VisualElement widthScale;
    static VisualElement fifthBg;

    Label playerName;
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        mvpScreen = root.Q<VisualElement>("mvp-screen");
        firstBg = root.Q<VisualElement>("first-bg");
        secondBg = root.Q<VisualElement>("second-bg");
        thirdBg = root.Q<VisualElement>("third-bg");
        fourthBg = root.Q<VisualElement>("fourth-bg");
        widthScale = root.Q<VisualElement>("width-scale");
        fifthBg = root.Q<VisualElement>("fifth-bg");

        ResetStyle();
    }

    public static void ResetStyle()
    {
        mvpScreen.style.height = new Length(580, LengthUnit.Pixel);

        firstBg.style.opacity = 0;
        secondBg.style.opacity = 0;
        thirdBg.style.opacity = 0;
        thirdBg.style.unityBackgroundImageTintColor = new StyleColor(new Color(0,0,0));

        fourthBg.style.translate = new Translate(new Length(-120, LengthUnit.Percent), 0);
        widthScale.style.width = new Length(120, LengthUnit.Pixel);
        fifthBg.style.width = new Length(0, LengthUnit.Pixel);
    }

    public static async void Show()
    {
        firstBg.style.opacity = 1;
        secondBg.style.opacity = 1;
        thirdBg.style.opacity = 1;

        thirdBg.style.unityBackgroundImageTintColor = new StyleColor(new Color(0, 190f / 255, 240f / 255));

        fourthBg.style.translate = new Translate(0, 0);
        widthScale.style.width = new Length(0, LengthUnit.Pixel);
        fifthBg.style.width = new Length(1200, LengthUnit.Pixel);
        await Task.Delay(400);

        //await Task.Delay(100);
        mvpScreen.style.height = new Length(400, LengthUnit.Pixel);

        fourthBg.style.width = new Length(800, LengthUnit.Pixel);
        await Task.Delay(400);
        fourthBg.style.width = new Length(1200, LengthUnit.Pixel);

        //fifthBg.style.width = new Length(0);
    }

}