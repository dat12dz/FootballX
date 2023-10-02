using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyScreen : MonoBehaviour
{
    private VisualElement root;
    private static VisualElement container;
    private static VisualElement lobbyScreen;
    private static VisualElement background;
    private static VisualElement redTeam;
    private static VisualElement blueTeam;
    private static VisualElement exitBtn;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        background = root.Q<VisualElement>("background");
        lobbyScreen = root.Q<VisualElement>("lobby-screen");
        redTeam = root.Q<VisualElement>("red-team");
        blueTeam = root.Q<VisualElement>("blue-team");
        exitBtn = root.Q<VisualElement>("exit-btn");
        InitStyle();
    }

    private static void InitStyle()
    {
        container.style.display = DisplayStyle.None;
        lobbyScreen.style.display = DisplayStyle.None;
        background.style.scale = new Scale(new Vector2(12f, 12f));
    }

    public static async void Show()
    {
        container.style.display = DisplayStyle.Flex;
        background.style.scale = new Scale(new Vector2(1f, 1f));
        await Task.Delay(1000);
        lobbyScreen.style.display = DisplayStyle.Flex;
        exitBtn.RegisterCallback<PointerDownEvent>(callback =>
        {
            Debug.Log("click");
            InitStyle();
        });
    }
}
