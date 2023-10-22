using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UINew_ChangeSceneEffect : MonoBehaviour
{
    static UINew_ChangeSceneEffect instance;
    private VisualElement root;
    private static VisualElement container;
    private static VisualElement changeSceneEffect;
    private static VisualElement showAndHideBg;
    void Start()
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
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        changeSceneEffect = root.Q<VisualElement>("change-scene-effect");
        showAndHideBg = root.Q<VisualElement>("show-and-hide-bg");
        ResetStyle();
    }

    public static async void Open()
    {
        container.style.display = DisplayStyle.Flex;
        showAndHideBg.style.translate = new Translate(new Length(134, LengthUnit.Percent), new Length(-134, LengthUnit.Percent));

        TransitionInit.instance.StartRendering();
        changeSceneEffect.style.display = DisplayStyle.Flex;
        await Task.Delay(300);
        showAndHideBg.style.transitionDuration = new List<TimeValue>()
        {
            new TimeValue(0, TimeUnit.Millisecond)
        };
        showAndHideBg.style.translate = new Translate(new Length(0, LengthUnit.Percent), new Length(0, LengthUnit.Percent));
    }

    public static async void Close()
    {
        await Task.Delay(1000);

        showAndHideBg.style.transitionDuration = new List<TimeValue>()
        {
            new TimeValue(300, TimeUnit.Millisecond)
        };

        showAndHideBg.style.translate = new Translate(new Length(134, LengthUnit.Percent), new Length(-134, LengthUnit.Percent));

        TransitionInit.instance.StopRendering();
        changeSceneEffect.style.display = DisplayStyle.None;

        await Task.Delay(300);
        ResetStyle();
    }

    static void ResetStyle()
    {
        container.style.display = DisplayStyle.None;
        showAndHideBg.style.translate = new Translate(new Length(0, LengthUnit.Percent), new Length(0, LengthUnit.Percent));
        changeSceneEffect.style.display = DisplayStyle.None;
    }

    public static async void ChangeScene(int sceneIndex)
    {
        Open();
        await Task.Delay(300);
   
        SceneManager.LoadScene(sceneIndex);
    }
  
}
