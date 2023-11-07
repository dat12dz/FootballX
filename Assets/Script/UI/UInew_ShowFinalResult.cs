using Assets.Script.NetCode;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UInew_ShowFinalResult : MonoBehaviour
{
    UIDocument document;
    Label lb_OverallResult;
    VisualElement MvpDisplayer;
    Button btn_Continue1, btn_Continue2;
    public static UInew_ShowFinalResult instance;
    private void Start()
    {
        var gameSystem = SceneHelper.GetGameSystem(gameObject.scene);
        instance = this;
        document = GetComponent<UIDocument>();
        var RootvsElement = document.rootVisualElement;
        MvpDisplayer = RootvsElement.Q<VisualElement>("pnl_MVPDisplayer");
        btn_Continue1 = RootvsElement.Q<Button>("Btn_continue1");
        btn_Continue1.clicked += () =>
        {
            DisplayAllInfomation(gameSystem.Client_GetAllPlayerList());
        };
        lb_OverallResult = RootvsElement.Q<Label>("lb_OverallResult");
    }
    public async void ShowOverallMatchResult(string text,int sec)
    {
        lb_OverallResult.style.display = DisplayStyle.Flex; 
        lb_OverallResult.text = text;       
        await Task.Delay(sec * 1000);
        lb_OverallResult.style.display = DisplayStyle.None;
    }
    // Update is called once per frame
    public void DisplayMvp(Player mvpPlayer)
    {
        MvpDisplayer.style.display = DisplayStyle.Flex;
        if (mvpPlayer.isGoalKeeper)
        {
            //  mvpPlayer.CatchedBallTimes.Value
            //    mvpPlayer.Score
        }
        else
        {
            //  mvpPlayer.TouchedBallTimes;
            //  mvpPlayer.GoalTimes
        }
    }    
    public void DisplayAllInfomation(Player[] allPlayerList)
    {

    }
    void Update()
    {
        
    }
}
