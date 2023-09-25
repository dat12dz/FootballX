using Assets.Utlis;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[CheckNullProperties]

public class UIHandler : MonoBehaviour
{

    public static UIHandler Instance { get; set; }
    [SerializeField] TextMeshProUGUI txtblk_Time;
    [SerializeField] TextMeshProUGUI txtblk_RedTeamScore;
    [SerializeField] TextMeshProUGUI txtblk_BlueTeamScore;
    [SerializeField] UI_IngameInformer pnl_ingameInformer;
    [SerializeField] Button Btn_Disconnect;
    void Start()
    {
      //  CheckNullOrNot();
        Instance = this;    
        GameSystem game = GameSystem.instance;
        game.OnTimeChange += ShowTime;
        game.OnScoreChange += ShowScore;
        Btn_Disconnect.onClick.AddListener(Btn_disCOnnect);
        Cursor.lockState = CursorLockMode.Locked;
       
    }
    void Btn_disCOnnect()
    {
      //  NetworkSystem.instance.TryDisconnectoall();
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);

    }
    void CheckNullOrNot()
    {
        Logging.CheckNLogObjectNull(pnl_ingameInformer, nameof(pnl_ingameInformer));
        Logging.CheckNLogObjectNull(txtblk_BlueTeamScore, nameof(txtblk_BlueTeamScore));
        Logging.CheckNLogObjectNull(txtblk_Time, nameof(txtblk_Time));
        Logging.CheckNLogObjectNull(txtblk_RedTeamScore, nameof(txtblk_RedTeamScore));
    }
    void ShowScore(int Red,int Blue, GameSystem.Team Goaler)
    {
        Action res = null;
       if (Goaler == GameSystem.Team.red)
        {       
            res = () => pnl_ingameInformer.Show("Goalllll!!", "Red team gooalll",10);  
        }
        if (Goaler == GameSystem.Team.blue)
        {
            res = () => pnl_ingameInformer.Show("Goalllll!!", "Blue team gooalll",10);
        }
       MainThreadDispatcher.ExecuteInMainThread(res);
        var RedScore = Red.ToString();
        var BlueScore = Blue.ToString();
        MainThreadDispatcher.ExecuteInMainThread(() =>
        {
            txtblk_RedTeamScore.text = RedScore;
            txtblk_BlueTeamScore.text = BlueScore;
        });

    }
    void ShowTime(int time)
    {
        var Minute = time / 60;
        var Second = time % 60;
        var ShowMinString = Minute <= 9 ? $"0{Minute}" : Minute.ToString();
        var ShowSecondString = Second <= 9 ? $"0{Second}" : Second.ToString();

        MainThreadDispatcher.ExecuteInMainThread(() => txtblk_Time.SetText(ShowMinString + ":" + ShowSecondString));
           
    }
    public void ShowInformation(string Title,string Infomation,int sec)
    {
        if (!pnl_ingameInformer.IsDestroyed())
        {


            MainThreadDispatcher.ExecuteInMainThread(() => { pnl_ingameInformer.Show(Title, Infomation, sec); });
        }
    }
     bool isCursorLocked = true;

    void LockCursor()
    {
        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = !isCursorLocked;
            LockCursor();
        }
    }
}
