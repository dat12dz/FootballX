using Assets.Utlis;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Setting",menuName = "DatDauTechonologies/Setting")]
public class SettingDAO : ScriptableObject
{
    public static SettingDAO Instance;
    public static string appdata;
    public SettingDAO()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        appdata = Application.persistentDataPath;
        Application.quitting += () => {
            SaveFile();
        };
        LoadFile();
    }

    [Serializable]
    public class Graphic : ISaveableContent
    {
        
        public int ChosenPreset;
        [JsonIgnore]
        [SerializeField] public RenderPipelineAsset[] GraphicPresetArray;

        public override void ConstantSaver<T>(T temp)
        {
            var graphicTemp = temp as Graphic;
            graphicTemp.GraphicPresetArray = GraphicPresetArray;

        }
        public void SetGraphicPreset(int index)
        {
            ChosenPreset = index;
            QualitySettings.renderPipeline = GraphicPresetArray[index];
        }
    }
    [Serializable]
    public class Gameplay : ISaveableContent
    {
       public float MouseSen;

        public override void ConstantSaver<T>(T temp)
        {
            
        }
    }
    [Serializable]
    public class Sound : ISaveableContent
    {
        public float volume;
        public void ChangeVolume(float v)
        {
            AudioListener.volume = v;
        }

        public override void ConstantSaver<T>(T temp)
        {
            
        }
    }
    public abstract class ISaveableContent
    {
        public ISaveableContent()
        {
            FileName = "/" +  GetType().Name + ".json";

        }
        public static void Save(ISaveableContent t)
        {
            var json =  JsonConvert.SerializeObject(t);
            File.WriteAllText(appdata + t.FileName, json);
        }
        public static void Load<T>(ref T t) where T : ISaveableContent
        {
              var json=   File.ReadAllText(appdata + t.FileName);
              var temp = JsonConvert.DeserializeObject<T>(json);
              t.ConstantSaver(temp);
              t = temp;
        }
        [JsonIgnore]
        string FileName;
        public abstract void ConstantSaver<T>(T temp) where T : ISaveableContent;

    }
   public Graphic graphicSetting;
   public Gameplay gameplaySetting;
   public Sound SoundSetting;

    public void SaveFile()
    {
        ISaveableContent.Save(graphicSetting);
        ISaveableContent.Save(gameplaySetting);
        ISaveableContent.Save(SoundSetting);
    }
    public void LoadFile()
    {
        ISaveableContent.Load(ref graphicSetting);
        ISaveableContent.Load(ref gameplaySetting);
        ISaveableContent.Load(ref SoundSetting);
    }

}

