using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UINew_MVPScreen : MonoBehaviour
{
    VisualElement root;
    static VisualElement container;
    static VisualElement mvpScreenTop;
    static VisualElement mvpScreenBottom;
    Label playerName;
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        mvpScreenTop = root.Q<VisualElement>("mvp-screen-top");
        mvpScreenBottom = root.Q<VisualElement>("mvp-screen-bottom");
        playerName = root.Q<Label>("player-name");

        ResetStyle();
    }

    public static void ResetStyle()
    {
        mvpScreenTop.style.translate = new Translate(0, new Length(-110, LengthUnit.Percent));
        mvpScreenBottom.style.translate = new Translate(0, new Length(110, LengthUnit.Percent));
    }

    public static async void Show()
    {
        mvpScreenTop.style.translate = new StyleTranslate(StyleKeyword.Null);
        //await Task.Delay(400);

        mvpScreenBottom.style.translate = new StyleTranslate(StyleKeyword.Null);
        await Task.Delay(400);
    }

}