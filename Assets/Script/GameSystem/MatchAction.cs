using Assets.Script.Utlis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MatchAction
{
    GameSystem gameSystem;
    ColorAdjustments color;
    public bool MatchPause;
    public MatchAction(GameSystem g)
    {
        gameSystem = g;
        Volume volume = g.sceneReference.PostProcessingVolume;
        volume.profile.TryGet(out color);
        gameSystem.OnScoreChange +=(scorered,scoreblue,team) => { OnGoal(); } ;
    }
    public async void OnStartMatch()
    {
        gameSystem.DisplayerInformerClientRpc("Start match","Ready to play??",5);
         await PauseMatch(5);
        StartTimer();
    }
    public async Task PauseTimer(int sec)
    {
        MatchPause = true;
        incTimeSpeed = 0;
        await Task.Delay(sec * 1000);
        ResumeTimer();
    }
    public void ResumeTimer()
    {
        MatchPause = false;

        incTimeSpeed = 1;
    }
    int incTimeSpeed = 1;
    void StartTimer()
    {
        ThreadHelper.SafeThreadCall(() =>
        {
            while (true)
            {
                gameSystem.time.Value += incTimeSpeed;
                Thread.Sleep(1000);
            }
        });
    }
    public async void OnGoal()
    {
        await PauseMatch(10,true,false);
        var allPlayerInTheGame = gameSystem.room.playerDict;
        foreach (PlayerRoomManager Roomanager in allPlayerInTheGame.Values)
        {
            Roomanager.thisPlayer.TelebackToSpawnPoint();
            gameSystem.sceneReference.ball.BackToSpawnPos();
        }
        await PauseMatch(5);
        NewGame();
    }
    void NewGame()
    {
        var allPlayerInTheGame = gameSystem.room.playerDict;
        foreach (PlayerRoomManager Roomanager in allPlayerInTheGame.Values)
        {
            Roomanager.thisPlayer.TelebackToSpawnPoint();
        }
    }
    public async Task PauseMatch(int sec,bool changeSat = true,bool BallSuppress = true)
    {
        PauseTimer(sec);
        var allPlayerInTheGame = gameSystem.room.playerDict;
        foreach (PlayerRoomManager Roomanager in allPlayerInTheGame.Values)
        {
            Roomanager.thisPlayer.isSuppress.Value = true;
            
        }
        Ball ball = gameSystem.sceneReference.ball;
        ball.Suppress(BallSuppress);
        if (changeSat)
            gameSystem.ChangeClientSaturationClientRpc(-100);
        await Task.Delay(sec * 1000);
        ResumeMatch();
        if (changeSat)
            gameSystem.ChangeClientSaturationClientRpc(0);
    }
    public void ResumeMatch()
    {
        var allPlayerInTheGame = gameSystem.room.playerDict;
        foreach (PlayerRoomManager Roomanager in allPlayerInTheGame.Values)
        {
            Roomanager.thisPlayer.isSuppress.Value = false;
        }
        Ball ball = gameSystem.sceneReference.ball;
        ball.Suppress(false);

    }
    public void ChangeScreenStaturation(float v)
    {
        if (color != null)
        {
            color.saturation.value = v;
        }
    }
    
}

