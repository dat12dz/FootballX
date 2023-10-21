using Assets.Utlis;
using Assets.Script.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MultiplePlayerHandle : MonoBehaviour
{
    private VisualElement root;
    private TextField inputName;
    private TextField inputIp;
    private Button localIpBtn;
    private Button connectBtn;
    private Button hostBtn;
    private Button characterBtn;
    [SerializeField] CharacterScreen charSelectionUI;
    //[SerializeField] TMP_InputField inp_PlayerName, inp_ServerIP;
    //[SerializeField] Button btn_Connect, btn_autoLocalhost, btn_HostBtn, btn_ShowCharSelectionUI;
    public static StartSceneInfo StartSceneInfoSync;
    
    NetworkManager netmang;
    int maxHostPlayer = 10;
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        inputName = root.Q<TextField>("input-name");
        inputIp = root.Q<TextField>("input-ip");
        localIpBtn = root.Q<Button>("local-ip-btn");
        connectBtn = root.Q<Button>("connect-btn");
        hostBtn = root.Q<Button>("host-btn");
        characterBtn = root.Q<Button>("character-btn");
        netmang = NetworkManager.Singleton;
        maxHostPlayer = 10;
        Logging.CheckNLogObjectNull(inputName, nameof(inputName));
        Logging.CheckNLogObjectNull(inputIp, nameof(inputIp));
        Logging.CheckNLogObjectNull(connectBtn, nameof(connectBtn));
        Logging.CheckNLogObjectNull(localIpBtn, nameof(localIpBtn));


        localIpBtn.clicked += () =>
        {
            inputIp.value = "127.0.0.1";
        };

        connectBtn.clicked += () =>
        {
            if (inputName.text == string.Empty)
            {
                MessageBox.Show("Player name cannot empty", "Please fill in your player name");
                return;
            }
            if (inputIp.text == string.Empty)
            {
                if (netmang.StartServer())
                    NetworkServer.StartServer();
            }
            else
            {
                NetworkClient_.StartClient(inputIp.text, inputName.text);
                StartGameInfo.instance.playerData.playerName = inputName.text;
            }
        };

        hostBtn.clicked += () =>
        {
            StartGameInfo.instance.playerData.playerName = inputName.text;
            netmang.ConnectionApprovalCallback = (req, res) =>
            {
                if (netmang.ConnectedClients.Count > maxHostPlayer)
                {
                    res.Approved = false;
                    res.Reason = "Server is full";
                }
                res.Approved = true;
                res.CreatePlayerObject = true;
            };
            if (netmang.StartHost())
                //SceneManager.LoadScene(1);
                ChangeSceneEffect.ChangeScene(1);
        };

        characterBtn.clicked += () =>
        {
            charSelectionUI.Display(true);
            //gameObject.SetActive(false);
            root.style.display = DisplayStyle.None;
        };
        if (FirstTimeInit)
        {
            netmang.OnServerStopped += Netmang_OnServerStopped;
            netmang.OnClientStopped += Netmang_OnClientStopped;
            netmang.OnTransportFailure += Netmang_OnTransportFailure;
            netmang.OnClientDisconnectCallback += Netmang_OnClientDisconnectCallback;
            FirstTimeInit = false;  
        }
        
    }
    public static bool FirstTimeInit = true;
    private void Clickable_clicked()
    {
        throw new System.NotImplementedException();
    }

    private void Netmang_OnClientDisconnectCallback(ulong obj)
    {
        if (!netmang.IsServer)
            MessageBox.Show("Disconnect from server", $"{netmang.DisconnectReason}");
    }

    private void Netmang_OnTransportFailure()
    {
        //SceneManager.LoadScene(0);
        ChangeSceneEffect.ChangeScene(0);
    }

    private void Netmang_OnClientStopped(bool obj)
    {
        //SceneManager.LoadScene(0);
        ChangeSceneEffect.ChangeScene(0);
    }

    private void Netmang_OnServerStopped(bool obj)
    {
        //SceneManager.LoadScene(0);
        ChangeSceneEffect.ChangeScene(0);
    }
}
