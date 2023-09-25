using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class pnl_JionRoom : MonoBehaviour
{
    public uint RoomID;
    public uint RoomPlayerCount;
    public uint RoomMaxPlayer;
    public string RoomName;
    [SerializeField] TextMeshProUGUI lb_RoomName;
    [SerializeField] Button JoinBtn;
    public void Set(uint roomID_, uint roomPlayerCount_,uint RoomMaxPlayer_)
    {
        RoomID = roomID_;
        RoomPlayerCount = roomPlayerCount_;
        RoomMaxPlayer = RoomMaxPlayer_;
        lb_RoomName.text = RoomName;   
    }
     void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
