using Assets;
using Assets.Script.Networking.NetworkRoom;
using Assets.Script.Utlis;
using Assets.Utlis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[CheckNullProperties]
public class GameSystem : SceneNetworkBehavior
{
    [SerializeField] Volume PostProcessing;
    public static Action<uint> OnStartGameSystem;
    public uint EndGameTime = 900;
    public uint MatchHalf = 1;
    public enum Team
    {
        NULL,
        blue,
        red,
    }
    // Start is called before the first frame update
    public static GameSystem instance;
    public Action<int> OnTimeChange;
    public MatchAction MatchAction;
    public AutoFindNextDictionary<GameSystem> GameSystemList = new AutoFindNextDictionary<GameSystem>();
    public uint GameID;
    public Room room;
    static int MatchTimeSpeed;
    public NetworkVariable<int> time;
    /*        get { return Time_; }
            set 
            { 
                if (OnTimeChange != null) OnTimeChange(value);
                Time_ = value; 
            }  */

    //input: Redteam, BlueTeam , Team;
    public Action<int, int, Team> OnScoreChange;
    PhysicsScene ps;
    DrawSpawnPoint[] spawnPoint;
    [Header("Reference")]
    [SerializeField]
    public GameSystemSceneReference sceneReference;
    public NetworkVariable<int> ScoreBlueTeam;
    public NetworkVariable<int> ScoreRedTeam;
    
    [SerializeField] public SoundPlayer WhiselSoundPlayer;
    public void Init(Room room_)
    {
        // On Server code
        ps = gameObject.scene.GetPhysicsScene();
        spawnPoint = GetComponentsInChildren<DrawSpawnPoint>();
        room = room_;
        byte counter = 0;
        foreach (var player in room.playerDict.Values)
        {
            Player thisPlayerInfo = player.thisPlayer;
            thisPlayerInfo.SpawnPoint = spawnPoint[player.SlotInRoom.Value].transform;
            thisPlayerInfo.TelebackToSpawnPoint();
            SceneManager.MoveGameObjectToScene(player.gameObject, gameObject.scene);
            thisPlayerInfo.isInGame.Value = true;
            thisPlayerInfo.System = this;
            if (player.thisPlayer.isGoalKeeper)
            {
                if (player.SlotInRoom.Value == 4)
                {
                    sceneReference.RedTeanSceneRef.GoalKeeper = player.thisPlayer;
                }
                if (player.SlotInRoom.Value == 9)
                {
                    sceneReference.BlueTeamSceneRef.GoalKeeper = player.thisPlayer;
                }
            }
            counter++;

        }
        NetworkObject.Spawn();
    }
    public override void OnNetworkSpawn()
    {
        instance = this;
        sceneReference.Init(this);
        base.OnNetworkSpawn();
        MatchAction = new MatchAction(this);
        MatchAction.OnStartMatch();

    }
    void Start()
    {
        Application.targetFrameRate = 60;
        if (!NetworkObject.IsSpawned)
        {

            return;
        }
        GameID = GameSystemList.Add(this);
        if (OnStartGameSystem != null) OnStartGameSystem(GameID);

        ScoreRedTeam.OnValueChanged += (old, curr) =>
        {
            // Người ghi bànn
            Team Goaler = Team.NULL;
            // Tăng điểm
            if (curr > old) Goaler = Team.red;

            if (OnScoreChange != null) OnScoreChange(curr, ScoreBlueTeam.Value, Goaler);

        };
        ScoreBlueTeam.OnValueChanged += (old, curr) =>
        {
            Team Goaler = Team.NULL;
            if (curr > old)
                Goaler = Team.blue;
            if (OnScoreChange != null) OnScoreChange(ScoreRedTeam.Value, curr, Goaler);
        };

        time.OnValueChanged += (old, curr) =>
        {
            OnTimeChange(curr);
        };

    }
    [ContextMenu("End half 1")]
    void EndHalf()
    {
        time.Value = (int)(EndGameTime / 2);
    }
    [ContextMenu("EndGame")]
    void Engame()
    {
        time.Value = (int)EndGameTime - 1;
    }
    private void OnDisable()
    {
        GameSystemList.Remove(GameID);
    }
    private void OnDestroy()
    {
        GameSystemList.Remove(GameID);
    }

