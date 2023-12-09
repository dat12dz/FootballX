using Assets.Script;
using Assets.Script.UI;
using Assets.Script.Utlis;
using Assets.Utlis;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using Dweiss;
public class Ball : Grabable
{
    [SerializeField] SoundPlayer soundPlayer;
    public Player lastToucher;
    public Rigidbody rb;
 
    // Client instance
    public static Ball instance;
    NetworkObject thisNetobj;
    public GameSystem thisSceneGameSystem;
    Transform ballSpawnPoint;
    public OnCatchableZoneEnum OnCatchableZone = OnCatchableZoneEnum.None;
    [Header("Trajectory")]
    [SerializeField] LineRenderer trajectoryRender;
    [SerializeField] float step;
    private void Awake()
    {
        //  base.Awake();

    }
    public void init(GameSystem g, Transform spawnPoint)
    {
        thisSceneGameSystem = g;
        ballSpawnPoint = spawnPoint;
    }
    public override void OnNetworkSpawn()//
    {
        base.OnNetworkSpawn();
        if (IsClient && !IsHost)
            gameObject.SetActive(true);
        Debug.Log(gameObject.name);
        
    }
    void Start()
    {
        if (!IsSpawned) gameObject.SetActive(false);
        if (IsClient)
            instance = this;
        rb = GetComponent<Rigidbody>();
        thisNetobj = NetworkObject;
        if (IsClient) 
        {
            var navBall = Navigator.instance;
            navBall.ball = this;
        }

    }

    public void Shoot(Vector3 force)
    {
        rb.AddForce(force, ForceMode.VelocityChange);
    }
    public void Shoot(float x, float y, float z)
    {
        rb.AddForce(new Vector3(x, y, z), ForceMode.VelocityChange);
    }

    protected override void Update()
    {
        base.Update();
        rb.CalculateMovement(0, Player.localPlayer.FinalShootForce);
    }

    public override bool Grab(Transform graber_, bool isPlayerAction)
    {
        Player Graber = graber_.root.GetComponent<Player>();
        if (isPlayerAction && Graber.isGoalKeeper)
        {
            if ((byte)OnCatchableZone != (byte)Graber.team.team)
            {
                Logging.LogError("Không thể nhặt bóng trong vùng cấm nhặt");
                return false;
            }

        }
        return base.Grab(graber_);
    }
    private void OnCollisionEnter(Collision collision)
    {
        soundPlayer.PlayRandomSound();
    }
    public void Suppress(bool t)
    {
        rb.isKinematic = t;
    }
    public void BackToSpawnPos()
    {
        transform.position = ballSpawnPoint.position;
    }
}
