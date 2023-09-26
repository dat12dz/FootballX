//using Assets.Utlis;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class MessageBox : MonoBehaviour
//{
//    public static MessageBox Instance;
//    [SerializeField] TextMeshProUGUI Title, message;
//    [SerializeField] Button Btn_Ok;
//    private void Start()
//    {
//        Instance = this;
//        Btn_Ok.onClick.AddListener(() => { gameObject.SetActive(false); });
//        gameObject.SetActive(false);
//    }
//   public void ShowBase(string Title_,string message_)
//    {
//        Logging.CheckNLogObjectNull(Title, nameof(Title));
//        Logging.CheckNLogObjectNull(message, nameof(message));
//        Logging.CheckNLogObjectNull(Btn_Ok, nameof(Btn_Ok));
//        Title.text = Title_;
//        message.text = message_;    
//        gameObject.SetActive(true);
//    }
//    /// <summary>
//    /// Execute on main thread
//    /// </summary>
//    /// <param name="title"></param>
//    /// <param name="mess"></param>
//    public static void Show(string title,string mess)
//    {

//        if (Instance)
//          Instance.ShowBase(title,mess);
//        else Logging.Log("Cannot find message box.Please instantiate message box game object");
//    }
//}
