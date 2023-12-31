using Assets.Script.NetCode;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UInew_ShowFinalResult : MonoBehaviour
{
    UINew_MVPScreen uiMvpScreen;

    UIDocument document;
    VisualElement container;
    VisualElement MvpDisplayer;
    VisualElement Img_PlayerMvpImage;
    VisualElement MvpScreen;
    VisualElement OveralResult;
    VisualElement ShowAllInfo;
    VisualElement pnl_PlayerInfo;
    Button btn_Continue1, btn_Continue2;
    Label lb_OverallResult;
    Label lb_PlayerName, lb_Score, lb_Change, lb_ShowKS;
    public static UInew_ShowFinalResult instance;
    private async void Start()
    {
        var gameSystem = await SceneHelper.GetGameSystem(gameObject.scene);
        instance = this;

        document = GetComponent<UIDocument>();
        var RootvsElement = document.rootVisualElement;

        uiMvpScreen = new UINew_MVPScreen(RootvsElement);

        container = RootvsElement.Q<VisualElement>("container");
        Img_PlayerMvpImage = RootvsElement.Q<VisualElement>("Img_MvpPlayer");
        MvpDisplayer = RootvsElement.Q<VisualElement>("pnl_MVPDisplayer");
        OveralResult = RootvsElement.Q<VisualElement>("pnl_OveralResult");
        MvpScreen = UINew_MVPScreen.mvpScreen;
        ShowAllInfo = RootvsElement.Q<VisualElement>("pnl_ShowAllInfo");
        pnl_PlayerInfo = RootvsElement.Q<VisualElement>("pnl_PlayerInfo");

        btn_Continue1 = RootvsElement.Q<Button>("Btn_continue1");
        btn_Continue2 = RootvsElement.Q<Button>("Btn_continue2");

        lb_OverallResult = RootvsElement.Q<Label>("lb_OverallResult");
        lb_Score = RootvsElement.Q<Label>("lb_Score");
        lb_Change = RootvsElement.Q<Label>("lb_Change");
        lb_ShowKS = RootvsElement.Q<Label>("lb_ShowKS");
        lb_PlayerName = RootvsElement.Q<Label>("lb_PlayerName");

        //lb_PlayerIndex = RootvsElement.Q<Label>("lb_PlayerIndex");
        //lb_KS = RootvsElement.Q<Label>("lb_KS");
        //lb_MVP = RootvsElement.Q<Label>("lb_MVP");

        btn_Continue1.clicked += () =>
        {
            Debug.Log("continue1 Btn is click");
            uiMvpScreen.ResetStyle();
            MvpDisplayer.style.display = DisplayStyle.None;
            DisplayAllInfomation(gameSystem.Client_GetAllPlayerList());
        };

        btn_Continue2.clicked += () =>
        {
            Debug.Log("continue2 Btn is click");
            ShowAllInfo.style.display = DisplayStyle.None;
            container.style.display = DisplayStyle.None;
            MvpPlayer.thisPlayerModel.ResetAnimation();
            GameSystem.instance.ResetScene();
        };

        //btn_Continue2.clicked += () =>
        //{
        //    Debug.Log("continue2 Btn is click");
        //    ShowAllInfo.style.display = DisplayStyle.None;
        //    container.style.display = DisplayStyle.None;
        //};

        ResetStyle();
    }

    public async Task ShowOverallMatchResult(string text,int sec)
    {
        container.style.display = DisplayStyle.Flex;
        OveralResult.style.display = DisplayStyle.Flex;
        lb_OverallResult.text = text;
        await Task.Delay(2 * 1000);
        OveralResult.style.display = DisplayStyle.None;
    }
    // Update is called once per frame
    Player MvpPlayer;
    public async void DisplayMvp(Player mvpPlayer,RenderTexture PlayerMVPTexture)
    {
        MvpPlayer = mvpPlayer;
        MvpDisplayer.style.display = DisplayStyle.Flex;
        Img_PlayerMvpImage.style.backgroundImage =  Background.FromRenderTexture(PlayerMVPTexture);
        await Task.Delay(1);
        uiMvpScreen.Show();

        lb_PlayerName.text = mvpPlayer.initialPlayerData.Value.playerName.ToString();
        lb_Score.text = mvpPlayer.Score.ToString();

        if (mvpPlayer.isGoalKeeper)
        {
            lb_Change.text = "Catch ball times";
            lb_ShowKS.text = mvpPlayer.CatchedBallTimes.Value.ToString();
        }

        else
        {
            lb_Change.text = "K/T";
            lb_ShowKS.text =   mvpPlayer.GoalTimes.Value.ToString() + "/" + mvpPlayer.TouchedBallTimes.Value.ToString();
        }
    }
    public void DisplayAllInfomation(Player[] allPlayerList)
    {
        ShowAllInfo.style.display = DisplayStyle.Flex;
        bool isRedPlayerMVP = false;
        bool isBluePlayerMVP = false;

        allPlayerList = allPlayerList.OrderByDescending(p => p.Score).ToArray();

        for (int i = 0; i < allPlayerList.Length; i++)
        {
            VisualElement MatchResult_infoCard = Resources.Load<VisualTreeAsset>("UITemplate/MatchResult_infoCard").CloneTree();
            Label lb_PlayerIndex = MatchResult_infoCard.Q<Label>("lb_PlayerIndex");
            Label lb_PlayerName = MatchResult_infoCard.Q<Label>("lb_PlayerName");
            Label lb_KS = MatchResult_infoCard.Q<Label>("lb_KS");
            Label lb_Score = MatchResult_infoCard.Q<Label>("lb_Score");
            Label lb_MVP = MatchResult_infoCard.Q<Label>("lb_MVP");

            lb_PlayerIndex.text = (i + 1).ToString();
            lb_PlayerName.text = allPlayerList[i].initialPlayerData.Value.playerName.ToString();
            lb_KS.text = "K/T: " + allPlayerList[i].GoalTimes.Value.ToString() + "/" + allPlayerList[i].TouchedBallTimes.Value.ToString();
            lb_Score.text = "Score: " + allPlayerList[i].Score.ToString();


            if (GameSystem.instance.Winner == null)
            {
                lb_MVP.text = null;
                pnl_PlayerInfo.Add(MatchResult_infoCard);
                continue;
            }

            if (allPlayerList[i].team.team == TeamEnum.Red)
            {
                if (isRedPlayerMVP == true)
                {
                    lb_MVP.text = null;
                    pnl_PlayerInfo.Add(MatchResult_infoCard);
                    continue;
                }

                isRedPlayerMVP = true;
                if (GameSystem.instance.Winner == TeamEnum.Red)
                {
                    lb_MVP.style.color = new StyleColor(new Color(200f / 255, 0, 0)); // redColor
                }
                else lb_MVP.style.color = new StyleColor(new Color(90f / 255, 0, 150f / 255)); // violetColor

                lb_MVP.text = "MVP";
                pnl_PlayerInfo.Add(MatchResult_infoCard);
                continue;
            }

            if (allPlayerList[i].team.team == TeamEnum.Blue)
            {
                if (isBluePlayerMVP == true)
                {
                    lb_MVP.text = null;
                    pnl_PlayerInfo.Add(MatchResult_infoCard);
                    continue;
                }

                isBluePlayerMVP = true;
                if (GameSystem.instance.Winner == TeamEnum.Blue)
                {
                    lb_MVP.style.color = new StyleColor(new Color(200f / 255, 0, 0)); // redColor
                }
                else lb_MVP.style.color = new StyleColor(new Color(90f / 255, 0, 150f / 255)); // violetColor
                    
                lb_MVP.text = "MVP";
                pnl_PlayerInfo.Add(MatchResult_infoCard);
                continue;
            }

        }
    }

    void ResetStyle()
    {
        container.style.display = DisplayStyle.None;
        MvpDisplayer.style.display = DisplayStyle.None;
        OveralResult.style.display = DisplayStyle.None;
        ShowAllInfo.style.display = DisplayStyle.None;
    }
}
