using Assets.Script;
using Assets.Script.NetCode;

using Assets.Utlis;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LookDev;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
[DefaultExecutionOrder(0)]
public partial class Player : NetworkBehaviour
{

    public static Player localPlayer;
    public static Player BallHolder;
    public Camera Playereyes;
    [Header("Grab")]
    public Transform Graber;
    public Grabable grabitem;
    public bool CanGrab = true;
    [Header("Shoot")]
    public float MinUltraShootForce = 2,MaxUltraShootForce = 10;
    public float UltraShootForceSpeed = 1;
    [SerializeField] Transform head_;
    public float FinalShootForce;
    public Action<float> OnFinalShootForceChange;
    [Header("Throw")]
    public float MinThrowingForce = 4;
    public float MaxThrowingForce = 10;
    public float ThrowingForceSpeed = 4;
    public float FinalThrowForce;
    public Action<float> OnFinalThrowForceChange;
    [Header("Character model")]
    [SerializeField] AllCharectorAssetReference ModelRef;
    public PlayerModelBase thisPlayerModel;
    public Transform PlayerLookAt;
    [Header("Role")]
    public TeamClass team = null;
    public bool isGoalKeeper;
    [Header("Score system")]
    public uint Score;
    public NetworkVariable<uint> TouchedBallTimes;
    public NetworkVariable<uint> GoalTimes;
    // GoalKeeperOnly
    public NetworkVariable<uint> CatchedBallTimes;
    [Header("ETC")]
    [HideInInspector]
    public Vector3 Vel;
    [SerializeField] GameObject Toggle;
    [SerializeField] public NetworkVariable<bool> isInGame;
    public GameSystem System;
    public PlayerNetworkTransform networkTransform_;
    public PhysicsScene thisPhysicScene;
    [HideInInspector]
    public NetworkVariable<InitialPlayerData> initialPlayerData;
    [HideInInspector]
    public NetworkVariable<bool> isSuppress;
    void OnPlayerInitialDataChange(InitialPlayerData old, InitialPlayerData curr)
    {
        if (!thisPlayerModel)
        {
            NameTag_.text = curr.playerName.ToString();
            var PlayerModel = Instantiate(ModelRef.CharArray[curr.playerChar], Toggle.transform);
            thisPlayerModel = PlayerModel;
         //   PlayerModel.gameObject.SetActive(true);
        }
    }
    
