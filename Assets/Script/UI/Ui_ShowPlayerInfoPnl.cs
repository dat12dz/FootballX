using Assets.Utlis;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(0)]
public class Ui_ShowPlayerInfoPnl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lb_PlayerName;
    [SerializeField] Button KickBtn;
    public PlayerRoomManager roomManager;
    public Image img_isHeader;
    public void init(PlayerRoomManager roomManager_)
    {
        if (roomManager_ == null)
        {
            gameObject.SetActive(true);
            roomManager = roomManager_;
            lb_PlayerName.text = roomManager_.thisPlayer.PlayerName.Value.ToString();
            onHeaderChange(false, roomManager_.isHeader.Value);
            roomManager_.isHeader.OnValueChanged += onHeaderChange;
        }
        else
        {
            Logging.LogError("Không thể điền 2 người vào 1 vị trí trên UI");
        }
    }
    void onHeaderChange(bool old,bool curr)
    {
        img_isHeader.gameObject.SetActive(curr);
    }
    public void de_init()
    {
        roomManager.isHeader.OnValueChanged -= onHeaderChange;
        roomManager = null;
        gameObject.SetActive(false);
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
