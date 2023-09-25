using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;

public class PlayerNetworkTransform : NetworkBehaviour
{
    Vector3 ServerPositon;
    public float DistanceCheckk = 0.02f;
    public bool Interpolate = true;
    public float InterplolateSpeed = 1;
    Move moveHelper;
    void Start()
    {
        moveHelper = GetComponent<Move>();
    }
    [ClientRpc] void ChangePosInClientClientRpc(Vector3 newPos)
    {
        
        if (IsLocalPlayer)
        {
            ServerPositon = new Vector3(transform.position.x, newPos.y, transform.position.z);
            if (!Interpolate) 
            transform.position = ServerPositon;
        }
        else
        {
            if (!Interpolate)
                transform.position = newPos;
            ServerPositon = newPos;
        }
    }
    [ClientRpc] public void ChangeCameraRotationClientRpc(float CameraX, float HeadY)
    {
        if (IsLocalPlayer) return;
        moveHelper.playereye.transform.localRotation = Quaternion.Euler(CameraX, 0, 0);
        moveHelper.head.transform.localRotation = Quaternion.Euler(0, HeadY, 0);

    }
    [ClientRpc] public void TeleportImidiateClientRpc(Vector3 v)
    {
        transform.position = v;
    }
    Vector3 oldPos;
    private void Update()
    {
        if (IsServer)
        {
            if (Vector3.Distance(transform.position, oldPos ) > DistanceCheckk)
            {

                ChangePosInClientClientRpc(transform.position);
                oldPos = transform.position;
            }
        }
        if (IsClient && Interpolate)
        {
            // Making some interpolation wiith server positionn;
            if (IsLocalPlayer)
            ServerPositon = new Vector3(transform.position.x, ServerPositon.y, transform.position.z);

            /* transform.position = Vector3.Lerp(transform.position,ServerPositon, InterplolateSpeed * Time.deltaTime);*/
            var moveDir = (ServerPositon - transform.position).normalized;
            transform.position += moveDir * InterplolateSpeed * Time.deltaTime;
        }
    }

}
