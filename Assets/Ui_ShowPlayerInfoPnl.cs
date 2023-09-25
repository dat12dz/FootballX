using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ui_ShowPlayerInfoPnl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lb_PlayerName;
    [SerializeField] Button KickBtn;
    public PlayerRoomManager roomManager;
    public void init(string playername)
    {
        lb_PlayerName.text = playername;
    }
    void Start()
    {
        gameObject.active = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
