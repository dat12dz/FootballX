using Assets.Script;
using Assets.Script.Utlis;
using Assets.Utlis;
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
    public GameStateEnum gameState;
    object locker = new object();
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
        
        if (!NetworkManager.Singleton.IsServer) return;
        ThreadHelper.SafeThreadCall(() =>
        {
            
            while (true)
            {
                lock (locker)
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
       
    }
    public void ResetGameScene(bool ResetBall = true)
    {
        var allPlayerInTheGame = gameSystem.room.playerDict;
        if (ResetBall)
        gameSystem.sceneReference.ball.BackToSpawnPos();
        foreach (PlayerRoomManager Roomanager in allPlayerInTheGame.Values)
        {
            Roomanager.thisPlayer.TelebackToSpawnPoint();

        }
       
    }
    public async void StartThrowInPhase(StartThrowInPhase_Info ThrowInTeam)
    {
        Player PlayerHoldBall = ThrowInTeam.ThrowInTeam.GetRandomPlayer();
       string PlayerHoldBallname = PlayerHoldBall != null ? PlayerHoldBall.initialPlayerData.Value.playerName.ToString() : "Nobody";
        gameSystem.DisplayerInformerClientRpc("Throw In", $"{ThrowInTeam.ThrowInmakaer.initialPlayerData.Value.playerName} shoot the ball out of the Throw-in Range,{PlayerHoldBallname} will be the thrower", 5);
        await PauseMatch(5,true,false);
        if (PlayerHoldBall != null)
        {
            ResetGameScene();
            gameState = GameStateEnum.ThrowIn;
            var PlayerHoldBall_NetworkTransform = PlayerHoldBall.GetComponent<PlayerNetworkTransform>();
            PlayerHoldBall_NetworkTransform.transform.position = ThrowInTeam.ThrowInPosition;
            PlayerHoldBall_NetworkTransform.TeleportImidiateClientRpc(ThrowInTeam.ThrowInPosition);
            PlayerHoldBall.ToggleUnstandbaleZone_ClientRpc(true);
            PlayerHoldBall.SuppressPlayer(true);            
            PlayerHoldBall.Grab(gameSystem.sceneReference.ball);
           
     
            Action<Grabable> OnPlayerThrowBall = null;
            
            OnPlayerThrowBall = (ball) =>
            {
                gameState = GameStateEnum.Playing;
                ResumeTimer();
                PlayerHoldBall.SuppressPlayer(false);
                PlayerHoldBall.OnThrowSomeThing -= OnPlayerThrowBall;
            };
            PlayerHoldBall.OnThrowSomeThing += OnPlayerThrowBall;
           await PauseTimer(50);
            PlayerHoldBall.Throw(PlayerHoldBall.MinThrowingForce);
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

