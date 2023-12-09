using Assets.Script.Utlis;
using Assets.Script.Utlis.CheckNullProp;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkTransform : NetworkBehaviour
{
    Vector3 ServerPositon;
    public NetworkVariable<bool> ClientAuth;
    public float DistanceCheckk = 0.02f;
    public bool Interpolate = true;
    public float InterplolateSpeed = 1;
    Move moveHelper;
    public float YDistanceCheck = 3;
    void Start()
    {
        if (IsHost) Interpolate = true;
        moveHelper = GetComponent<Move>();
        ClientAuth.Value = true;
    }
    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    void ChangePosInClientClientRpc(Vector3 newPos)
    {
        // Nhận dữ liệu từ server;

        // Nếu xác thực trên client
        if (IsLocalPlayer && ClientAuth.Value)
        {
            /*  ServerPositon = new Vector3(transform.position.x, newPos.y, transform.position.z);
              ServerPositon = transform.position;
              if (!Interpolate)
                  transform.position = ServerPositon; */

            if (Mathf.Abs(transform.position.y - newPos.y) > YDistanceCheck * YDistanceCheck)
            {
                transform.position = newPos;
                Request_YposAndYGrav_ServerRpc();
            }
        }
        else
        {
            if (!Interpolate)
                transform.position = newPos;
            ServerPositon = newPos;
        }
    }
    [ServerRpc]
    void Request_YposAndYGrav_ServerRpc(ServerRpcParams rpc = default)
    {
        var requester = NetworkkHelper.CreateRpcTo(rpc.Receive.SenderClientId);
        Respone_YposAndYGrav_ClientRpc(moveHelper.GravityRuntime, requester);
    }
    [ClientRpc]
    void Respone_YposAndYGrav_ClientRpc(float yGrav, ClientRpcParams rpc)
    {
        moveHelper.GravityRuntime = yGrav;
    }
    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    public void ChangeCameraRotationClientRpc(float CameraX, float HeadY)
    {
        if (IsLocalPlayer) return;
        moveHelper.playereye.transform.localRotation = Quaternion.Euler(CameraX, 0, 0);
        moveHelper.head.transform.localRotation = Quaternion.Euler(0, HeadY, 0);

    }
    [ClientRpc]
    public void TeleportImidiateClientRpc(Vector3 v)
    {
        transform.position = v;
    }
    Vector3 oldPos;
    private void Update()
    {
        
        if (IsServer)
            if (MathHelper.DistanceNoSqrt(transform.position, oldPos) > DistanceCheckk * DistanceCheckk)
            {

                ChangePosInClientClientRpc(transform.position);
                oldPos = transform.position;
            }


        if (!IsServer && Interpolate)
        {
            // Making some interpolation wiith server positionn;
            if (IsLocalPlayer)
            {
                ServerPositon = transform.position;
                Interpolate = false;
            }
            /* transform.position = Vector3.Lerp(transform.position,ServerPositon, InterplolateSpeed * Time.deltaTime);*/
            var moveDir = (ServerPositon - transform.position).normalized;
            transform.position += moveDir * InterplolateSpeed * Time.deltaTime;
        }
    }

}