    public Transform SpawnPoint;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsLocalPlayer)
            try
            {
                OnPlayerInitialDataChange(null, initialPlayerData.Value);
            }
            catch
            {

            }
        initialPlayerData.OnValueChanged += OnPlayerInitialDataChange;
        // Gán sự kiện cho isInGame
      //  ChangeSceneEffect.Open();
        isInGame.OnValueChanged += (pev, new_) =>
        {
            if (new_)
            {
                PlayerRoomManager roomManager = GetComponent<PlayerRoomManager>();
                UINew_LobbyScreenRoomRender.instance.gameObject.SetActive(false);
                if (IsLocalPlayer && !IsHost)
                {
                    Scene a;
                    // Định nghĩa action
                    UnityAction<Scene, LoadSceneMode> loadSceneComplete = null;
                    // gán loadscene complete

                    // loadscene complete
                    loadSceneComplete = (scene, loadmode) =>
                    {
                        SceneManager.SetActiveScene(scene);
                        SceneManager.sceneLoaded -= loadSceneComplete;

                    };
                    SceneManager.sceneLoaded += loadSceneComplete;
                    UINew_ChangeSceneEffect.Open();
                    SceneManager.LoadScene(2, new LoadSceneParameters(LoadSceneMode.Additive));

                }
                roomManager.ChangeColorModel(roomManager.SlotInRoom.Value);
                TogglePoolObj(true);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }    
        
        };
      
        if (thisPhysicScene == null)
        {
            Logging.LogError("Không tìm thấy physic scene có thể đây là scene chính");
        }
    }

    void Start()
    {
        //DontDestroyOnLoad(gameObject);
        TogglePoolObj(false);
        Init();
    }
   public void TogglePoolObj(bool o)
    {
        Toggle.SetActive(o);
        GetComponent<Move>().enabled = o;
        GetComponent<CharacterController>().enabled = o;
        //  GetComponent<MeshRenderer>().enabled = o;
        thisPhysicScene = gameObject.scene.GetPhysicsScene();
        TouchedBallTimes.OnValueChanged += (old, curr) => CalcScore();
        GoalTimes.OnValueChanged += (old, curr) => CalcScore();
        CatchedBallTimes.OnValueChanged += (old, curr) => CalcScore();
    }
    void Init()
    {
        oldPosition = transform.position;
        if (IsLocalPlayer)
        {
            localPlayer = this;
        }
        Playereyes = GetComponentInChildren<Camera>(includeInactive: true);
        if (IsLocalPlayer)
            SendPlayerNameToServerRpc(StartGameInfo.instance.playerData);
    }

    public void Shootball(Ball b)
    {
        b.lastToucher = this;
        b.Shoot(head_.transform.forward * FinalShootForce);
        FinalShootForce = MinUltraShootForce;
        if (OnShootBall != null)
            OnShootBall();
        TouchedBallTimes.Value++;
    }
        [Header("Look at setting")]
        public LayerMask lookatMask;
        GameObject oldCheckedGameobj;
       [SerializeField] float LookatDistance = 5;
        public void LookAt()
        {
          RaycastHit hit;
            
           var result = thisPhysicScene.Raycast(Playereyes.transform.position,Playereyes.transform.forward, out hit, LookatDistance, lookatMask);
            if (result && hit.collider != null)
            {
                GameObject raycasthitGameobj = hit.collider.gameObject;
                if (raycasthitGameobj != oldCheckedGameobj)
                {
                /// just newly checkked
                      raycasthitGameobj.GetComponent<Outline>().OutlineWidth = 8.66f;
                      if (oldCheckedGameobj != null)
                      {
                         oldCheckedGameobj.GetComponent<Outline>().OutlineWidth = 0;
                      }
                     oldCheckedGameobj = raycasthitGameobj;
                }

            }
            else
            {
                if (oldCheckedGameobj != null)
                {
                    oldCheckedGameobj.GetComponent<Outline>().OutlineWidth = 0;
                    oldCheckedGameobj = null;
                }
            }
        }
    float UltraShootForceVel = 0;
    float ThrowForceVel;
    Vector3 oldPosition;

    void Update()
   {
        
        if (IsLocalPlayer)
        {
            if (Input.GetKeyDown(ControlKey.GrabNThrow)) {
                FinalThrowForce = MinThrowingForce;
                ThrowForceVel = ThrowingForceSpeed;
            }
            if (Input.GetKey(ControlKey.GrabNThrow))
            {
                FinalThrowForce += ThrowForceVel * Time.deltaTime;
                if (FinalThrowForce < MinThrowingForce)
                {
                    ThrowForceVel = ThrowingForceSpeed;
                }
                if (FinalThrowForce > MaxThrowingForce)
                {
                    ThrowForceVel = -ThrowingForceSpeed;
                }
                if(OnFinalThrowForceChange != null) OnFinalThrowForceChange(FinalThrowForce);
            }
            if (Input.GetKeyUp(ControlKey.GrabNThrow))
            {
                GrabItemOrThrowServerRpc(FinalThrowForce);
            }
            if (Input.GetKeyDown(ControlKey.Shoot))
            {
                // start holding
                FinalShootForce = MinUltraShootForce;
                UltraShootForceVel = UltraShootForceSpeed;

            }
            if (Input.GetKey(ControlKey.Shoot))
            {
                FinalShootForce += UltraShootForceVel * Time.deltaTime;
                if (FinalShootForce > MaxUltraShootForce)
                {
                    UltraShootForceVel = -UltraShootForceSpeed;
                }

                if (FinalShootForce < MinUltraShootForce)
                {
                    UltraShootForceVel = UltraShootForceSpeed;
                }

                if(OnFinalShootForceChange != null) OnFinalShootForceChange(FinalShootForce);
            }
            if (isInGame.Value)
            LookAt();
            if (Input.GetKeyUp(ControlKey.Shoot))
            {
                SendShootForceServerRpc(FinalShootForce);
            }
        }

       
    }
    private void FixedUpdate()
    {
        void CalcVel()
        {
            Vel = transform.position - oldPosition;
            oldPosition = transform.position;
        }
        CalcVel();
    }
    public void AutoShootBall(TweenCallback OnComplete = null)
    {
        networkTransform_.ClientAuth.Value = false;
        Ball ball = System.sceneReference.ball;
        var t = transform.DOMove(ball.transform.position, 0.5f);
        t.onComplete += () =>
        {
            networkTransform_.ClientAuth.Value = true;
        };
        if (OnComplete != null)
            t.onComplete += OnComplete;
    }
    Move moveHelper;
    public void SuppressPlayer(bool a,Vector3 SuppressZoneCenter = default,float SuppressZoneRad = 0)
    {
        SuppressPlayerBase(a);
        isSuppress.Value = a;
        moveHelper.SuppressZoneAnchor.Value = SuppressZoneCenter;
        moveHelper.SuppressRadius.Value = SuppressZoneRad;   
    }
    public void SuppressPlayer(bool a,Collider suppressZone)
    {
        SuppressPlayerBase(a);
        isSuppress.Value = a;
        moveHelper.SuppressZone = suppressZone;
        Logging.Log("Đã dừng người chơi:" + a);
    }   
    public void SuppressPlayerBase(bool a)
    {     
        if (!moveHelper) 
            moveHelper = GetComponent<Move>(); 
        if (!a)
        {
            moveHelper.SuppressZone = null;
        }
    }
    [ContextMenu("Teleback to spawn")]
    public void TelebackToSpawnPoint()
    {
        transform.position = SpawnPoint.position;
        networkTransform_.TeleportImidiateClientRpc(SpawnPoint.position);
    }
    [Header("Unstanable Zone")]//
    [SerializeField] GameObject Unstandable_Zone;
    [HideInInspector]
    public bool isUnstanableZoneActive;
    public void ToggleUnstanableZone(bool a)
    {
        isUnstanableZoneActive = a;
        Unstandable_Zone.SetActive(a);
    }
    public void CalcScore()
    {
        if (isGoalKeeper) Score = CatchedBallTimes.Value * 30;
        else
        Score = GoalTimes.Value * 100 + TouchedBallTimes.Value;       
    }
}
