using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_RoomRenderPnl : MonoBehaviour
{
    public static UI_RoomRenderPnl instance;    
    [SerializeField] TextMeshProUGUI lb_RoomID;
    public Ui_ShowPlayerInfoPnl[] ShowPlayerInfoPnl;

    public void init(uint roomid)
    {
        lb_RoomID.text = roomid.ToString();
    }

    void Start()
    {
        instance = this;
        gameObject.active = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
