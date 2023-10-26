using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Setting",menuName = "DatDauTechonologies/Setting")]
public class SettingDAO : ScriptableObject,IBinding
{
    public static SettingDAO Instance { get; private set; }


    public SettingDAO() {
        Instance = this;
    }
    [Serializable]
    public class Graphic
    {

        int ChosenPreset;
        [SerializeField] public RenderPipelineAsset[] GraphicPresetArray;
        public void SetGraphicPreset(int index)
        {
            ChosenPreset = index;
            QualitySettings.renderPipeline = GraphicPresetArray[index];
        }
    }
    [Serializable]
    public class Gameplay
    {
        float MouseSen;
    }
    [Serializable]
    public class Sound
    {
        public float volume;
        public void ChangeVolume(float v)
        {
            AudioListener.volume = v;
        }
    }
   public Graphic graphicSetting;
   public Gameplay gameplaySetting;
   public Sound SoundSetting;
    const string FileName = "Setting.json";
    public void SaveFile()
    {
        File.WriteAllText(FileName, JsonConvert.SerializeObject(this));
    
    }
    public void LoadFile()
    {

    }

    public void PreUpdate()
    {
       
    }

    public void Update()
    {
     
    }

    public void Release()
    {
      
    }
}

