using System;
using UnityEngine.Rendering;

namespace UINew
{
    public class ShowOnSettingAttribute : Attribute
    {
        public ShowOnSettingAttribute(string name, SettingBindingType type, int minValue = 0, int maxValue = 100)
        {
            Name = name;
            BindingType = type;
            MinValue = minValue;
            MaxValue = maxValue;
        }
        public string Name { private set; get; }
        public int MinValue { private set; get; }
        public int MaxValue { private set; get; }        
        public SettingBindingType BindingType { private set; get; }
        public enum SettingBindingType
        {
            SliderInt,
            SliderFloat,
            IntegerField,
            FloatField,
            TextField,
            Toggle
        }
    }
    public class TabNameAttribute : Attribute
    {
        public TabNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { set; get; }
    }
}