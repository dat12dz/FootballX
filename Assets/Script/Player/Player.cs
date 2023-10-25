using Assets.Script;
using Assets.Script.NetCode;
using Assets.Script.Player;
using Assets.Utlis;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
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
    [Header("Shoot")]
    public float MinUltraShootForce = 2,MaxUltraShootForce = 10;
    public float UltraShootForceSpeed = 1;
    [SerializeField] Transform head_;
    float FinalShootForce;
    [Header("Throw")]
    public float MinThrowingForce = 4;
    public float MaxThrowingForce = 10;
    public float ThrowingForceSpeed = 4;
    float FiinalThrowForce ;
    [Header("Character model")]
    [SerializeField] AllCharectorAssetReference ModelRef;
    public PlayerModelBase thisPlayerModel;
    public Transform PlayerLookAt;
    [Header("Role")]
    public TeamClass team = null;
    [Header("ETC")]
    [SerializeField] GameObject Toggle;
    [SerializeField] public NetworkVariable<bool> isInGame;
    public uint Score;
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
            PlayerRoomManager roomManager = GetComponent<PlayerRoomManager>();
            UINew_LobbyScreenRoomRender.instance.gameObject.SetActive(false);           
            if (IsLocalPlayer && !IsHost)
            {
                Scene a;               
               // Định nghĩa action
                UnityAction<Scene, LoadSceneMode> loadSceneComplete = null;
                // gán loadscene complete
              
                // loadscene complete
               loadSceneComplete = (scene, loadmode) => {
                    SceneManager.SetActiveScene(scene);
                   SceneManager.sceneLoaded -= loadSceneComplete;

                };
                SceneManager.sceneLoaded += loadSceneComplete;
                UINew_ChangeSceneEffect.Open();
                 SceneManager.LoadScene(2, new LoadSceneParameters(LoadSceneMode.Additive));

            }
            roomManager.ChangeColorModel(roomManager.SlotInRoom.Value);
            TogglePoolObj(new_);
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
    void TogglePoolObj(bool o)
    {
        Toggle.SetActive(o);
        GetComponent<Move>().enabled = o;
        GetComponent<CharacterController>().enabled = o;
      //  GetComponent<MeshRenderer>().enabled = o;
        thisPhysicScene = gameObject.scene.GetPhysicsScene();

    }
    void Init()
    {

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
    void Update()
   {
       
        if (IsLocalPlayer)
        {
            if (Input.GetKeyDown(ControlKey.GrabNThrow)) {
                FiinalThrowForce = MinThrowingForce;
                ThrowForceVel = ThrowingForceSpeed;
            }
            if (Input.GetKey(ControlKey.GrabNThrow))
            {
                FiinalThrowForce += ThrowForceVel * Time.deltaTime;
                if (FiinalThrowForce < MinThrowingForce)
                {
                    ThrowForceVel = ThrowingForceSpeed;
                }
                if (FiinalThrowForce > MaxThrowingForce)
                {
                    ThrowForceVel = -ThrowingForceSpeed;
                }
                
            }
            if (Input.GetKeyUp(ControlKey.GrabNThrow))
            {
                if (isGrabed.Value == false)
                GrabItemOrThrowServerRpc(0);
                else
                GrabItemOrThrowServerRpc(FiinalThrowForce);
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

            }
            if (isInGame.Value)
            LookAt();
            if (Input.GetKeyUp(ControlKey.Shoot))
            {
                SendShootForceServerRpc(FinalShootForce);
            }
        }
    }
    public void SuppressPlayer(bool a)
    {
        isSuppress.Value = a;
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
    void ToggleUnstanableZone(bool a)
    {
        isUnstanableZoneActive = a;
        Unstandable_Zone.SetActive(a);
    }
}
