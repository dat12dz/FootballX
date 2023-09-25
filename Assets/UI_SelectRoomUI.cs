using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectRoomUI : MonoBehaviour
{
    [SerializeField] Button btn_CreateRoom;
    void Start()
    {

        btn_CreateRoom.onClick.AddListener(() =>
        {
           PlayerRoomManager localRoomManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerRoomManager>();
            localRoomManager.CreateRoomServerRpc();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
