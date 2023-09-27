using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyScreen : MonoBehaviour
{
    private VisualElement root;
    private VisualElement container;
    private VisualElement redTeam;
    private VisualElement blueTeam;

    private static VisualElement emptyPlayer;

    private static VisualElement ownerPlayer;

    private static VisualElement player;
    private static VisualElement playerAvatar;
    private static Label playerName;
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        redTeam = root.Q<VisualElement>("red-team");
        blueTeam = root.Q<VisualElement>("blue-team");

    }

    static void CreateEmptyPlayer()
    { 
    }

    static void CreatePlayer()
    {
        playerAvatar.AddToClassList("player-avatar");
        playerName.AddToClassList("player-name");
        player.Add(playerAvatar);
        player.Add(playerName);
        player.AddToClassList("player-info");
    }

    static void CreateOwnerPlayer()
    {

    }

    public static void Show()
    {

    }
}
