using Assets.Script;
using Assets.Script.Utlis;
using Assets.Script.Utlis.CheckNullProp;
using Assets.Utlis;
using System;
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
    }

    public async void OnStartMatch()
    {
        if (!NetworkManager.Singleton.IsClient) return;
        gameSystem.DisplayerInformerClientRpc("Start match", "Ready to play??", 5);
        await PauseMatch(5);
        StartTimer();
        PlayWhiselSound();
        gameSystem.OnTimeChange += (time) =>
        {
            if (gameSystem.time.Value == (int)(gameSystem.EndGameTime / 2))
                MainThreadDispatcher.ExecuteInMainThread(() => EndHalf());
            if (gameSystem.time.Value == gameSystem.EndGameTime)
                MainThreadDispatcher.ExecuteInMainThread(() => EndGame()); 
        };
    }
    public async void EndHalf()
    {
        PlayWhiselSound();
        gameSystem.DisplayerInformerClientRpc("End Half", "", 6);
        await PauseMatch(5);
        ResetGameScene();
        {
            gameSystem.DisplayerInformerClientRpc("Start new match half", "", 4);
            gameSystem.MatchHalf++;
        }
        await PauseMatch(5);

    }
    async void EndGame()
    {
        PlayWhiselSound();
        PauseMatch(5);
        TeamEnum? winner()
        {
            TeamEnum? res = null;
            if (gameSystem.ScoreBlueTeam.Value > gameSystem.ScoreRedTeam.Value)
            {
                res = TeamEnum.Blue;
            }
            if (gameSystem.ScoreBlueTeam.Value < gameSystem.ScoreRedTeam.Value)
            {
                res = TeamEnum.Red;
            }
            return res;
        }
        var Winner = winner();

        
        if (!Winner.HasValue)
        {
            gameSystem.DisplayerFinalResultTie_ClientRpc();
        }
        else
        {
            foreach (PlayerRoomManager player in gameSystem.room.playerDict.Values)
            {
               var ClientRpc = NetworkkHelper.CreateRpcTo(player.OwnerClientId);
                if (player.thisPlayer.team.team != Winner)
                gameSystem.DisplayerFinalResultLoss_ClientRpc(ClientRpc);
                if (player.thisPlayer.team.team == Winner)
                    gameSystem.DisplayerFinalResultWin_ClientRpc(ClientRpc);
                player.thisPlayer.isInGame.Value = false;

            }
        }
        if (!NetworkManager.Singleton.IsClient)
        {
            await Task.Delay(5000);
        }
    }
    CancellationTokenSource PauseTimer_cancel;

    public async Task<bool> PauseTimer(int sec)
    {
        if (PauseTimer_cancel != null) return false;
        PauseTimer_cancel = new CancellationTokenSource();
        if (!NetworkManager.Singleton.IsServer) return false;
        MatchPause = true;
        incTimeSpeed = 0;
        try
        {
            await Task.Delay(sec * 1000, PauseTimer_cancel.Token);
            ResumeTimer();
        }
        catch (TaskCanceledException)
        {
            return false;
     
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }
    public void ResumeTimer()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        MatchPause = false;
        incTimeSpeed = 1;
        if (PauseTimer_cancel != null)
            PauseTimer_cancel.Cancel();
        PauseTimer_cancel.Dispose();
        PauseTimer_cancel = null;
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
    public async void OnGoal(Player GoalMaker,TeamEnum teamAddPoint)
    {
        if (GoalMaker.team.team == teamAddPoint) 
        {
            GoalMaker.GoalTimes.Value++;
        }
        else
        {
            GoalMaker.GoalTimes.Value--;
        }
        if (!NetworkManager.Singleton.IsServer) return;
        await PauseMatch(10, true, false);
        ResetGameScene();
        await PauseMatch(5);
        PlayWhiselSound();
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
       
        if (gameState != GameStateEnum.Playing) return;
        PlayWhiselSound();
        Player PlayerHoldBall = ThrowInTeam.ThrowInTeam.GetRandomPlayer();
        string PlayerHoldBallname = PlayerHoldBall != null ? PlayerHoldBall.initialPlayerData.Value.playerName.ToString() : "Nobody";
        gameSystem.DisplayerInformerClientRpc("Throw In", $"{ThrowInTeam.ThrowInmakaer.initialPlayerData.Value.playerName} shoot the ball out of the Throw-in Range,{PlayerHoldBallname} will be the thrower", 5);
        await PauseMatch(5, true, false);
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

                PlayerHoldBall.OnThrowSomeThing -= OnPlayerThrowBall;
            };
            PlayerHoldBall.OnThrowSomeThing += OnPlayerThrowBall;
            await PauseTimer(50);
            PlayerHoldBall.Throw(PlayerHoldBall.MinThrowingForce);
            PlayerHoldBall.SuppressPlayer(false);
        }
    }
    public async void StartConerKickPhase(StartCornerKickPhase_Info info)
    {
        if (gameState == GameStateEnum.Playing)
        {
            PlayWhiselSound();
            Ball ball = gameSystem.sceneReference.ball;
            gameState = GameStateEnum.CornerKick;
            var ConerKickTakerName = info.ConerKickTaker == null ? "Nobody" : info.ConerKickTaker.initialPlayerData.Value.playerName;
            var CornerKickSubMessage = $"{info.CornerKickMaker.initialPlayerData.Value.playerName} strike the ball out of the zone,{ConerKickTakerName} will take the corner kick";
            gameSystem.DisplayerInformerClientRpc("Corner Kick", CornerKickSubMessage, 5);
            await PauseMatch(5, true, false);
            if (info.ConerKickTaker)
            {
                var CornerKickTaker = info.ConerKickTaker;
                var cornerKickTakerNetTransform = CornerKickTaker.GetComponent<PlayerNetworkTransform>();
                var TeamSeparateSceneRef = info.ConerKickTaker.team.GetOpponentTeam().GetSceneRefernce(gameSystem);
                // Check distance between PlayerPositionR and PlayerPositionL
                var DistanceL = Vector3.Distance(TeamSeparateSceneRef.CornerKickPlayerPosL.position, ball.transform.position);
                var DistanceR = Vector3.Distance(TeamSeparateSceneRef.CornerKickPlayerPosR.position, ball.transform.position);
                Vector3 PlayerPosition;
                Vector3 BallPosition;
                if (DistanceL > DistanceR)
                {
                    PlayerPosition = TeamSeparateSceneRef.CornerKickPlayerPosR.position;
                    BallPosition = TeamSeparateSceneRef.CornerKickBallSetterR.position;
                }
                else
                {
                    PlayerPosition = TeamSeparateSceneRef.CornerKickPlayerPosL.position;
                    BallPosition = TeamSeparateSceneRef.CornerKickBallSetterL.position;
                }

                CornerKickTaker.transform.position = PlayerPosition;
                cornerKickTakerNetTransform.TeleportImidiateClientRpc(PlayerPosition);
                ball.transform.position = BallPosition;
                CornerKickTaker.ToggleUnstanableZone(true);
                CornerKickTaker.ToggleUnstandbaleZone_ClientRpc(true);
                CornerKickTaker.SuppressPlayer(true, PlayerPosition, 2);
                Action OnShootBall = null;
                OnShootBall = () =>
                {
                    ResumeTimer();
                    CornerKickTaker.OnShootBall -= OnShootBall;
                };
                CornerKickTaker.OnShootBall += OnShootBall;
                await PauseTimer(45);
                CornerKickTaker.ToggleUnstanableZone(false);
                CornerKickTaker.ToggleUnstandbaleZone_ClientRpc(false);
                CornerKickTaker.SuppressPlayer(false);

            }
            gameState = GameStateEnum.Playing;
        }
    }
    public async void GoalKeeperTakeTheBall(string PlayerShootTheBallOut, TeamClass team)
    {
        PlayWhiselSound();
        gameSystem.DisplayerInformerClientRpc("Ball out", $"{PlayerShootTheBallOut} shoot the ball out of the map", 5);
        await PauseMatch(5, true);
        var SceneReferencec = team.GetSceneRefernce(gameSystem);
        Player GoalKeeperPlayer = SceneReferencec.GoalKeeper;
        if (GoalKeeperPlayer)
        {
            // Set ball to the ball
            gameSystem.sceneReference.ball.transform.position = SceneReferencec.GoalKeeperBallSetter.position;
            ResetGameScene(false);
            GoalKeeperPlayer.CanGrab = false;
            GoalKeeperPlayer.ToggleUnstanableZone(true);
            GoalKeeperPlayer.ToggleUnstandbaleZone_ClientRpc(true);
            Action OnShoot = null;
            OnShoot = () =>
            {
                GoalKeeperPlayer.CanGrab = true;
                GoalKeeperPlayer.OnShootBall -= OnShoot;
                GoalKeeperPlayer.ToggleUnstanableZone(false);
                GoalKeeperPlayer.ToggleUnstandbaleZone_ClientRpc(false);
            };
            GoalKeeperPlayer.OnShootBall += OnShoot;
            var task = PauseTimer(10);
            await task;
            if (task.Result) // Wait complete and the goalkeeper still not shoot the ball
                GoalKeeperPlayer.AutoShootBall();
        }
        else
        {
            ResetGameScene(true);
        }
    }
    public async void GoalKeeperCatchTheBall(Player GoalKeeper)
    {
        if (GoalKeeper == null)
        {
            ResetGameScene();
        }
        gameSystem.DisplayerInformerClientRpc("GoalKeeper caught the ball", $"{GoalKeeper.initialPlayerData.Value.playerName} : Caught the ball", 5);
        await PauseMatch(5, true);

        var GoalKeeperZone = GoalKeeper.team.GetSceneRefernce(gameSystem).GoalKeeperZone;
        GoalKeeper.SuppressPlayer(true, GoalKeeperZone);

        Action<Grabable> OnThrow = null;
        OnThrow = (ball) =>
        {
            GoalKeeper.SuppressPlayer(false);
            GoalKeeper.OnThrowSomeThing -= OnThrow;
        };
        GoalKeeper.OnThrowSomeThing += OnThrow;
        var t = PauseTimer(10);
        await t;
        if (t.Result)
        {
            GoalKeeper.Throw(GoalKeeper.FinalThrowForce);
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
    public async Task PauseMatch(int sec, bool changeSat = true, bool BallSuppress = true)
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
    public void PlayWhiselSound()
    {
        gameSystem.PlayWhiselSound_ClientRpc();
    }
}

