
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class StartGameInfo : MonoBehaviour
{
    public static StartGameInfo instance;
    public InitialPlayerData playerData = new InitialPlayerData();
    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
public class InitialPlayerData : INetworkSerializable
{
   public FixedString32Bytes playerName;
    public int playerChar;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerChar);
    }
}