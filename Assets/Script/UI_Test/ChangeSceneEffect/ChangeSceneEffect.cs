using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
public class ChangeSceneEffect : MonoBehaviour
{
    private VisualElement root;
    private static VisualElement container;
    private static VisualElement changeSceneEffect;
    private static VisualElement showAndHideBg;
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        changeSceneEffect = root.Q<VisualElement>("change-scene-effect");
        showAndHideBg = root.Q<VisualElement>("show-and-hide-bg");
        ResetStyle();
    }

    public static async void Show()
    {
        container.style.display = DisplayStyle.Flex;

        showAndHideBg.style.translate = new Translate(new Length(134, LengthUnit.Percent), new Length(-134, LengthUnit.Percent));
        
        changeSceneEffect.style.backgroundColor = Color.black;
        await Task.Delay(300);

        
        showAndHideBg.style.transitionDuration = new List<TimeValue>()
        {
            new TimeValue(0, TimeUnit.Millisecond)
        };
        showAndHideBg.style.translate = new Translate(new Length(0, LengthUnit.Percent), new Length(0, LengthUnit.Percent));

        await Task.Delay(1000);
        showAndHideBg.style.transitionDuration = new List<TimeValue>()
        {
            new TimeValue(300, TimeUnit.Millisecond)
        };
        showAndHideBg.style.translate = new Translate(new Length(134, LengthUnit.Percent), new Length(-134, LengthUnit.Percent));

        changeSceneEffect.style.backgroundColor = Color.clear;
        await Task.Delay(1000);
        ResetStyle();
    }

    static void ResetStyle()
    {
        container.style.display = DisplayStyle.None;
        showAndHideBg.style.translate = new Translate(new Length(0, LengthUnit.Percent), new Length(0, LengthUnit.Percent));
    }
}
