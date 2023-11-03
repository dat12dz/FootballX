
using Assets.Script.NetCode;

using Assets.Script.Utlis;
using Assets.Utlis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public void TeleportXZ(float x,float z)
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
        Vector2 SuppressPlayerCenter()
        {
            if (SuppressRadius.Value > 0)
            {
                return new Vector2 (SuppressZoneAnchor.Value.x, SuppressZoneAnchor.Value.z);
            }
            else
            {
                return new Vector2(transform.position.x, transform.position.z);
            }
        }
        if (MathHelper.DistanceNoSqrt(new Vector2(x, z), SuppressPlayerCenter()) > MathHelper.Power2(SafeDistanceCheck))
        {
            nettrans.TeleportImidiateClientRpc(transform.position);
                Logging.LogError("Người chơi chạy quá nhanh");
                return;
        }
        else
        {
                var newPosition = new Vector3(x, transform.position.y, z);
                Collider[] result = new Collider[1];
                PhysicsScene physics = gameObject.scene.GetPhysicsScene();
               int collider_got = physics.OverlapSphere(newPosition,0.001f, result,unstandableZone_mask,QueryTriggerInteraction.UseGlobal);
                if (collider_got == 0 || result[0].gameObject == UnstandableZone)
                {
                    transform.position = newPosition;
                }

        }
    }

    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    public void MoveCameraServerRpc(float CameraX,float headY)
    {
        if (head == null) Logging.Log("Không thể tìm thấy Head. Gắn nó vào đúng vị trí ");
        playereye.transform.localRotation = Quaternion.Euler(CameraX, 0 , 0);
        head.transform.localRotation = Quaternion.Euler(0, headY, 0);
       nettrans.ChangeCameraRotationClientRpc(CameraX, headY);
    }
    public void MovePlayer(Vector3 v)
    {
        controller.Move(v);
    }
    public void MovePlayer(float x,float y, float z)
    {

        controller.Move(new Vector3(x, y, z));
    }
    // Update is called once per frame

    void Update()
    {

        
        var MoveDirectionn = new Vector3();

        if (IsLocalPlayer)
        {
            // Up
            playereye.enabled = true;
            if (Input.GetKey(ControlKey.GoForward))
            {
                MoveDirectionn = head.transform.forward;

            }
            // Down
            if (Input.GetKey(ControlKey.GoBackward))
            {
                MoveDirectionn = -head.transform.forward;
            }

            // Right
            if (Input.GetKey(ControlKey.GoRight))
            {
                MoveDirectionn = head.transform.right;
            }
            // Left
            if (Input.GetKey(ControlKey.GoLeft))
            {
                MoveDirectionn = -head.transform.right;
            }

            if (Input.GetKeyDown(ControlKey.Jump))
            {
                JumpServerRpc();
            }

            if (MoveDirectionn != Vector3.zero)
            {
             
                if (!(player.isSuppress.Value && SuppressRadius.Value == 0))
                {                  
                        MovePlayer(MoveDirectionn * RuntimeSpeed * Time.deltaTime);
                        MovePlayerXZServerRpc(transform.position.x, transform.position.z);
                }

            }

            else
            {

            }
        }
        if (IsServer)
        {
            GravityRuntime -= GravityInit *Time.deltaTime;
            MovePlayer(0, GravityRuntime * Time.deltaTime, 0);
        }

    }
    [SerializeField] float AirResist = 3;
    private void FixedUpdate()
    {
        if (!jump)
        {
            isGrounded = CheckisGrounded();
            if (!isGrounded)
            {
                RuntimeSpeed = InitSpeed / AirResist;
            }
            else
            {
                RuntimeSpeed = InitSpeed;
            }
        }
        else
        {
            jump = false;
        }

    }
    [SerializeField] float jumpHeight = 9;
    bool jump = true;
    [ServerRpc]
   public void JumpServerRpc()
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
