using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[DefaultExecutionOrder(0)]
public class LobbyScreen : MonoBehaviour
{
    private VisualElement root;
    private static VisualElement container;
    private static VisualElement background;
    private static VisualElement lobbyScreen;
    private static VisualElement exitBtn;
    private static List<VisualElement> playerCard;
    private static VisualElement leaderIcon;
    private static VisualElement readyIcon;
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        background = root.Q<VisualElement>("background");
        lobbyScreen = root.Q<VisualElement>("lobby-screen");
        exitBtn = root.Q<VisualElement>("exit-btn");
        leaderIcon = root.Q<VisualElement>("leader");
        readyIcon = root.Q<VisualElement>("ready");
        InitStyle();
    }

    private static void InitStyle()
    {
        playerCard = lobbyScreen.Query("player-card").ToList();
        foreach(var p in playerCard)
        {
            p.Q<VisualElement>("player-empty").style.display = DisplayStyle.Flex;
            p.Q<VisualElement>("player-description").style.display = DisplayStyle.None;
            p.Q<VisualElement>("notify").style.display = DisplayStyle.None;
            p.Q<VisualElement>("swap-btn").style.display = DisplayStyle.None;
            p.Q<VisualElement>("interact-leader").style.display = DisplayStyle.None;
        }

        container.style.display = DisplayStyle.None;
        lobbyScreen.style.display = DisplayStyle.None;
        background.style.scale = new Scale(new Vector2(12f, 12f));
        leaderIcon.style.display = DisplayStyle.None;
        readyIcon.style.display = DisplayStyle.None;
    }

    public static async void Show()
    {
        container.style.display = DisplayStyle.Flex;
        background.style.scale = new Scale(new Vector2(1f, 1f));
        await Task.Delay(1000);
        lobbyScreen.style.display = DisplayStyle.Flex;
        ClickHandle();
    }
    static void ClickHandle()
    {
        // Exit LobbyScreen
        exitBtn.RegisterCallback<PointerDownEvent>(callback =>
        {
            InitStyle();
        });
    }
}