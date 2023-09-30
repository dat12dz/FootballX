using Assets.Script;
using Assets.Script.Player;
using Assets.Utlis;
using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using UnityEngine.SceneManagement;

public partial class Player : NetworkBehaviour
{
    public static Player BallHolder;
    public static Player localPlayer;
    Camera Playereyes;
    [Header("Grab")]
    public Transform Graber;
    public Grabable grabitem;
    [Header("Shoot")]
    public float MinUltraShootForce = 2,MaxUltraShootForce = 10;
    public float UltraShootForceSpeed = 1;
    float FinalShootForce;
    [Header("Throw")]
    public float MinThrowingForce = 4;
    public float MaxThrowingForce = 10;
    public float ThrowingForceSpeed = 4;
    float FiinalThrowForce ;
    [Header("ETC")]
    [SerializeField] GameObject Toggle;
    [SerializeField] public NetworkVariable<bool> isInGame;
    public NetworkVariable<FixedString32Bytes> PlayerName;
    public Transform SpawnPoint;
    public override void OnNetworkSpawn()
    {
        isInGame.OnValueChanged += (pev, new_) =>
        {
            if (IsLocalPlayer)
            {
                
                UI_SelectRoomUI.instance.ToggleVisible(new_);
                if (new_)
                SceneManager.LoadScene(2,LoadSceneMode.Additive);
            }
            TogglePoolObj(new_);
        };
        base.OnNetworkSpawn();
    }
    private void Awake()
    {
    
    }
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        TogglePoolObj(false);
        Init();

    }
    void TogglePoolObj(bool o)
    {
        Toggle.SetActive(o);
        GetComponent<Move>().enabled = o;
        GetComponent<CharacterController>().enabled = o;
        GetComponent<MeshRenderer>().enabled = o;

    }
    void Init()
    {
        if (IsLocalPlayer)
        {
            localPlayer = this;
        }
        Playereyes = GetComponentInChildren<Camera>(includeInactive: true);
        if (IsLocalPlayer)
            SendPlayerNameToServerRpc(StartGameInfo.instance.PlayerName);
    }
    public virtual void SetBallHolder()
    {
        BallHolder = this;
        Ball.instance.Grab(Graber);
    }

    public void Shootball(Ball b)
    {
      
        b.Shoot(transform.forward * FinalShootForce);
        FinalShootForce = MinUltraShootForce;
    }
        [Header("Look at setting")]
        public LayerMask lookatMask;
        GameObject oldCheckedGameobj;
       [SerializeField] float LookatDistance = 5;
        public void LookAt()
        {
          RaycastHit hit;
            
           var result = Physics.Raycast(Playereyes.transform.position,Playereyes.transform.forward, out hit, LookatDistance, lookatMask);
            if (result && hit.collider != null)
            {
                GameObject raycasthitGameobj = hit.collider.gameObject;
                if (raycasthitGameobj!= oldCheckedGameobj)
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
}
