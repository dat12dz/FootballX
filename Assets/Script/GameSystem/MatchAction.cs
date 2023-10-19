using Assets.Script.Utlis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
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
        gameSystem.OnScoreChange += (scorered,scoreblue,team) => { OnGoal(); } ;
    }

    public async void OnStartMatch()
    {
        if (!NetworkManager.Singleton.IsClient) return;
        gameSystem.DisplayerInformerClientRpc("Start match","Ready to play??",5);
         await PauseMatch(5);
        StartTimer();
        gameSystem.OnTimeChange += (time) => {
            if (gameSystem.time.Value  == (int)(gameSystem.EndGameTime / 2))
            EndHalf();
        };
    }
    public async void EndHalf()
    {
        gameSystem.DisplayerInformerClientRpc("End Half", "", 6);
        await PauseMatch(10);
        ResetGameScene();
        await PauseMatch(5);
        if (gameSystem.MatchHalf == 2)
        {
            EndGame();
        }
        else
        {
            gameSystem.DisplayerInformerClientRpc("Start new match half", "", 4);
            gameSystem.MatchHalf++;
        }
    }
    void EndGame()
    {
        Debug.Log("endGame");
    }
    public async Task PauseTimer(int sec)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        MatchPause = true;
        incTimeSpeed = 0;
        await Task.Delay(sec * 1000);
        ResumeTimer();
    }
    public void ResumeTimer()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        MatchPause = false;

        incTimeSpeed = 1;
    }
    int incTimeSpeed = 1;
    void StartTimer()
    {
        if (!NetworkManager.Singleton.IsClient) return;
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
        if (!NetworkManager.Singleton.IsServer) return;
        await PauseMatch(10,true,false);
        
        ResetGameScene();
        await PauseMatch(5);
        NewGame();
    }
    public void ResetGameScene()
    {
        var allPlayerInTheGame = gameSystem.room.playerDict;
        foreach (PlayerRoomManager Roomanager in allPlayerInTheGame.Values)
        {
            Roomanager.thisPlayer.TelebackToSpawnPoint();
            gameSystem.sceneReference.ball.BackToSpawnPos();
        }
    }
    void NewGame()
    {
        if (NetworkManager.Singleton.IsServer) return;
        var allPlayerInTheGame = gameSystem.room.playerDict;
        foreach (PlayerRoomManager Roomanager in allPlayerInTheGame.Values)
        {
            Roomanager.thisPlayer.TelebackToSpawnPoint();
        }
    }
    public async Task PauseMatch(int sec,bool changeSat = true,bool BallSuppress = true)
    {
        if (!NetworkManager.Singleton.IsServer) return;
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
        if (!NetworkManager.Singleton.IsServer) return;
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

