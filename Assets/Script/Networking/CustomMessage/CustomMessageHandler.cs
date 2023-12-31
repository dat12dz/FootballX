using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CustomMessageHandler : NetworkBehaviour
{
    NetworkManager netmang = NetworkManager.Singleton;
    CustomMessagingManager MessageHandler = NetworkManager.Singleton.CustomMessagingManager;
    /// <summary>
    /// Gửi gói tin từ client đến server hoặc từ server tới tất cả client
    /// </summary>
    /// <param name="clma"></param>
    public void SendMessage(byte[] clma)
    {
        var writer = new FastBufferWriter();
        writer.WriteBytes(clma);
        if (IsClient)
        {
            MessageHandler.SendUnnamedMessage(0, writer);
        }
        else
        {
            MessageHandler.SendUnnamedMessageToAll(writer);
        }
    }
    public void SendMessage(ulong client, byte[] message)
    {
        var writer = new FastBufferWriter();
        writer.WriteBytes(message);
        MessageHandler.SendUnnamedMessage(client, writer);
    }
    public void SendMessage(string clma)
    {
        var writer = new FastBufferWriter();
        writer.WriteValue(clma);
        if (IsClient)
        {
            MessageHandler.SendUnnamedMessage(0, writer);
        }
        else
        {
            MessageHandler.SendUnnamedMessageToAll(writer);
        }
    }
    public void SendMessage(ulong client, string message)
    {
        var writer = new FastBufferWriter();
        writer.WriteValue(message);
        MessageHandler.SendUnnamedMessage(client, writer);
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
