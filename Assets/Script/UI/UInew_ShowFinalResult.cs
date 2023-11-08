using Assets.Script.NetCode;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UInew_ShowFinalResult : MonoBehaviour
{
    UIDocument document;
    VisualElement MvpDisplayer;
    VisualElement OveralResult;
    VisualElement ShowAllInfo;
    VisualElement pnl_PlayerInfo;
    VisualElement MatchResult_infoCard;
    Button btn_Continue1, btn_Continue2;
    Label lb_OverallResult;
    Label lb_PlayerName, lb_Score, lb_Change, lb_ShowKS;
    public static UInew_ShowFinalResult instance;
    private void Start()
    {
        var gameSystem = SceneHelper.GetGameSystem(gameObject.scene);
        instance = this;

        document = GetComponent<UIDocument>();
        var RootvsElement = document.rootVisualElement;
        MvpDisplayer = RootvsElement.Q<VisualElement>("pnl_MVPDisplayer");
        OveralResult = RootvsElement.Q<VisualElement>("pnl_OveralResult");
        ShowAllInfo = RootvsElement.Q<VisualElement>("pnl_ShowAllInfo");
        pnl_PlayerInfo = RootvsElement.Q<VisualElement>("pnl_PlayerInfo");

        btn_Continue1 = RootvsElement.Q<Button>("Btn_continue1");
        btn_Continue2 = RootvsElement.Q<Button>("Btn_continue2");

        lb_OverallResult = RootvsElement.Q<Label>("lb_OverallResult");
        lb_Score = RootvsElement.Q<Label>("lb_Score");
        lb_Change = RootvsElement.Q<Label>("lb_Change");
        lb_ShowKS = RootvsElement.Q<Label>("lb_ShowKS");

        MatchResult_infoCard = Resources.Load<VisualTreeAsset>("/UITemplate/MatchResult_infoCard").CloneTree();
        //lb_PlayerIndex = RootvsElement.Q<Label>("lb_PlayerIndex");
        //lb_PlayerName = RootvsElement.Q<Label>("lb_PlayerName");
        //lb_KS = RootvsElement.Q<Label>("lb_KS");
        //lb_MVP = RootvsElement.Q<Label>("lb_MVP");

        btn_Continue1.clicked += () =>
        {
            MvpDisplayer.style.display = DisplayStyle.None;
            DisplayAllInfomation(gameSystem.Client_GetAllPlayerList());
        };

        ResetStyle();
    }

    public async void ShowOverallMatchResult(string text,int sec)
    {
        OveralResult.style.display = DisplayStyle.Flex;
        lb_OverallResult.text = text;
        await Task.Delay(sec * 1000);
        OveralResult.style.display = DisplayStyle.None;
    }
    // Update is called once per frame
    public void DisplayMvp(Player mvpPlayer,Texture2D PlayerMVPTexture)
    {
        Img_PlayerMvpImage.style.backgroundImage = PlayerMVPTexture;
        MvpDisplayer.style.display = DisplayStyle.Flex;

        lb_PlayerName.text = mvpPlayer.initialPlayerData.Value.playerName.Value;
        lb_Score.text = mvpPlayer.Score.ToString();

        if (mvpPlayer.isGoalKeeper)
        {
            lb_Change.text = "Catch ball times";
            lb_ShowKS.text = mvpPlayer.CatchedBallTimes.Value.ToString();
        }

        else
        {
            lb_Change.text = "K/T";
            lb_ShowKS.text = mvpPlayer.TouchedBallTimes.Value.ToString() + "/" + mvpPlayer.GoalTimes.Value.ToString();
        }
    }
    public void DisplayAllInfomation(Player[] allPlayerList)
    {
        ShowAllInfo.style.display = DisplayStyle.Flex;

        Label lb_PlayerIndex = MatchResult_infoCard.Q<Label>("lb_PlayerIndex");
        Label lb_PlayerName = MatchResult_infoCard.Q<Label>("lb_PlayerName");
        Label lb_KS = MatchResult_infoCard.Q<Label>("lb_KS");
        Label lb_Score = MatchResult_infoCard.Q<Label>("lb_Score");
        Label lb_MVP = MatchResult_infoCard.Q<Label>("lb_MVP");

        allPlayerList = allPlayerList.OrderByDescending(p => p.Score).ToArray();
        for (int i = 1; i <= allPlayerList.Length; i++)
        {
            lb_PlayerIndex.text = i.ToString();
            lb_PlayerName.text = allPlayerList[i].name;
            lb_KS.text = allPlayerList[i].Score.ToString();
            lb_Score.text = allPlayerList[i].Score.ToString();

            if(i > 2)
            {
                lb_MVP.text = null;
            }
            pnl_PlayerInfo.Add(MatchResult_infoCard);
        }
    }

    void ResetStyle()
    {
        MvpDisplayer.style.display = DisplayStyle.None;
        OveralResult.style.display = DisplayStyle.None;
        ShowAllInfo.style.display = DisplayStyle.None;
    }
}
