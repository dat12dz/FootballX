using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class UI_IngameInformer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tblock_title;
    [SerializeField] TextMeshProUGUI tblock_information;
    public void Show(string title,string Information,int sec)
    {
        
        tblock_title.text = title;
        tblock_information.text = Information;
        gameObject.active = true;
        Thread a = new Thread(()=> {
            Thread.Sleep(sec * 1000);
            MainThreadDispatcher.ExecuteInMainThread(() =>
            {
               if (gameObject)
                gameObject.active = false;
            });
            
        });
        a.Name = "Main Informator Waitor";
        a.Start();
    }
    
}
