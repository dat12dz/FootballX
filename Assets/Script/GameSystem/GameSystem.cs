using Assets;

using Assets.Script.Utlis;
using Assets.Utlis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[CheckNullProperties]
public class GameSystem : NetworkBehaviour
{
    [SerializeField] Volume PostProcessing;
    public static Action<uint> OnStartGameSystem;
  public enum Team
    {
        NULL,
        blue,
        red,
    }
    // Start is called before the first frame update
    public static GameSystem instance;
    public Action<int> OnTimeChange;
    int Time_;
    public AutoFindNextDictionary<GameSystem> GameSystemList = new AutoFindNextDictionary<GameSystem>();
    public uint GameID;
    public Room room;
    static int MatchTimeSpeed;
    int time { 
        get { return Time_; }
        set 
        { 
            if (OnTimeChange != null) OnTimeChange(value);
            Time_ = value; 
        }  
    }
    //input: Redteam, BlueTeam , Team;
    public Action<int,int, Team> OnScoreChange;
    PhysicsScene ps;
    DrawSpawnPoint[] spawnPoint;
    [Header("Reference")]
    [SerializeField]
    public GameSystemSceneReference sceneReference;
    public NetworkVariable<int> ScoreBlueTeam;
/*    {
        get { return ScoreBlueTeam_; }
        set
        {
            Team Goaler = Team.NULL;
            if (value > ScoreBlueTeam_)
            
                Goaler = Team.blue;
            
            if (OnScoreChange != null) OnScoreChange(ScoreRedTeam,value, Goaler);
          
            ScoreBlueTeam_ = value;
        }
    }  */
   public NetworkVariable<int> ScoreRedTeam;
/*    {
        get { return ScoreRedTeam_; }
        set
        {
            Team Goaler = Team.NULL;
            if (value > ScoreBlueTeam_) Goaler = Team.red;
            if (OnScoreChange != null) OnScoreChange(value,ScoreBlueTeam, Goaler);
            ScoreRedTeam_ = value;
        }
    }   */
    public void Init(Room room_)
    {
        // On Server code
        ps = gameObject.scene.GetPhysicsScene();
        spawnPoint = GetComponentsInChildren<DrawSpawnPoint>();
        room = room_;
        byte counter = 0;
        foreach (var player in room.playerDict.Values)
        {   

            // Dịch chuy
            player.transform.position = spawnPoint[player.SlotInRoom.Value].transform.position;
            player.GetComponent<PlayerNetworkTransform>().TeleportImidiateClientRpc(player.transform.position);
            SceneManager.MoveGameObjectToScene(player.gameObject, gameObject.scene);
            player.thisPlayer.isInGame.Value = true;

            player.thisPlayer.System = this;
            counter++;
            
        }
        sceneReference.Init();


    }

    void Start()
    {
        GameID = GameSystemList.Add(this);
        if (OnStartGameSystem != null) OnStartGameSystem(GameID);
        InitPhasetest();
        instance = this;
        ThreadHelper.SafeThreadCall(() =>
        {
            while (true)
            {
                time += MatchSystem.MatchTimeSpeed;
                Thread.Sleep(1000);
                
            }
        });
        
        // Start game phase thread
      ThreadHelper.SafeThreadCall(() =>
        {
            // loop
           
            var QueuePhase = MatchSystem.QueuePhase;
            if (QueuePhase != null)
            for (int  i = 0;i < QueuePhase.Count;i++)
            {
                // Get other phase
                var Phase = QueuePhase.Dequeue();
                MatchSystem.currentMachPhase = Phase;

                // Start phase
                Phase.Begin();
                

                // if the time of the phase not end
                while (time < Phase.Length)
                {

                    if (!Phase.isContinue)
                    {

                        PostProcessingHelper.ToggleNoSaturation(PostProcessing, true);
                        MatchSystem.MatchTimeSpeed = 0;
                    }
                    else
                    {
                       
                        PostProcessingHelper.ToggleNoSaturation(PostProcessing, false);
                    }
                    Thread.Sleep(100);
                        
                       // NetworkSystem.instance.OnDisconect += (s) => { Phase.End(); };
                }
                // Phase end
                ResetTimer();
                Phase.End();
            }
        });

        OnScoreChange += PauseWhenGoal;
    }
    private void OnDisable()
    {
        GameSystemList.Remove(GameID);
    }
    private void OnDestroy()
    {
        GameSystemList.Remove(GameID);
    }
    void PauseWhenGoal(int red, int blue, Team t)
    {
        if (t != Team.NULL)
        {
            PauseCurrentPhase(5);
        }
    }
    void InitPhasetest()
    {
        var phase1 = new WarmUpPhase();

        MatchSystem.QueuePhase.Enqueue(phase1);
    }
    public void PauseCurrentPhase(int sec)
    {
        MatchSystem.currentMachPhase.Pause(sec);
    }
    public void ResumeCurrentPhase()
    {
        MatchSystem.currentMachPhase.Resume();
    }
    public void ResetTimer()
    {
        time = 0;
    }
    // Update is called once per frame
    void Update()
    {
       
    }
    private void FixedUpdate()
    {
          ps.Simulate(Time.fixedDeltaTime);
    }

}