    public void ResetTimer()
    {
        time.Value = 0;
    }
    // Update is called once per frame
    void Update()
    {

    }
    [ClientRpc] public void PlayWhiselSound_ClientRpc()
    {
        WhiselSoundPlayer.PlayRandomSound();
    }    
    [ClientRpc] public void DisplayerInformerClientRpc(FixedString128Bytes name, FixedString128Bytes des, int time)
    { 
        UINew_InGameScreen.WaitForInstace(() =>
        {
            UINew_InGameScreen.instance.ShowInformation(name.ToString(), des.ToString(), 5);
        });
    }
    [ClientRpc] public void ChangeClientSaturationClientRpc(int s)
    {
        MatchAction.ChangeScreenStaturation(s);

    }
    [ClientRpc] public void DisplayerFinalResultWin_ClientRpc(ClientRpcParams rpcParam = default)
    {
        UInew_ShowFinalResult.instance.ShowOverallMatchResult("Victory",5).ContinueWith(t =>
        {
            MainThreadDispatcher.ExecuteInMainThread(() => Client_DisplayerFinalResultBase());
        }); ;
        
    }
    [ClientRpc] public void DisplayerFinalResultLoss_ClientRpc(ClientRpcParams rpcParam = default)
    {
        UInew_ShowFinalResult.instance.ShowOverallMatchResult("Loss", 5).ContinueWith(t =>
        {
            MainThreadDispatcher.ExecuteInMainThread(() => Client_DisplayerFinalResultBase());

        }); ;
       
    }
    [ClientRpc] public void DisplayerFinalResultTie_ClientRpc()
    {
        UInew_ShowFinalResult.instance.ShowOverallMatchResult("Tie", 5).ContinueWith(t =>
        {
            MainThreadDispatcher.ExecuteInMainThread(() => Client_DisplayerFinalResultBase());
          
        });
    }
    public TeamEnum? Winner;
    public async void Client_DisplayerFinalResultBase()
    {
        var AllPlayer = CaculateRank();
        TeamEnum? winner()
        {
            TeamEnum? res = null;
            if (this.ScoreBlueTeam.Value > this.ScoreRedTeam.Value)
            {
                res = TeamEnum.Blue;
            }
            if (this.ScoreBlueTeam.Value < this.ScoreRedTeam.Value)
            {
                res = TeamEnum.Red;
            }
            return res;
        }
         Winner = winner();

        var PlayerListSorted = this.CaculateRank();
        Player FindMvp()
        {
            if (Winner.HasValue)
            for (int i = 0; i < PlayerListSorted.Length; i++)
            {

                Player p = PlayerListSorted[i].thisPlayer;
               
                    if (p.team.team == Winner) return p;
                        
            }
            
                return PlayerListSorted[0].thisPlayer;
         
        }
        var Mvp = FindMvp();
        var Playertxt = await Mvp.thisPlayerModel.PlayMvpAnimation();
        UInew_ShowFinalResult.instance.DisplayMvp(Mvp, Playertxt);
    //    Player.localPlayer.TogglePoolObj(false);
    }    

    public Player[] Client_GetAllPlayerList()
    {
        return GameObject.FindObjectsOfType<Player>();
    }    
    /*public bool Pause;*/
    private void FixedUpdate()
    {
        /*if (!Pause)*/
          ps.Simulate(Time.fixedDeltaTime);
    }

    public PlayerRoomManager[] CaculateRank()
    {
        PlayerRoomManager[] PlayerSortedRank;
        PlayerSortedRank = room.playerDict.Values.ToArray();
       Array.Sort(PlayerSortedRank,new ComparePlayer());
        return PlayerSortedRank; 
    }
    class ComparePlayer : IComparer
    {
        public int Compare(object x, object y)
        {
            var PlayerX = (PlayerRoomManager)x;
            var PlayerY = (PlayerRoomManager)y;
            if (PlayerX.thisPlayer.Score > PlayerY.thisPlayer.Score)
            {
                return -1;
            }
            if (PlayerX.thisPlayer.Score < PlayerY.thisPlayer.Score)
            {
                return 1;
            }
            return 0;
        }
    }
}
