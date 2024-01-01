using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class UINew_InGameOverlay : MonoBehaviour
{
    private VisualElement root;
    private VisualElement container;
    private VisualElement inGameOverlay;
    private VisualElement labelBar;
    private VisualElement blueTeam;
    private VisualElement redTeam;
    private VisualElement playerInfoTemplate;
    private bool isShow = false;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        inGameOverlay = root.Q<VisualElement>("in-game-overlay");
        labelBar = root.Q<VisualElement>("label-bar");
        blueTeam = root.Q<VisualElement>("blue-team");
        redTeam = root.Q<VisualElement>("red-team");
        playerInfoTemplate = Resources.Load<VisualTreeAsset>("UITemplate/player-info").CloneTree();
        ResetStyle();

        var players = NetworkManager.Singleton.ConnectedClients;
        foreach(var player in players)
        {
            var p = player.Value.PlayerObject.GetComponent<Player>();
            if(p.team.team == TeamEnum.Red)
            {
                redTeam.Add(GeneratePlayerInfo(p));
            }

            if (p.team.team == TeamEnum.Blue)
            {
                blueTeam.Add(GeneratePlayerInfo(p));
            }
        }
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(isShow)
            {
                ResetStyle();
                isShow = false;
            }
            else
            {
                Show();
                isShow = true;
            }
        }
    }

    void Show()
    {
        container.style.display = DisplayStyle.Flex;
    }

    void ResetStyle()
    {
        container.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Hàm này sẽ gán các giá trị trong PlayerInfo (tên, số bàn thắng, ...)
    /// </summary>
    /// <param name="player"></param>
    /// <returns>Trả về đối tượng VisualElement</returns>
    private VisualElement GeneratePlayerInfo(Player player)
    {
        VisualElement playerInfo = playerInfoTemplate;
        Label playerName = playerInfo.Query<Label>("player-name");
        Label playerGoal = playerInfo.Query<Label>("player-goal");
        Label playerTouchBall = playerInfo.Query<Label>("player-touch-ball");

        playerName.text = player.initialPlayerData.Value.playerName.Value;
        player.GoalTimes.OnValueChanged = (oldValue, newValue) =>
        {
            playerGoal.text = newValue.ToString();
        };

        player.TouchedBallTimes.OnValueChanged = (oldValue, newValue) =>
        {
            playerTouchBall.text = newValue.ToString();
        };
        return playerInfo;
    }
}