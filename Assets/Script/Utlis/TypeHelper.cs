using Assets.Utlis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assets.Script.Utlis
{
    static class TypeHelper
    {
        public static List<MemberInfo> GetAllFieldAndPropIn(Type t)
        {
            List<MemberInfo> res = new List<MemberInfo> ();
            res.AddRange(t.GetFields());
            res.AddRange(t.GetProperties());
            return res;
        }
        /// <summary>
        /// Get all field and prop
        /// </summary>
        /// <param name="t">Type of the class</param>
        /// <param name="attribute"> Attribute </param>
        /// <returns></returns>
        public static List<MemberInfo> GetAllFieldAndPropIn(Type t,Type attribute)
        {
          var ad =  t.GetFields();
          
            List<MemberInfo> res = new List<MemberInfo>();
            res.AddRange(t.GetFields().Where(x => x.GetCustomAttribute(attribute) != null));
            res.AddRange(t.GetProperties().Where(x => x.GetCustomAttribute(attribute) != null));
            return res;
        }
        /// <summary>
        /// Set value for member info
        /// </summary>
        /// <param name="m"> Properties</param>
        /// <param name="obj"> Class with the Properties </param>
        /// <param name="value"> The value need to set </param>
        public static void SetValueFor(MemberInfo m,object obj, object value)
        {
            if (m is PropertyInfo)
            {
                var PropInfo = (PropertyInfo)m;
                PropInfo.SetValue(obj, value);
                return;
            }
            if (m is FieldInfo)
            {
                var PropInfo = (FieldInfo)m;
                PropInfo.SetValue(obj, value);
                return;
            }
            Logging.Log("Member info this not Feild or Properties");
        }
        /// <summary>
        /// Get Value for a Prop
        /// </summary>
        /// <param name="m"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetValueFor(MemberInfo m, object obj)
        {
            if (m is PropertyInfo)
            {
                var PropInfo = (PropertyInfo)m;
                return PropInfo.GetValue(obj);
            }
            if (m is FieldInfo)
            {
                var PropInfo = (FieldInfo)m;
                 return  PropInfo.GetValue(obj);
            }
            Logging.Log("Member info this not Feild or Properties, Cannot get the value");

            return null;
        }
        public static string GetNameFor(MemberInfo m)
        {
            if (m is PropertyInfo)
            {
                var PropInfo = (PropertyInfo)m;
                return m.Name;
            }
            if (m is FieldInfo)
            {
                var PropInfo = (FieldInfo)m;
                return m.Name;
            }
            return "";
        }
        public static Type GetTypeFor(MemberInfo m)
        {
            if (m is PropertyInfo)
            {
                var PropInfo = (PropertyInfo)m;
                return PropInfo.PropertyType;
            }
            if (m is FieldInfo)
            {
                FieldInfo PropInfo = m as FieldInfo;
                return PropInfo.FieldType; 
            }
            return null;
        }
    }
}
