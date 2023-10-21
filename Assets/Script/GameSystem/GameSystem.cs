using Assets;
using Assets.Script.Networking.NetworkRoom;
using Assets.Script.Utlis;
using Assets.Utlis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    public Action<int,int, Team> OnScoreChange;
    PhysicsScene ps;
    DrawSpawnPoint[] spawnPoint;
    [Header("Reference")]
    [SerializeField]
    public GameSystemSceneReference sceneReference;
    public NetworkVariable<int> ScoreBlueTeam;
    public NetworkVariable<int> ScoreRedTeam;
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
            counter++;     
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        sceneReference.Init(this);
        MatchAction = new MatchAction(this);
        MatchAction.OnStartMatch();
    }
    void Start()
    {
        Application.targetFrameRate = 60;
        if (!NetworkObject.IsSpawned)
        {
            Destroy(gameObject);
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
        instance = this;
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
    [ClientRpc] public void DisplayerInformerClientRpc(FixedString128Bytes name, FixedString128Bytes des, int time)
    { 
        UIHandler.WaitForInstace(() => UIHandler.instance.ShowInformation(name.ToString(), des.ToString(), 5));
    }
    [ClientRpc] public void ChangeClientSaturationClientRpc(int s)
    {
        MatchAction.ChangeScreenStaturation(s);
    }
    
    /*public bool Pause;*/
    private void FixedUpdate()
    {
        /*if (!Pause)*/
          ps.Simulate(Time.fixedDeltaTime);
    }

}
