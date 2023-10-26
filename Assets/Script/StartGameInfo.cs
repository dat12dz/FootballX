
using Assets.Utlis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
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
        {
            Destroy(gameObject);
            return;
        }
        Application.quitting += Application_quitting;
        LoadFile();

        DontDestroyOnLoad(gameObject);
      
    }
    void LoadFile()
    {
       var StartScreenJson = File.ReadAllText(Application.persistentDataPath + "/StartScreenSaver.json");
        try
        {
            playerData = JsonConvert.DeserializeObject<InitialPlayerData>(StartScreenJson);
        }
        catch (Exception e)
        {
            Logging.LogError("Không thể đọc được file--------------");
            Logging.Log(e);
        }
    }
    private void Application_quitting()
    {   
        File.WriteAllText(Application.persistentDataPath + "/StartScreenSaver.json", JsonConvert.SerializeObject(playerData));
    }
}

public class InitialPlayerData : INetworkSerializable
{
    [JsonConverter(typeof(FixedStringCOnverter))]
    public FixedString32Bytes playerName;
    public int playerChar;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerName);
        
        serializer.SerializeValue(ref playerChar);
    }
}
public class FixedStringCOnverter : JsonConverter<FixedString32Bytes>
{
    public override FixedString32Bytes ReadJson(JsonReader reader, Type objectType, FixedString32Bytes existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return (string)reader.Value;
    }

    public override void WriteJson(JsonWriter writer, FixedString32Bytes value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}