using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

//[DefaultExecutionOrder(0)]
public class UINew_LobbyScreen : MonoBehaviour
{
    private VisualElement root;
    private static VisualElement container;
    private static VisualElement background;
    private static VisualElement lobbyScreen;
    private static List<VisualElement> playerCard;
    private static VisualElement leaderIcon;
    private static VisualElement readyIcon;
    void Awake()
    {
        //Debug.LogError("Awake");
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        background = root.Q<VisualElement>("background");
        lobbyScreen = root.Q<VisualElement>("lobby-screen");
        leaderIcon = root.Q<VisualElement>("leader");
        readyIcon = root.Q<VisualElement>("ready");
        ResetStyle();
    }

    public static void ResetStyle()
    {
        playerCard = lobbyScreen.Query("player-card").ToList();
        for(int i = 0; i < playerCard.Count; i++)
        {
            playerCard[i].Q<VisualElement>("player-empty").style.display = DisplayStyle.Flex;
            playerCard[i].Q<VisualElement>("player-description").style.display = DisplayStyle.None;
            playerCard[i].Q<VisualElement>("notify").style.width = 0f;
        }

        container.style.display = DisplayStyle.None;
        //lobbyScreen.style.display = DisplayStyle.None;
        //background.style.scale = new Scale(new Vector2(12f, 12f));
        leaderIcon.style.display = DisplayStyle.None;
        readyIcon.style.display = DisplayStyle.None;
    }

    public static void Show()
    {
        container.style.display = DisplayStyle.Flex;
        //background.style.scale = new Scale(new Vector2(1f, 1f));
        //await Task.Delay(1000);
        //lobbyScreen.style.display = DisplayStyle.Flex;
    }
}