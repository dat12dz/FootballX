using Mono.CSharp;
using Newtonsoft.Json;
using System;
using System.IO;
using UINew;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Setting", menuName = "DatDauTechonologies/Setting")]
public class SettingDAO : MonoBehaviour
{
    public static SettingDAO Instance;
    public static string appdata;
    public SettingDAO()
    {
        Instance = this;
    }
    private void Start()
    {
        appdata = Application.persistentDataPath;
        Application.quitting += () =>
        {
            SaveFile();
        };
        LoadFile();
    }

    [Serializable]
    [TabName("Graphic")]
    public class Graphic : ISaveableContent
    {
        [ShowOnSetting("Graphic mode", ShowOnSettingAttribute.SettingBindingType.SliderInt, 0, 3)]
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
    [TabName("Gameplay")]
    public class Gameplay : ISaveableContent
    {
        public enum LanguageEnum
        {
            English,
            Vietnamese,
            Chinese
        }

        [ShowOnSetting("Mouse Sensitivity", ShowOnSettingAttribute.SettingBindingType.FloatField)]
        public float MouseSen = 90;
        [ShowOnSetting("Language", ShowOnSettingAttribute.SettingBindingType.DropdownField, new string[] { "English", "Vietnamese", "Chinese" })]
        public LanguageEnum Language;
        Assets.Utlis.Rotate LocalPlayerRotate;
        public override void ConstantSaver<T>(T temp)
        {

        }
        public void SetMouseSen(float val)
        {
            MouseSen = val;
            try
            {
                if (!LocalPlayerRotate)
                {
                    LocalPlayerRotate = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Assets.Utlis.Rotate>();
                    LocalPlayerRotate.MouseSen = val;
                }
            }
            catch
            {

            }
        }
    }
    [Serializable]
    [TabName("Sound")]
    public class Sound : ISaveableContent
    {
        [ShowOnSetting("Volume", ShowOnSettingAttribute.SettingBindingType.SliderFloat, minValue: 0, maxValue: 1)]
        public float volume;
        public void ChangeVolume(float v)
        {
            volume = v;
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
            FileName = "/" + GetType().Name + ".json";

        }
        public static void Save(ISaveableContent t)
        {
            var json = JsonConvert.SerializeObject(t);
            File.WriteAllText(appdata + t.FileName, json);
        }
        public static void Load<T>(ref T t) where T : ISaveableContent
        {
            var json = File.ReadAllText(appdata + t.FileName);
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
        var t = typeof(SoundPlayer);

    }
    public void LoadFile()
    {
        ISaveableContent.Load(ref graphicSetting);
        ISaveableContent.Load(ref gameplaySetting);
        ISaveableContent.Load(ref SoundSetting);
    }
}

