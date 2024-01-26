using Assets.Script.Utlis;
using Assets.Utlis;
using System;
using Unity.Netcode;
using UnityEngine;

public partial class Move : NetworkBehaviour
{
    [SerializeField] CharacterController controller;
    public float InitSpeed, RuntimeSpeed;
    public Camera playereye;
    NetworkManager netmang = NetworkManager.Singleton;
    public float SafeDistanceCheck = 2f;
    PlayerNetworkTransform nettrans;
    public Transform head;
    LayerMask unstandableZone_mask;
    Player player;
    [Header("Suppress Zone")]
    public NetworkVariable<Vector3> SuppressZoneAnchor;

    public NetworkVariable<float> SuppressRadius;
    public Collider SuppressZone;
    [SerializeField] GameObject UnstandableZone;
    void Start()
    {

        controller = GetComponent<CharacterController>();
        if (controller == null) Logging.LogObjectNull(nameof(controller));
        RuntimeSpeed = InitSpeed;
        playereye = GetComponentInChildren<Camera>();
        playereye.enabled = false;
        nettrans = GetComponent<PlayerNetworkTransform>();
        player = GetComponent<Player>();
        unstandableZone_mask = LayerMask.GetMask("UnstanableZone");
    }
    public void MovePlayer_ManuallySpeed(Vector3 v)
    {
       transform.position +=  v * RuntimeSpeed * Time.deltaTime;
        
    }
    public void TeleportXZ(float x, float z)
    {
        var newPos = new Vector3(x, transform.position.y, z);
        if (Vector3.Distance(newPos, transform.position) < SafeDistanceCheck)
        {
            transform.position = newPos;
        }
        else
        {
            Logging.LogError("Người chơi chạy quá nhanh");
        }
    }
    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    public void MovePlayerXZServerRpc(float x, float z)
    {
        bool? SuppressionZoneCheck() // Retturn true khi người chơi đang ở trong vùng 
        {
            if (SuppressZone && player.isSuppress.Value)
            {
                Collider[] hit = new Collider[1];
                player.thisPhysicScene.OverlapSphere(new Vector3(x, transform.position.y, z), 0.1f, hit, LayerMask.GetMask("GoalkeeperZone"), QueryTriggerInteraction.Collide);
                if (hit.Length > 0 && hit[0] == SuppressZone)
                {
                    return true;
                }
                else
                    return false;
            }
            return null;
        }
        bool? SuppressionZoneCheckResult = SuppressionZoneCheck();

        Vector2 SuppressPlayerCenter()
        {

            if (SuppressRadius.Value > 0)
            {
                return new Vector2(SuppressZoneAnchor.Value.x, SuppressZoneAnchor.Value.z);
            }
            return new Vector2(transform.position.x, transform.position.z);


        }
        if (MathHelper.DistanceNoSqrt(new Vector2(x, z), SuppressPlayerCenter()) > MathHelper.Power2(SafeDistanceCheck) || SuppressionZoneCheckResult == false)
        {
            nettrans.TeleportImidiateClientRpc(transform.position);
            Logging.LogError("Người chơi chạy quá nhanh");

            if (IsHost && player.networkTransform_.ClientAuth.Value)
            {
                MovePlayer(-MoveDirectionn * RuntimeSpeed * Time.deltaTime);
            }
            return;


        }
        else
        {
            var newPosition = new Vector3(x, transform.position.y, z);
            Collider[] result = new Collider[1];
            PhysicsScene physics = gameObject.scene.GetPhysicsScene();
            int collider_got = physics.OverlapSphere(newPosition, 0.001f, result, unstandableZone_mask, QueryTriggerInteraction.UseGlobal);
            if (collider_got == 0 || result[0].gameObject == UnstandableZone)
            {
                transform.position = newPosition;
            }

        }
    }

    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    public void MoveCameraServerRpc(float CameraX, float headY)
    {
        if (head == null) Logging.Log("Không thể tìm thấy Head. Gắn nó vào đúng vị trí ");
        playereye.transform.localRotation = Quaternion.Euler(CameraX, 0, 0);
        head.transform.localRotation = Quaternion.Euler(0, headY, 0);
        nettrans.ChangeCameraRotationClientRpc(CameraX, headY);
    }
    public void MovePlayer(Vector3 v)
    {
        controller.Move(v);
    }
    public void MovePlayer(float x, float y, float z)
    {

        controller.Move(new Vector3(x, y, z));
    }
    // Update is called once per frame
    Vector3 MoveDirectionn = new Vector3();
    void Update()
    {


        MoveDirectionn = Vector3.zero;

        if (IsLocalPlayer)
        {
            // Up
            playereye.enabled = true;
            if (Input.GetKey(ControlKey.GoForward))
            {
                MoveDirectionn += head.transform.forward;

            }
            // Down
            if (Input.GetKey(ControlKey.GoBackward))
            {
                MoveDirectionn += -head.transform.forward;
            }

            // Right
            if (Input.GetKey(ControlKey.GoRight))
            {
                MoveDirectionn += head.transform.right * 0.75f;
            }
            // Left
            if (Input.GetKey(ControlKey.GoLeft))
            {
                MoveDirectionn += -head.transform.right * 0.75f;
            }

            if (Input.GetKeyDown(ControlKey.Jump))
            {
                if (!IsHost)
                    JumpServerRpc();
                Jump();
            }

            if (MoveDirectionn != Vector3.zero)
            {

                if (!(player.isSuppress.Value && SuppressRadius.Value == 0 && SuppressZone == null))
                {

                    MovePlayer(MoveDirectionn * RuntimeSpeed * Time.deltaTime);
                    MovePlayerXZServerRpc(transform.position.x, transform.position.z);
                }

            }
        }
        GravityRuntime -= GravityInit * Time.deltaTime;

        MovePlayer(0, GravityRuntime * Time.deltaTime, 0);
        if (IsServer)
        {

        }
        isGrounded = CheckisGrounded();

    }
    float NextYPosition;
    [SerializeField] float AirResist = 3;
    private void FixedUpdate()
    {
        if (!jump)
        {

            if (isGrounded)
                if (landed != null) { landed(); }

            if (!isGrounded)
            {
                RuntimeSpeed = InitSpeed / AirResist;
            }
            else
            {
                RuntimeSpeed = InitSpeed;
            }
        }
  

    }
    [SerializeField] float jumpHeight = 9;
    bool jump = true;
    public Action landed;
    [ServerRpc]
    public void JumpServerRpc()
    {
        Jump();
    }
    public void Jump()
    {
        if (isGrounded)
        {
            jump = true;
            GravityRuntime = Mathf.Sqrt(2 * GravityInit * jumpHeight);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(GroundCheck.position, GroundCheckRadius);
    }
}
